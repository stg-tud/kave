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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.CodeCompletion.Impl;
using KaVE.Commons.Utils.CodeCompletion.Stores;
using KaVE.Commons.Utils.IO;
using KaVE.Commons.Utils.Json;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Utils
{
    internal static class UsageModelsIndexFileGenerator
    {
        public static void Main(string[] args)
        {
            Console.Write("Base path: ");
            var basePath = Console.ReadLine();
            Asserts.NotNull(basePath);

            var indexFile = Path.Combine(basePath, "index.json.gz");
            var indexFileContent = GenerateIndexFileContent(basePath, new IoUtils(), new TypePathUtil());
            new IoUtils().WriteZippedFile(indexFileContent, indexFile);
        }

        [Pure, NotNull]
        public static string GenerateIndexFileContent([NotNull] string basePath,
            [NotNull] IIoUtils io,
            [NotNull] ITypePathUtil typePathUtil)
        {
            var descriptors = new List<UsageModelDescriptor>();
            foreach (var modelDescriptor in io.GetFilesRecursive(basePath, "*.zip")
                                              .Select(
                                                  modelFilePath =>
                                                      new UsageModelDescriptor(
                                                          typePathUtil.GetTypeName(
                                                              modelFilePath.Replace(basePath, "").TrimStart('\\')),
                                                          typePathUtil.GetVersionNumber(
                                                              modelFilePath.Replace(basePath, "").TrimStart('\\'))))
                )
            {
                descriptors.Add(modelDescriptor);
                Console.WriteLine("Found version {0} of {1}", modelDescriptor.Version, modelDescriptor.TypeName);
            }

            return descriptors.ToCompactJson();
        }
    }
}