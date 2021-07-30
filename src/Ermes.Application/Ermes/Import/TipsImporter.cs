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

namespace Ermes.Import
{
    public static class TipsImporter
    {
        public const string IndexSheetName = "Index";

        private interface IMultilanguageTipTable
        {
            IEnumerable<ITipSheet> Sheets { get; }
        }

        private interface ITipSheet
        {
            string Language { get; }
            IEnumerable<ITipRow> Rows { get; }
        }

        private interface ITipRow
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
            T GetEnum<T>(string columnName) where T : Enum
            {
                return GetString(columnName).ParseEnum<T>();
            }
        }

        #region Excel
        const int numberOfHeaderRows = 1;
        const int excelIndexingStart = 1;
        private class ExcelMultilanguageTipTable : IMultilanguageTipTable
        {
            private ExcelWorksheets _excelWorksheets;
            public ExcelMultilanguageTipTable(ExcelWorksheets excelWorksheets)
            {
                _excelWorksheets = excelWorksheets;
            }
            public IEnumerable<ITipSheet> Sheets { get { return _excelWorksheets.Select(w => new ExcelTipSheet(w)); } }
        }

        private class ExcelTipSheet : ITipSheet
        {
            private readonly ExcelWorksheet _excelWorksheet;
            private readonly Dictionary<string, int> _columnsIndexes = new Dictionary<string, int>();
            public ExcelTipSheet(ExcelWorksheet excelWorksheet)
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
            public IEnumerable<ITipRow> Rows
            {
                get
                {
                    for (int i = 0; !string.IsNullOrWhiteSpace(_excelWorksheet.Cells[i + numberOfHeaderRows + excelIndexingStart, excelIndexingStart].Text); i++)
                        yield return new ExcelTipRow(this, i);
                }
            }
        }

        private class ExcelTipRow : ITipRow
        {
            ExcelTipSheet _parent;
            int _index;
            public ExcelTipRow(ExcelTipSheet parent, int index)
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

        public static async Task<ImportResultDto> ImportTipsAsync(string filename, string contentType, TipManager tipManager, ErmesLocalizationHelper localizer, IActiveUnitOfWork context)
        {
            IMultilanguageTipTable tips;
            ImportResultDto result = new ImportResultDto();
            if (contentType == MimeTypeNames.ApplicationVndMsExcel || contentType == MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet)
            {
                FileInfo finfo = new FileInfo(filename);
                ExcelPackage epack = new ExcelPackage(finfo);
                epack.Compatibility.IsWorksheets1Based = false;
                tips = new ExcelMultilanguageTipTable(epack.Workbook.Worksheets);
            }
            else
            {
                throw new NotImplementedException();
            }

            bool isFirstSheet = true;
            foreach (ITipSheet sheet in tips.Sheets)
            {
                if (sheet.Language == IndexSheetName)
                {
                    if (!isFirstSheet)
                        throw new UserFriendlyException(localizer.L("TipImportIndexMustBeFirst"));

                    foreach (ITipRow row in sheet.Rows)
                    {
                        Tip tip = await tipManager.GetTipByCodeAsync(row.GetString("Code"));

                        if (tip != null)
                        {
                            result.ElementsUpdated++;
                            //string newGroupCode = row.GetString("GroupCode");

                            //if (newGroupCode != cat.GroupCode)
                            //{
                            //    // If groupCode has changed, retreive a translation corresponding to the new group, or use the new groupcode as a placeholder
                            //    categoryManager.CategoriesTranslation
                            //        .Where(ct => ct.CoreId == cat.Id)
                            //        .ToList()
                            //        .ForEach(ct =>
                            //        {
                            //            ct.Group = categoryManager.CategoriesTranslation
                            //                .Where(cx => cx.Core.GroupCode == newGroupCode && cx.Language == ct.Language && cx.Group != cat.GroupCode && cx.Group != newGroupCode)
                            //                .FirstOrDefault()?.Group ?? newGroupCode;
                            //        });
                            //}
                        }
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

                    foreach (ITipRow row in sheet.Rows)
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

                        // Check if other entities in this language have the same groupcode but a different translation for group, in that case update it
                        //tipManager.CategoriesTranslation
                        //    .Where(ct => ct.Language == catTrans.Language && ct.Core.GroupCode == parentCat.GroupCode && ct.Group != catTrans.Group)
                        //    .ToList()
                        //    .ForEach(ct => ct.Group = catTrans.Group);

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
