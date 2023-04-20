using Abp.Domain.Uow;
using Abp.UI;
using Ermes.Enums;
using Ermes.Gamification;
using Ermes.Import.Dto;
using Ermes.Localization;
using Ermes.Net.MimeTypes;
using OfficeOpenXml;
using System;
using System.IO;
using System.Threading.Tasks;
using static Ermes.Excel.Common.ExcelCommon;

namespace Ermes.Import
{
    public class GamificationActionsImporter
    {
        public const string IndexSheetName = "Index";

        public static async Task<ImportResultDto> ImportActionsAsync(string filename, string contentType, GamificationManager manager, ErmesLocalizationHelper localizer, IActiveUnitOfWork context)
        {
            IMultilanguageTable actions;
            ImportResultDto result = new ImportResultDto();
            if (contentType == MimeTypeNames.ApplicationVndMsExcel || contentType == MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet)
            {
                FileInfo finfo = new FileInfo(filename);
                ExcelPackage epack = new ExcelPackage(finfo);
                epack.Compatibility.IsWorksheets1Based = false;
                actions = new ExcelMultilanguageTable(epack.Workbook.Worksheets);
            }
            else
            {
                throw new NotImplementedException();
            }

            bool isFirstSheet = true;
            foreach (IErmesSheet sheet in actions.Sheets)
            {
                if (sheet.Language == IndexSheetName)
                {
                    if (!isFirstSheet)
                        throw new UserFriendlyException(localizer.L("ImportIndexMustBeFirst"));

                    foreach (IErmesRow row in sheet.Rows)
                    {
                        GamificationAction action = await manager.GetActionByCodeAsync(row.GetString("Code"));

                        if (action != null)
                            result.ElementsUpdated++;
                        else
                        {
                            action = new GamificationAction();
                            result.ElementsAdded++;
                        }

                        action.Code = row.GetString("Code");
                        action.Name = row.GetString("Name");
                        action.Description = row.GetString("Description");
                        action.Points = row.GetInt("Points").Value;
                        action.Competence = row.GetEnum<CompetenceType>("Competence");
                        await manager.InsertOrUpdateActionAsync(action);
                        context.SaveChanges();
                    }
                }
                else
                {
                    continue;
                }
                isFirstSheet = false;
            }

            return result;
        }
    }
}
