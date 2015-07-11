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

namespace KaVE.Commons.Utils.Json
{
    public class SSTTypeNameShortener
    {
        public string RemoveDetails(string json)
        {
            var regex = new Regex("KaVE\\.Commons\\.Model\\.SSTs\\.Impl\\.([.a-zA-Z0-9_]+), KaVE\\.Commons");
            return regex.Replace(
                json,
                m =>
                {
                    var typeName = m.Groups[1].Value;
                    return string.Format("[SST:{0}]", typeName);
                });
        }

        public string AddDetails(string json)
        {
            var regex = new Regex("\\[SST:([.a-zA-Z0-9_]+)\\]");
            return regex.Replace(
                json,
                m =>
                {
                    var typeName = m.Groups[1].Value;
                    return string.Format("KaVE.Commons.Model.SSTs.Impl.{0}, KaVE.Commons", typeName);
                });
        }
    }
}