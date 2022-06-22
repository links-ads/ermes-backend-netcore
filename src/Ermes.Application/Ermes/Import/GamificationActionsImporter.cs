using Abp.Domain.Uow;
using Abp.UI;
using Ermes.Enums;
using Ermes.Gamification;
using Ermes.Import.Dto;
using Ermes.Localization;
using Ermes.Net.MimeTypes;
using OfficeOpenXml;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Ermes.Excel.Common.ExcelCommon;

namespace Ermes.Import
{
    public  class GamificationActionsImporter 
    {
        public const string IndexSheetName = "Index";

        public static async Task<ImportResultDto> ImportGamificationActionsAsync(string filename, string contentType, GamificationManager gamificationManager, ErmesLocalizationHelper localizer, IActiveUnitOfWork context)
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
                        throw new UserFriendlyException(localizer.L("GamificationActionsImportIndexMustBeFirst"));

                    foreach (IErmesRow row in sheet.Rows)
                    {
                        GamificationAction act = await gamificationManager.GetActionByCodeAsync(row.GetString("Code"));

                        if (act != null)
                            result.ElementsUpdated++;
                        else
                        {
                            act = new GamificationAction();
                            result.ElementsAdded++;
                        }

                        act.Code = row.GetString("Code");
                        act.Name = row.GetString("Name");
                        act.Competence = row.GetEnum<CompetenceType>("Competence");
                        act.Points = row.GetInt("Points");

                        await gamificationManager.InsertOrUpdateActionAsync(act);
                        context.SaveChanges();
                    }
                }
                else
                {
                    if (!CultureInfo.GetCultures(CultureTypes.AllCultures).Any(c => c.TwoLetterISOLanguageName == sheet.Language))
                        throw new UserFriendlyException(localizer.L("NotALanguageCode", sheet.Language));

                    foreach (IErmesRow row in sheet.Rows)
                    {
                        GamificationAction parentAct = await gamificationManager.GetActionByCodeAsync(row.GetString("Code"));

                        if (parentAct == null)
                            throw new UserFriendlyException(localizer.L("UnexistentEntities", "GamificationAction", row.GetString("Code")));

                        GamificationActionTranslation actTrans = await gamificationManager.GetActionTranslationByCoreIdLanguageAsync(parentAct.Id, sheet.Language.ToLower());

                        if (actTrans != null)
                        {
                            result.TranslationsUpdated++;
                        }
                        else
                        {
                            actTrans = new GamificationActionTranslation();
                            result.TranslationsAdded++;
                        }

                        actTrans.Description = row.GetString("Description");
                        actTrans.CoreId = parentAct.Id;
                        actTrans.Language = sheet.Language.ToLower();

                        await gamificationManager.InsertOrUpdateActionTranslationAsync(actTrans);
                        context.SaveChanges();
                    }
                }
                isFirstSheet = false;
            }

            return result;
        }
    }
}
