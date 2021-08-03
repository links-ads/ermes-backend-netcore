using Abp.Domain.Uow;
using Abp.UI;
using Ermes.Enums;
using Ermes.Helpers;
using Ermes.Import.Dto;
using Ermes.Localization;
using Ermes.Net.MimeTypes;
using Ermes.Tips;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Ermes.Excel.Common.ExcelCommon;

namespace Ermes.Import
{
    public static class TipsImporter
    {
        public const string IndexSheetName = "Index";

        public static async Task<ImportResultDto> ImportTipsAsync(string filename, string contentType, TipManager tipManager, ErmesLocalizationHelper localizer, IActiveUnitOfWork context)
        {
            IMultilanguageTable tips;
            ImportResultDto result = new ImportResultDto();
            if (contentType == MimeTypeNames.ApplicationVndMsExcel || contentType == MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet)
            {
                FileInfo finfo = new FileInfo(filename);
                ExcelPackage epack = new ExcelPackage(finfo);
                epack.Compatibility.IsWorksheets1Based = false;
                tips = new ExcelMultilanguageTable(epack.Workbook.Worksheets);
            }
            else
            {
                throw new NotImplementedException();
            }

            bool isFirstSheet = true;
            foreach (IErmesSheet sheet in tips.Sheets)
            {
                if (sheet.Language == IndexSheetName)
                {
                    if (!isFirstSheet)
                        throw new UserFriendlyException(localizer.L("TipImportIndexMustBeFirst"));

                    foreach (IErmesRow row in sheet.Rows)
                    {
                        Tip tip = await tipManager.GetTipByCodeAsync(row.GetString("Code"));

                        if (tip != null)
                            result.ElementsUpdated++;
                        else
                        {
                            tip = new Tip();
                            result.ElementsAdded++;
                        }

                        tip.Code = row.GetString("Code");
                        tip.Hazard = row.GetEnum<HazardType>("Hazard");
                        tip.CrisisPhaseKey = row.GetEnum<CrisisPhaseType>("Crisis Phase Key");
                        tip.EventContextKey = row.GetEnum<EventContextType>("Event Context Key");
                        tip.Url = row.GetString("Url");

                        await tipManager.InsertOrUpdateTipAsync(tip);
                        context.SaveChanges();
                    }
                }
                else
                {
                    if (!CultureInfo.GetCultures(CultureTypes.AllCultures).Any(c => c.TwoLetterISOLanguageName == sheet.Language))
                        throw new UserFriendlyException(localizer.L("NotALanguageCode", sheet.Language));

                    foreach (IErmesRow row in sheet.Rows)
                    {
                        Tip parent = await tipManager.GetTipByCodeAsync(row.GetString("Code"));

                        if (parent == null)
                            throw new UserFriendlyException(localizer.L("UnexistentEntities", "Tip", row.GetString("Code")));

                        TipTranslation trans = await tipManager.GetTipTranslationByCoreIdLanguageAsync(parent.Id, sheet.Language.ToLower());

                        if (trans != null)
                        {
                            result.TranslationsUpdated++;
                        }
                        else
                        {
                            trans = new TipTranslation();
                            result.TranslationsAdded++;
                        }

                        trans.Title = row.GetString("Title");
                        trans.Text = row.GetString("Text");
                        trans.CrisisPhase = row.GetString("Crisis Phase");
                        trans.EventContext = row.GetString("Event Context");
                        trans.Language = sheet.Language.ToLower();
                        trans.CoreId = parent.Id;

                        await tipManager.InsertOrUpdateTipTranslationAsync(trans);
                        context.SaveChanges();
                    }
                }
                isFirstSheet = false;
            }

            return result;
        }
    }
}
