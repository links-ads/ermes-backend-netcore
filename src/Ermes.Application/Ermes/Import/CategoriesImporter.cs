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

namespace Ermes.Import
{
    public static class CategoriesImporter
    {
        public const string IndexSheetName = "Index";

        private interface IMultilanguageCategoryTable
        {
            IEnumerable<ICategorySheet> Sheets { get; }
        }

        private interface ICategorySheet
        {
            string Language { get; }
            IEnumerable<ICategoryRow> Rows { get; }
        }

        private interface ICategoryRow
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
        private class ExcelMultilanguageCategoryTable: IMultilanguageCategoryTable
        {
            private ExcelWorksheets _excelWorksheets;
            public ExcelMultilanguageCategoryTable(ExcelWorksheets excelWorksheets)
            {
                _excelWorksheets = excelWorksheets;
            }
            public IEnumerable<ICategorySheet> Sheets { get { return _excelWorksheets.Select(w => new ExcelCategorySheet(w));  } }
        }

        private class ExcelCategorySheet: ICategorySheet
        {
            private readonly ExcelWorksheet _excelWorksheet;
            private readonly Dictionary<string, int> _columnsIndexes = new Dictionary<string, int>();
            public ExcelCategorySheet(ExcelWorksheet excelWorksheet)
            {
                _excelWorksheet = excelWorksheet;
                string colName;

                for (int i = 0; !string.IsNullOrWhiteSpace((colName = _excelWorksheet.Cells[excelIndexingStart, excelIndexingStart + i].Text)); i++)
                    _columnsIndexes.Add(colName.Trim(), i);
                    
            }
            public string Language { get { return _excelWorksheet.Name; } }
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
            public IEnumerable<ICategoryRow> Rows { 
                get {
                    for (int i = 0; !string.IsNullOrWhiteSpace(_excelWorksheet.Cells[i + numberOfHeaderRows + excelIndexingStart, excelIndexingStart].Text); i++)
                        yield return new ExcelCategoryRow(this, i);
                } }
        }

        private class ExcelCategoryRow: ICategoryRow
        {
            ExcelCategorySheet _parent;
            int _index;
            public ExcelCategoryRow(ExcelCategorySheet parent, int index)
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
        public static async Task<ImportResultDto> ImportCategoriesAsync(string filename, string contentType, CategoryManager categoryManager, ErmesLocalizationHelper localizer, IActiveUnitOfWork context)
        {
            IMultilanguageCategoryTable categories;
            ImportResultDto result = new ImportResultDto();
            if (contentType == MimeTypeNames.ApplicationVndMsExcel || contentType == MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet)
            {
                FileInfo finfo = new FileInfo(filename);
                ExcelPackage epack = new ExcelPackage(finfo);
                epack.Compatibility.IsWorksheets1Based = false;
                categories = new ExcelMultilanguageCategoryTable(epack.Workbook.Worksheets);
            }
            else
            {
                throw new NotImplementedException();
            }

            bool isFirstSheet = true;
            foreach(ICategorySheet sheet in categories.Sheets)
            {
                if(sheet.Language == IndexSheetName)
                {
                    if (!isFirstSheet)
                        throw new UserFriendlyException(localizer.L("CategoryImportIndexMustBeFirst"));

                    foreach(ICategoryRow row in sheet.Rows)
                    {
                        Category cat = await categoryManager.GetCategoryByCodeAsync(row.GetString("Code"));

                        if (cat != null)
                        {
                            result.ElementsUpdated++;
                            string newGroupCode = row.GetString("GroupCode");

                            if (newGroupCode != cat.GroupCode)
                            {
                                // If groupCode has changed, retreive a translation corresponding to the new group, or use the new groupcode as a placeholder
                                categoryManager.CategoriesTranslation
                                    .Where(ct => ct.CoreId == cat.Id)
                                    .ToList()
                                    .ForEach(ct =>
                                    {
                                        ct.Group = categoryManager.CategoriesTranslation
                                            .Where(cx => cx.Core.GroupCode == newGroupCode && cx.Language == ct.Language && cx.Group != cat.GroupCode && cx.Group != newGroupCode)
                                            .FirstOrDefault()?.Group ?? newGroupCode;
                                    });
                            }
                        }
                        else
                        {
                            cat = new Category();
                            result.ElementsAdded++;
                        }

                        cat.Code = row.GetString("Code");
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
                }
                else
                {
                    if (!CultureInfo.GetCultures(CultureTypes.AllCultures).Any(c => c.TwoLetterISOLanguageName == sheet.Language))
                        throw new UserFriendlyException(localizer.L("NotALanguageCode", sheet.Language));

                    foreach (ICategoryRow row in sheet.Rows)
                    {
                        Category parentCat = await categoryManager.GetCategoryByCodeAsync(row.GetString("Code"));

                        if (parentCat == null)
                            throw new UserFriendlyException(localizer.L("UnexistentEntities", "Category", row.GetString("Code")));

                        CategoryTranslation catTrans = await categoryManager.GetCategoryTranslationByCoreIdLanguageAsync(parentCat.Id, sheet.Language.ToLower());

                        if (catTrans != null)
                        {
                            result.TranslationsUpdated++;
                        }
                        else
                        {
                            catTrans = new CategoryTranslation();
                            result.TranslationsAdded++;
                        }

                        catTrans.Description = row.GetString("Description");
                        catTrans.Group = row.GetString("Group");
                        catTrans.SubGroup = row.GetString("SubGroup");
                        catTrans.Language = sheet.Language.ToLower();
                        catTrans.Name = row.GetString("Name");
                        catTrans.Values = row.GetStringArray("Values");
                        catTrans.UnitOfMeasure = row.GetString("Unit of Measure");
                        catTrans.CoreId = parentCat.Id;

                        // Check if other entities in this language have the same groupcode but a different translation for group, in that case update it
                        categoryManager.CategoriesTranslation
                            .Where(ct => ct.Language == catTrans.Language && ct.Core.GroupCode == parentCat.GroupCode && ct.Group != catTrans.Group)
                            .ToList()
                            .ForEach(ct => ct.Group = catTrans.Group);

                        await categoryManager.InsertOrUpdateCategoryTranslationAsync(catTrans);
                        context.SaveChanges();
                    }
                }
                isFirstSheet = false;
            }

            return result;
        }
    }
}
