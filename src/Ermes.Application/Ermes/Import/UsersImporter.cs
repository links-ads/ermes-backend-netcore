using Abp.Domain.Uow;
using Abp.UI;
using Ermes.Activities;
using Ermes.Categories;
using Ermes.Enums;
using Ermes.Helpers;
using Ermes.Import.Dto;
using Ermes.Net.MimeTypes;
using Ermes.Localization;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ermes.Persons;
using Ermes.Organizations;

namespace Ermes.Import
{
    public static class UsersImporter
    {
        public const string IndexSheetName = "Index";

        private interface IUserTable
        {
            IEnumerable<IUserSheet> Sheets { get; }
        }

        private interface IUserSheet
        {
            IEnumerable<IUserRow> Rows { get; }
        }

        private interface IUserRow
        {
            int GetInt(string columnName)
            {
                return Convert.ToInt32(GetString(columnName) ?? "0");
            }
            string GetString(string columnName);
            string[] GetStringArray(string columnName)
            {
                return GetString(columnName)?.Split(',').Select(s => s.Trim()).ToArray();
            }
            T GetEnum<T>(string columnName) where T:Enum
            {
                return GetString(columnName).ParseEnum<T>();
            }
        }

        #region Excel
        const int numberOfHeaderRows = 1;
        const int excelIndexingStart = 1;

        private class ExcelUserTable : IUserTable
        {
            private ExcelWorksheets _excelWorksheets;
            public ExcelUserTable(ExcelWorksheets excelWorksheets)
            {
                _excelWorksheets = excelWorksheets;
            }
            public IEnumerable<IUserSheet> Sheets { get { return _excelWorksheets.Select(w => new ExcelUserSheet(w)); } }
        }

        private class ExcelUserSheet: IUserSheet
        {
            private readonly ExcelWorksheet _excelWorksheet;
            private readonly Dictionary<string, int> _columnsIndexes = new Dictionary<string, int>();
            public ExcelUserSheet(ExcelWorksheet excelWorksheet)
            {
                _excelWorksheet = excelWorksheet;
                string colName;

                for (int i = 0; !string.IsNullOrWhiteSpace((colName = _excelWorksheet.Cells[excelIndexingStart, excelIndexingStart + i].Text)); i++)
                    _columnsIndexes.Add(colName.Trim(), i);
                    
            }
            public string GetTextForColumn(string columnName, int rowId)
            {
                if (!_columnsIndexes.TryGetValue(columnName, out int columnIndex))
                {
                    throw new UserFriendlyException($"No such column: {columnName} in sheet {Language}");
                }
                string retval = _excelWorksheet.Cells[rowId + numberOfHeaderRows + excelIndexingStart, columnIndex + excelIndexingStart].Text.Trim();

                if (string.IsNullOrWhiteSpace(retval))
                    return null;
                else
                    return retval;
            }
            public IEnumerable<IUserRow> Rows { 
                get {
                    for (int i = 0; !string.IsNullOrWhiteSpace(_excelWorksheet.Cells[i + numberOfHeaderRows + excelIndexingStart, excelIndexingStart].Text); i++)
                        yield return new ExcelUserRow(this, i);
                } }
        }

        private class ExcelUserRow : IUserRow
        {
            ExcelUserSheet _parent;
            int _index;
            public ExcelUserRow(ExcelUserSheet parent, int index)
            {
                _parent = parent;
                _index = index;
            }
            public string GetString(string columnName)
            {
                return _parent.GetTextForColumn(columnName, _index);
            }            
        }

        #endregion
        public static async Task<ImportResultDto> ImportUsersAsync(string filename, string contentType, PersonManager personManager, OrganizationManager organizationManager, IActiveUnitOfWork context)
        {
            IUserTable users;
            ImportResultDto result = new ImportResultDto();
            if (contentType == MimeTypeNames.ApplicationVndMsExcel || contentType == MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet)
            {
                FileInfo finfo = new FileInfo(filename);
                ExcelPackage epack = new ExcelPackage(finfo);
                epack.Compatibility.IsWorksheets1Based = false;
                users = new ExcelUserTable(epack.Workbook.Worksheets);
            }
            else
            {
                throw new NotImplementedException();
            }

            bool isFirstSheet = true;
            foreach (IUserSheet sheet in users.Sheets)
            {
                if (!isFirstSheet)
                    break;
                foreach (IUserRow row in sheet.Rows)
                {
                    Person person = personManager.GetPersonByUsername(row.GetString("Username"));
                    ProfileDto
                    if (person != null)
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        person = new Person();
                        result.ElementsAdded++;
                    }

                    var organizationId = row.GetInt("OrganizationId");
                    var org = await organizationManager.GetOrganizationByIdAsync(organizationId);
                    if (org == null)
                        throw new UserFriendlyException(string.Format("Invalid Organization Id: {0}", organizationId));
                    else
                        person.OrganizationId = organizationId;

                    person.Username = row.GetString("Username");
                    person.TeamId = row.GetString("TeamId");
                    var 
                    cat.Code = 
                    cat.GroupCode = row.GetString("GroupCode");
                    cat.Hazard = row.GetEnum<HazardType>("Hazard");
                    cat.MaxValue = row.GetInt("Max Value");
                    cat.MinValue = row.GetInt("Min Value");
                    cat.Type = row.GetEnum<CategoryType>("Type");
                    cat.StatusValues = row.GetStringArray("Status Values");
                    cat.GroupIcon = row.GetString("Group Icon");

                    await categoryManager.InsertOrUpdateCategoryAsync(cat);
                    context.SaveChanges();
                }
                isFirstSheet = false;
            }

            return result;
        }
    }
}
