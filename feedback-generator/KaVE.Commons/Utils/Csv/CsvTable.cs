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
using System.Linq;
using System.Text.RegularExpressions;

namespace KaVE.Commons.Utils.Csv
{
    public class CsvTable
    {

        public static CsvTable Read(string csv, char delimiter = ',')
        {
            return new CsvTable(SplitLines(RemoveTrailingNewline(csv)), delimiter);
        }

        private static string RemoveTrailingNewline(string csv)
        {
            return csv.Trim();
        }

        private static string[] SplitLines(string removeTrailingNewline)
        {
            return Regex.Split(removeTrailingNewline, "\r\n|\r|\n");
        }

        private static string[] SplitFields(string row, char delimiter)
        {
            return row.Split(delimiter);
        }

        private readonly string[] _fields;
        private readonly string[] _rows;
        private readonly char _delimiter;

        private CsvTable(IList<string> lines, char delimiter)
        {
            _delimiter = delimiter;
            _fields = SplitFields(lines.First(), _delimiter);
            _rows = lines.Skip(1).ToArray();
        }

        public IList<CsvRow> Rows
        {
            get
            {
                return _rows.Select(row => new CsvRow(_fields, SplitFields(row, _delimiter))).ToList();
            }
        }
    }

    public class CsvRow
    {
        private readonly IDictionary<string, string> _row = new Dictionary<string, string>(); 

        public CsvRow(IList<string> fields, IList<string> values)
        {
            for (var i = 0; i < fields.Count; i++)
            {
                _row[fields[i]] = values[i];
            }
        }

        public string this[string fieldName]
        {
            get { return _row[fieldName]; }
        }
    }
}
