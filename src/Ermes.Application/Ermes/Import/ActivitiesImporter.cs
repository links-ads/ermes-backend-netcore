using Abp.UI;
using Ermes.Activities;
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
using Ermes.Enums;
using Ermes.Helpers;

namespace Ermes.Import
{
    public static class ActivitiesImporter
    {
        private const int MAX_CODE_LENGTH = 8;
        private interface IMultilanguageActivityTable
        {
            IEnumerable<IActivityTranslationRow> Rows { get; }
        }

        private interface IActivityTranslationRow
        {
            [StringLength(MAX_CODE_LENGTH)]
            String ShortName { get; }
            [StringLength(MAX_CODE_LENGTH)]
            String ParentShortName { get; }
            String HazardString { get; }
            HazardType Hazard { get; }
            IEnumerable<(String Translation, String Language)> Items { get; }
        }
        #region Excel
        const int numberOfHeaderCols = 3;
        const int numberOfHeaderRows = 1;
        const int excelIndexingStart = 1;
        private class ExcelActivityTranslationRow : IActivityTranslationRow
        {
            ExcelRange _cells;
            int _rowIndex;
            int _length;

            public ExcelActivityTranslationRow(ExcelRange cells, int rowIndex, int length)
            {
                _cells = cells;
                _rowIndex = rowIndex;
                _length = length - numberOfHeaderCols;
            }

            public IEnumerable<(string Translation, string Language)> Items
            {
                get
                {
                    for(int i=0; i<_length; i++)
                    {
                        string t = _cells[_rowIndex + numberOfHeaderRows + excelIndexingStart, i + numberOfHeaderCols + excelIndexingStart].Text;
                        if(!String.IsNullOrWhiteSpace(t))
                            yield return (t, _cells[excelIndexingStart, i + numberOfHeaderCols + excelIndexingStart].Text);
                        else
                            yield return (null, _cells[excelIndexingStart, i + numberOfHeaderCols + excelIndexingStart].Text);
                    }
                }
            }

            public string ShortName
            {
                get { return _cells[_rowIndex + numberOfHeaderRows + excelIndexingStart, excelIndexingStart].Text; }
            }

            public string ParentShortName
            {
                get
                {
                    string psn = _cells[_rowIndex + numberOfHeaderRows + excelIndexingStart, excelIndexingStart + 1].Text;
                    return String.IsNullOrWhiteSpace(psn) ? null : psn;
                }
            }

            public string HazardString
            {
                get
                {
                    string hazard = _cells[_rowIndex + numberOfHeaderRows + excelIndexingStart, excelIndexingStart + 2].Text;
                    return String.IsNullOrWhiteSpace(hazard) ? null : hazard;
                }
            }

            public HazardType Hazard { get { return HazardString.ParseEnum<HazardType>();  } }
        }
        private class ExcelMultilanguageActivityTable : IMultilanguageActivityTable
        {
            private ExcelRange _cells;
            int length;
            public ExcelMultilanguageActivityTable(ExcelRange cells, ErmesLocalizationHelper localizer)
            {
                _cells = cells;
                for (length = 3; !String.IsNullOrWhiteSpace(cells[excelIndexingStart, length + excelIndexingStart].Text); length++) ;
                for (int i = 3; i < length; i++)
                {
                    string lcode = cells[excelIndexingStart, excelIndexingStart + i].Text;
                    if (!CultureInfo.GetCultures(CultureTypes.AllCultures).Any(c => c.TwoLetterISOLanguageName == lcode))
                        throw new UserFriendlyException(localizer.L("NotALanguageCode", lcode));
                }
            }

            public IEnumerable<IActivityTranslationRow> Rows
            {
                get
                {
                    for(int i=0; !String.IsNullOrWhiteSpace(_cells[i + excelIndexingStart + numberOfHeaderRows, excelIndexingStart].Text) ; i++)
                    {
                       yield return new ExcelActivityTranslationRow(_cells, i, length);
                    }
                }
            }
        }
        #endregion
        public static async Task<ImportResultDto> ImportActivitiesAsync(string filename, string contentType, ActivityManager activityManager, ErmesLocalizationHelper localizer)
        {
            IMultilanguageActivityTable activities;
            ImportResultDto result = new ImportResultDto();
            if (contentType == MimeTypeNames.ApplicationVndMsExcel || contentType == MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet)
            {
                FileInfo finfo = new FileInfo(filename);
                ExcelPackage epack = new ExcelPackage(finfo);
                epack.Compatibility.IsWorksheets1Based = false;
                activities = new ExcelMultilanguageActivityTable(epack.Workbook.Worksheets[0].Cells, localizer);
            }
            else
            {
                throw new NotImplementedException();
            }

            foreach (IActivityTranslationRow activity in activities.Rows)
            {
                if (activity.ShortName.Length > MAX_CODE_LENGTH)
                    throw new UserFriendlyException(localizer.L("InvalidActivityCode", activity.ShortName, MAX_CODE_LENGTH));

                Activity act = await activityManager.GetActivityByShortNameAsync(activity.ShortName);
                Activity parentAct = null;

                if (activity.ParentShortName != null)
                {
                    parentAct = await activityManager.GetActivityByShortNameAsync(activity.ParentShortName);
                    if (parentAct == null)
                    {
                        throw new UserFriendlyException(localizer.L("ParentNotYetDeclared", activity.ParentShortName)); // If we create the parent now, it could be possible to have a circular dependency
                    }
                }

                if (act == null)
                {
                    act = new Activity()
                    {
                        ShortName = activity.ShortName,
                        ParentId = parentAct?.Id,
                        Hazard = activity.Hazard
                    };
                    act.Id = await activityManager.InsertActivityAsync(act);
                    result.ElementsAdded++;
                }
                else
                {
                    act.ParentId = parentAct?.Id;
                    act.Hazard = activity.Hazard;
                    result.ElementsUpdated++;
                }

                foreach ((String Translation, String Language) activitytrans in activity.Items)
                {
                    if (activitytrans.Translation == null)
                    {
                        result.TranslationsDetectedEmptyAndSkipped++;
                        continue;
                    }

                    ActivityTranslation at = await activityManager.getActivityTranslationByCoreIdAndLangAsync(act.Id, activitytrans.Language);

                    if (at == null)
                    {
                        at = new ActivityTranslation()
                        {
                            CoreId = act.Id,
                            Language = activitytrans.Language,
                            Name = activitytrans.Translation
                        };
                        await activityManager.InsertActivityTranslationAsync(at);
                        result.TranslationsAdded++;
                    }
                    else
                    {
                        at.Name = activitytrans.Translation;
                        result.TranslationsUpdated++;
                    }
                }
            }

            return result;
        }
    }
}
