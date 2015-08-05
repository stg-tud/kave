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

using System.Text.RegularExpressions;
using KaVE.Commons.Utils.Json;

namespace KaVE.Commons.Utils.CodeCompletion.Impl
{
    public class TypePathUtil
    {
        public string ToNestedPath(object o)
        {
            var regex = new Regex(@"[^a-zA-Z0-9,\-_/+$(){}[\]]");

            var relName = o.ToCompactJson();
            relName = relName.Replace('.', '/');
            relName = relName.Replace("\\\"", "\""); // quotes inside json
            relName = relName.Replace("\"", ""); // surrounding quotes
            relName = relName.Replace('\\', '/');
            relName = regex.Replace(relName, "_");

            return relName;
        }

        public string ToFlatPath(object o)
        {
            var nested = ToNestedPath(o);
            var flat = nested.Replace("/", "_");
            return flat;
        }
    }
}