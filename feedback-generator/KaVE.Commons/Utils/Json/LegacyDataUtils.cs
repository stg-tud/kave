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

using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace KaVE.Commons.Utils.Json
{
    internal static class LegacyDataUtils
    {
        public static string UpdateLegacyDataFormats(string json, JsonSerializerSettings settings)
        {
            ConfigureConversionOfNonEnumerableProposalCollections(settings);

            json = ChangeCompletionEventNamespace(json);
            json = MoveModelToCommons(json);
            json = MoveVariableDeclarationFromDeclarationsToStatements(json);
            json = ShortenSSTTypeNames(json);
            json = ConvertJaggedArraysWithMultidimensionalArrays(json);
            return json;
        }

        private static void ConfigureConversionOfNonEnumerableProposalCollections(JsonSerializerSettings settings)
        {
            settings.Converters.Add(new ProposalCollectionConverter());
        }

        private static string ChangeCompletionEventNamespace(string json)
        {
            if (json.StartsWith("{\"$type\":\"KaVE.Model.Events.CompletionEvent.CompletionEvent, KaVE.Model\""))
            {
                return json.Replace("KaVE.Model.Events.CompletionEvent.", "KaVE.Model.Events.CompletionEvents.");
            }
            return json;
        }

        private static string MoveModelToCommons(string json)
        {
            return json.Replace("KaVE.Model.", "KaVE.Commons.Model.").Replace("KaVE.Model", "KaVE.Commons");
        }

        private static string MoveVariableDeclarationFromDeclarationsToStatements(string json)
        {
            return json.Replace(
                "KaVE.Commons.Model.SSTs.Impl.Declarations.VariableDeclaration",
                "KaVE.Commons.Model.SSTs.Impl.Statements.VariableDeclaration");
        }

        private static string ShortenSSTTypeNames(string json)
        {
            // TODO discuss this replacement (seb)
            return new SSTTypeNameShortener().AddDetails(json);
        }

        private static string ConvertJaggedArraysWithMultidimensionalArrays(string json)
        {
            var match = Regex.Match(json, "((?:\\[\\]){2,})");
            if (match.Success)
            {
                var oldMarker = match.Groups[0];
                var newJson = new StringBuilder();
                newJson.Append(json.Substring(0, oldMarker.Index));
                newJson.Append("[");
                newJson.Append(new string(',', oldMarker.Length/2 - 1));
                newJson.Append("]");
                newJson.Append(json.Substring(oldMarker.Index + oldMarker.Length));
                return newJson.ToString();
            }
            return json;
        }
    }
}