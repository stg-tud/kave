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

using System;
using System.IO;
using System.Text.RegularExpressions;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Json;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Utils.CodeCompletion.Impl
{
    public class TypePathUtil
    {
        [Pure]
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

        [Pure]
        public string ToFlatPath(object o)
        {
            var nested = ToNestedPath(o);
            var flat = nested.Replace("/", "_");
            return flat;
        }

        [Pure]
        public int GetVersionNumber(string relativeModelFilePath)
        {
            try
            {
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(relativeModelFilePath);
                Asserts.NotNull(fileNameWithoutExtension);
                var versionSubstring =
                    fileNameWithoutExtension.Substring(fileNameWithoutExtension.LastIndexOf('.') + 1);
                return int.Parse(versionSubstring);
            }
            catch
            {
                return 0;
            }
        }

        [Pure]
        public CoReTypeName GetTypeNameString(string relativeModelFilePath)
        {
            var filePathWithoutExtension = relativeModelFilePath.Substring(0, relativeModelFilePath.LastIndexOf('.'));
            Asserts.NotNull(filePathWithoutExtension);
            var typeNameSubstring = "";

            try
            {
                typeNameSubstring = filePathWithoutExtension.Substring(0, filePathWithoutExtension.LastIndexOf('.'));
            }
            catch (ArgumentOutOfRangeException)
            {
                typeNameSubstring = filePathWithoutExtension;
            }

            return new CoReTypeName(typeNameSubstring);
        }

        [Pure]
        public string GetNestedFileName(string basePath, CoReTypeName typeName, int version, string extension)
        {
            var typePart = ToNestedPath(typeName).Replace("//", @"\");
            var fileName = Path.Combine(basePath, typePart + '.' + version + '.' + extension);
            return fileName;
        }

        [Pure]
        public string GetNestedFileName(string basePath, CoReTypeName typeName, string extension)
        {
            var typePart = ToNestedPath(typeName).Replace("//", @"\");
            var fileName = Path.Combine(basePath, typePart + '.' + extension);
            return fileName;
        }
    }
}