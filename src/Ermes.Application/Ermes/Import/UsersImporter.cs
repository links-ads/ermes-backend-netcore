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
using Ermes.Profile.Dto;
using Ermes.Auth.Dto;
using Ermes.Teams;
using io.fusionauth.domain;
using Ermes.Authorization;
using Ermes.Gamification;
using static Ermes.Authorization.AppPermissions;

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
            int? GetInt(string columnName)
            {
                return Convert.ToInt32(GetString(columnName));
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
                    throw new UserFriendlyException($"No such column: {columnName} in sheet");
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
        public static async Task<ImportUsersDto> ImportUsersAsync(string filename, string contentType, PersonManager personManager, OrganizationManager organizationManager, TeamManager teamManager, ErmesLocalizationHelper localizer, GamificationManager gamificationManager)
        {
            IUserTable users;
            ImportUsersDto result = new ImportUsersDto() {
               Accounts = new List<Tuple<UserDto, Person>>()
            };
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
                    UserDto user = new UserDto();
                    var person = personManager.GetPersonByEmail(row.GetString("Email"));
                    if (person != null)
                        throw new NotImplementedException(string.Format("Person with Email {0} already exists, cannot be created", person.Email));
                    else
                    {
                        result.ElementsAdded++;
                        person = new Person();
                    }

                    var roles = row.GetStringArray("Roles").ToList();
                    if (roles == null || roles.Count == 0)
                        throw new UserFriendlyException(localizer.L("InvalidRoles"));
                    user.Roles = roles;

                    var organizationName = row.GetString("OrganizationName");
                    if (organizationName != null && organizationName != string.Empty)
                    {
                        Organization org = await organizationManager.GetOrganizationByNameAsync(organizationName);
                        if (org == null)
                        {
                            org = new Organization(organizationName);
                            org.Id = await organizationManager.InsertOrganizationAsync(org);
                        }
                        
                        person.OrganizationId = org.Id;

                        var teamName = row.GetString("TeamName");
                        if (teamName != null && teamName != string.Empty)
                        {
                            var team = await teamManager.GetTeamByNameAndOrganizationIdAsync(teamName, org.Id);
                            if (team == null)
                            {
                                team = new Team(org.Id, teamName);
                                team.Id = await teamManager.InsertTeamAsync(team);
                            }
                            
                            person.TeamId = team.Id;
                        }
                    }
                    else
                    {
                        //must be a citizen
                        if (user.Roles.Count(r => r == AppRoles.CITIZEN) == 0)
                            throw new UserFriendlyException(localizer.L("InvalidOrganizationName", organizationName));
                        else
                            person.LevelId = (await gamificationManager.GetDefaultLevel()).Id;
                    }

                    var email =  row.GetString("Email");
                    if (email == null || string.IsNullOrWhiteSpace(email))
                        throw new UserFriendlyException(localizer.L("InvalidEmail"));
                    else
                        person.Email = user.Email = email;

                    var username = row.GetString("Username");
                    person.Username = user.Username  = username;

                    var preferredLanguages = row.GetStringArray("PreferredLanguages").ToList();
                    if (preferredLanguages == null || preferredLanguages.Count == 0)
                        preferredLanguages.Add("en");
                    user.PreferredLanguages = preferredLanguages;


                    var timezone = row.GetString("Timezone");
                    if (timezone == null || string.IsNullOrWhiteSpace(timezone))
                        timezone = AppConsts.DefaultTimezone;
                    user.Timezone = timezone;

                    var password = row.GetString("Password");
                    if(password == null || string.IsNullOrWhiteSpace(password))
                        throw new UserFriendlyException(localizer.L("InvalidPassword", password));
                    user.Password = password;
                    
                    user.Id = Guid.NewGuid();
                    person.FusionAuthUserGuid = user.Id;
                    person.IsNewUser = false;
                    result.Accounts.Add(new Tuple<UserDto, Person>(user, person));
                }
                isFirstSheet = false;
            }

            return result;
        }
    }
}
