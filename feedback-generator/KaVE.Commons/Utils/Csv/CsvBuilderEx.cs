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

namespace KaVE.Commons.Utils.Csv
{
    public static class CsvBuilderEx
    {
        public static string ToCsv<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> dictionary)
        {
            var builder = new CsvBuilder();
            foreach (var keyValuePair in dictionary)
            {
                builder.StartRow();
                builder["key"] = keyValuePair.Key;
                builder["value"] = keyValuePair.Value;
            }
            return builder.Build();
        }
    }
}