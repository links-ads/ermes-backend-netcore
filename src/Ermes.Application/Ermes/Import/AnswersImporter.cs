using Abp.Domain.Uow;
using Abp.UI;
using Ermes.Answers;
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
    public static class AnswersImporter
    {
        public const string IndexSheetName = "Index";

        public static async Task<ImportResultDto> ImportAnswersAsync(string filename, string contentType, AnswerManager manager, ErmesLocalizationHelper localizer, IActiveUnitOfWork context)
        {
            IMultilanguageTable answers;
            ImportResultDto result = new ImportResultDto();
            if (contentType == MimeTypeNames.ApplicationVndMsExcel || contentType == MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet)
            {
                FileInfo finfo = new FileInfo(filename);
                ExcelPackage epack = new ExcelPackage(finfo);
                epack.Compatibility.IsWorksheets1Based = false;
                answers = new ExcelMultilanguageTable(epack.Workbook.Worksheets);
            }
            else
            {
                throw new NotImplementedException();
            }

            bool isFirstSheet = true;
            foreach (IErmesSheet sheet in answers.Sheets)
            {
                if (sheet.Language == IndexSheetName)
                {
                    if (!isFirstSheet)
                        throw new UserFriendlyException(localizer.L("AnswerImportIndexMustBeFirst"));

                    foreach (IErmesRow row in sheet.Rows)
                    {
                        var answer = await manager.GetAnswerByCodeAsync(row.GetString("Code"));

                        if (answer != null)
                            result.ElementsUpdated++;
                        else
                        {
                            answer = new Answer();
                            result.ElementsAdded++;
                        }

                        answer.Code = row.GetString("Code");
                        answer.QuizCode = row.GetString("Quiz Code");
                        answer.IsTheRightAnswer = row.GetBoolean("Right Answer");
                        
                        await manager.InsertOrUpdateAnswerAsync(answer);
                        context.SaveChanges();
                    }
                }
                else
                {
                    if (!CultureInfo.GetCultures(CultureTypes.AllCultures).Any(c => c.TwoLetterISOLanguageName == sheet.Language))
                        throw new UserFriendlyException(localizer.L("NotALanguageCode", sheet.Language));

                    foreach (IErmesRow row in sheet.Rows)
                    {
                        var parent = await manager.GetAnswerByCodeAsync(row.GetString("Code"));

                        if (parent == null)
                            throw new UserFriendlyException(localizer.L("UnexistentEntities", "Answer", row.GetString("Code")));

                        var trans = await manager.GetAnswerTranslationByCoreIdLanguageAsync(parent.Id, sheet.Language.ToLower());

                        if (trans != null)
                        {
                            result.TranslationsUpdated++;
                        }
                        else
                        {
                            trans = new AnswerTranslation();
                            result.TranslationsAdded++;
                        }

                        trans.Text = row.GetString("Text");
                        trans.Language = sheet.Language.ToLower();
                        trans.CoreId = parent.Id;

                        await manager.InsertOrUpdateAnswerTranslationAsync(trans);
                        context.SaveChanges();
                    }
                }
                isFirstSheet = false;
            }

            return result;
        }
    }
}
