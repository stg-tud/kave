/*
 * Copyright 2014 Technische Universität Darmstadt
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections.Generic;
using System.Text;

namespace KaVE.Commons.Utils.Csv
{
    /// <summary>
    ///     Simple CSV export
    ///     Example:
    ///     CsvBuilder builder = new CsvBuilder();
    ///     builder.StartRow();
    ///     builder["Region"] = "New York, USA";
    ///     builder["Sales"] = 100000;
    ///     builder["Date Opened"] = new DateTime(2003, 12, 31);
    ///     builder.StartRow();
    ///     builder["Region"] = "Sydney \"in\" Australia";
    ///     builder["Sales"] = 50000;
    ///     builder["Date Opened"] = new DateTime(2005, 1, 1, 9, 30, 0);
    ///     var csv = builder.Build();
    /// </summary>
    public class CsvBuilder
    {
        private readonly List<string> _fields = new List<string>();
        private readonly List<Dictionary<string, object>> _rows = new List<Dictionary<string, object>>();

        public enum SortFields
        {
            ByInsertionOrder,
            ByName,
            ByNameLeaveFirst
        }

        public IList<string> Fields
        {
            get { return _fields; }
        }

        private Dictionary<string, object> CurrentRow
        {
            get { return _rows[_rows.Count - 1]; }
        }

        public object this[string field]
        {
            set
            {
                if (!_fields.Contains(field))
                {
                    _fields.Add(field);
                }
                CurrentRow[field] = value;
            }
        }

        public void StartRow()
        {
            _rows.Add(new Dictionary<string, object>());
        }

        public string Build(SortFields sortFields = SortFields.ByInsertionOrder)
        {
            var sb = new StringBuilder();
            Sort(sortFields);
            ExportHeader(sb);
            ExportRows(sb);
            return sb.ToString();
        }

        private void Sort(SortFields sortFields)
        {
            switch (sortFields)
            {
                case SortFields.ByName:
                    _fields.Sort();
                    break;
                case SortFields.ByNameLeaveFirst:
                    var first = _fields[0];
                    _fields.RemoveAt(0);
                    _fields.Sort();
                    _fields.Insert(0, first);
                    break;
            }
        }

        private void ExportHeader(StringBuilder sb)
        {
            var firstLine = true;
            foreach (var field in _fields)
            {
                if (!firstLine)
                {
                    sb.Append(",");
                }
                firstLine = false;
                sb.Append(field);
            }
            sb.AppendLine();
        }

        private void ExportRows(StringBuilder sb)
        {
            foreach (var row in _rows)
            {
                var firstField = true;
                foreach (var field in _fields)
                {
                    if (!firstField)
                    {
                        sb.Append(",");
                    }
                    firstField = false;
                    sb.Append(ToField(row, field));
                }
                sb.AppendLine();
            }
        }

        private static string ToField(IDictionary<string, object> row, string field)
        {
            object value;
            row.TryGetValue(field, out value);
            return QuoteIfRequired(ToString(value));
        }

        private static string ToString(object value)
        {
            string output;
            if (value == null)
            {
                output = "";
            }
            else if (value is System.DateTime)
            {
                var dateTime = (System.DateTime) value;
                output = dateTime.ToString(dateTime == dateTime.Date ? "yyyy-MM-dd" : "yyyy-MM-dd HH:mm:ss");
            }
            else
            {
                output = value.ToString();
            }
            return output;
        }

        private static string QuoteIfRequired(string value)
        {
            if (value.Contains(",") || value.Contains("\""))
            {
                value = '"' + value.Replace("\"", "\"\"") + '"';
            }
            return value;
        }
    }
}