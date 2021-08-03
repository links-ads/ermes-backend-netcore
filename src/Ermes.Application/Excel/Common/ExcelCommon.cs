using Abp.UI;
using Ermes.Helpers;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ermes.Excel.Common
{
    public class ExcelCommon
    {
        public interface IMultilanguageTable
        {
            public IEnumerable<IErmesSheet> Sheets { get; }
        }

        public interface IErmesSheet
        {
            public string Language { get; }
            public IEnumerable<IErmesRow> Rows { get; }
        }

        public interface IErmesRow
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
            bool GetBoolean(string columnName)
            {
                string value = GetString(columnName);
                if (!Boolean.TryParse(value, out bool result))
                    return value == "1";
                else
                    return result;
            }
        }

        #region Excel
        const int numberOfHeaderRows = 1;
        const int excelIndexingStart = 1;
        public class ExcelMultilanguageTable : IMultilanguageTable
        {
            private ExcelWorksheets _excelWorksheets;
            public ExcelMultilanguageTable(ExcelWorksheets excelWorksheets)
            {
                _excelWorksheets = excelWorksheets;
            }
            public IEnumerable<IErmesSheet> Sheets { get { return _excelWorksheets.Select(w => new ExcelTipSheet(w)); } }
        }

        private class ExcelTipSheet : IErmesSheet
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
            public IEnumerable<IErmesRow> Rows
            {
                get
                {
                    for (int i = 0; !string.IsNullOrWhiteSpace(_excelWorksheet.Cells[i + numberOfHeaderRows + excelIndexingStart, excelIndexingStart].Text); i++)
                        yield return new ExcelTipRow(this, i);
                }
            }
        }

        private class ExcelTipRow : IErmesRow
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
    }
}
