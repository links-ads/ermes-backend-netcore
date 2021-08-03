using Abp.Domain.Uow;
using Abp.UI;
using Ermes.Enums;
using Ermes.Helpers;
using Ermes.Import.Dto;
using Ermes.Localization;
using Ermes.Net.MimeTypes;
using Ermes.Quizzes;
using Ermes.Tips;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Ermes.Excel.Common.ExcelCommon;

namespace Ermes.Import
{
    public static class QuizzesImporter
    {
        public const string IndexSheetName = "Index";

        public static async Task<ImportResultDto> ImportQuizzesAsync(string filename, string contentType, QuizManager manager, ErmesLocalizationHelper localizer, IActiveUnitOfWork context)
        {
            IMultilanguageTable quizzes;
            ImportResultDto result = new ImportResultDto();
            if (contentType == MimeTypeNames.ApplicationVndMsExcel || contentType == MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet)
            {
                FileInfo finfo = new FileInfo(filename);
                ExcelPackage epack = new ExcelPackage(finfo);
                epack.Compatibility.IsWorksheets1Based = false;
                quizzes = new ExcelMultilanguageTable(epack.Workbook.Worksheets);
            }
            else
            {
                throw new NotImplementedException();
            }

            bool isFirstSheet = true;
            foreach (IErmesSheet sheet in quizzes.Sheets)
            {
                if (sheet.Language == IndexSheetName)
                {
                    if (!isFirstSheet)
                        throw new UserFriendlyException(localizer.L("QuizImportIndexMustBeFirst"));

                    foreach (IErmesRow row in sheet.Rows)
                    {
                        Quiz quiz = await manager.GetQuizByCodeAsync(row.GetString("Code"));

                        if (quiz != null)
                            result.ElementsUpdated++;
                        else
                        {
                            quiz = new Quiz();
                            result.ElementsAdded++;
                        }

                        quiz.Code = row.GetString("Code");
                        quiz.TipCode = row.GetString("Tip Code");
                        quiz.Hazard = row.GetEnum<HazardType>("Hazard");
                        quiz.CrisisPhaseKey = row.GetEnum<CrisisPhaseType>("Crisis Phase Key");
                        quiz.EventContextKey = row.GetEnum<EventContextType>("Event Context Key");
                        quiz.DifficultyKey = row.GetEnum<DifficultyType>("Difficulty Key");
                        await manager.InsertOrUpdateQuizAsync(quiz);
                        context.SaveChanges();
                    }
                }
                else
                {
                    if (!CultureInfo.GetCultures(CultureTypes.AllCultures).Any(c => c.TwoLetterISOLanguageName == sheet.Language))
                        throw new UserFriendlyException(localizer.L("NotALanguageCode", sheet.Language));

                    foreach (IErmesRow row in sheet.Rows)
                    {
                        var parent = await manager.GetQuizByCodeAsync(row.GetString("Code"));

                        if (parent == null)
                            throw new UserFriendlyException(localizer.L("UnexistentEntities", "Quiz", row.GetString("Code")));

                        var trans = await manager.GetQuizTranslationByCoreIdLanguageAsync(parent.Id, sheet.Language.ToLower());

                        if (trans != null)
                        {
                            result.TranslationsUpdated++;
                        }
                        else
                        {
                            trans = new QuizTranslation();
                            result.TranslationsAdded++;
                        }

                        trans.Text = row.GetString("Text");
                        trans.CrisisPhase = row.GetString("Crisis Phase");
                        trans.EventContext = row.GetString("Event Context");
                        trans.Difficulty = row.GetString("Difficulty");
                        trans.Language = sheet.Language.ToLower();
                        trans.CoreId = parent.Id;

                        await manager.InsertOrUpdateQuizTranslationAsync(trans);
                        context.SaveChanges();
                    }
                }
                isFirstSheet = false;
            }

            return result;
        }
    }
}
