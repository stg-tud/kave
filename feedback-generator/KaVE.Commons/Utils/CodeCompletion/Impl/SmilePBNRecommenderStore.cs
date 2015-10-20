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
using System.IO;
using System.Linq;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.IO;
using Smile;

namespace KaVE.Commons.Utils.CodeCompletion.Impl
{
    public class SmilePBNRecommenderStore : IPBNRecommenderStore
    {
        public string BasePath { get; protected set; }

        private readonly IIoUtils _io;
        private readonly TypePathUtil _typePathUtil;

        public SmilePBNRecommenderStore(string basePath, IIoUtils io, TypePathUtil typePathUtil)
        {
            BasePath = basePath;
            _io = io;
            _typePathUtil = typePathUtil;
        }

        public bool IsAvailable(CoReTypeName type)
        {
            return GetAvailableModels().Any(model => model.TypeName.Equals(type));
        }

        public IPBNRecommender Load(CoReTypeName type)
        {
            var availableModel = GetAvailableModels().FirstOrDefault(model => model.TypeName.Equals(type));
            Asserts.NotNull(availableModel, "Model for {0} is not available", type);

            var zipFileName = GetNestedFileName(BasePath, type, availableModel.Version, "zip");

            var tmpFolder = _io.UnzipToTempFolder(zipFileName);
            var xdslFileName = GetFlatFileName(tmpFolder, type, "xdsl");
            Asserts.That(_io.FileExists(xdslFileName), "Couldn't find xdsl file for {0}", type);

            var network = ReadNetwork(xdslFileName);
            return new SmilePBNRecommender(type, network);
        }

        private static Network ReadNetwork(string xdslFileName)
        {
            try
            {
                var network = new Network();
                network.ReadFile(xdslFileName);
                return network;
            }
            catch (SmileException e)
            {
                throw new AssertException("error reading the network", e);
            }
        }

        private string GetNestedFileName(string basePath, CoReTypeName typeName, int version, string extension)
        {
            var typePart = _typePathUtil.ToNestedPath(typeName);
            var fileName = Path.Combine(basePath, typePart + '.' + version + '.' + extension);
            return fileName;
        }

        private string GetFlatFileName(string basePath, CoReTypeName typeName, string extension)
        {
            var typePart = _typePathUtil.ToFlatPath(typeName);
            var fileName = Path.Combine(basePath, typePart + '.' + extension);
            return fileName;
        }

        public IEnumerable<UsageModelDescriptor> GetAvailableModels()
        {
            var availableModels = new Dictionary<CoReTypeName, UsageModelDescriptor>();

            string[] zipFiles;
            try
            {
                zipFiles = _io.GetFilesRecursive(BasePath, "*.zip");
            }
            catch
            {
                zipFiles = new string[0];
            }

            foreach (var newModel in zipFiles
                                        .Select(
                                            modelFilePath =>
                                                new UsageModelDescriptor(
                                                    GetTypeNameString(modelFilePath.Replace(BasePath, "").TrimStart('\\')),
                                                    GetVersionNumber(modelFilePath.Replace(BasePath, "").TrimStart('\\'))))
                                        .Where(
                                            newModel =>
                                                !availableModels.ContainsKey(newModel.TypeName) ||
                                                newModel.Version > availableModels[newModel.TypeName].Version))
            {
                availableModels[newModel.TypeName] = newModel;
            }

            return availableModels.Values;
        }

        private static int GetVersionNumber(string relativeModelFilePath)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(relativeModelFilePath);
            Asserts.NotNull(fileNameWithoutExtension);
            var versionSubstring =
                fileNameWithoutExtension.Substring(fileNameWithoutExtension.LastIndexOf('.') + 1);
            return int.Parse(versionSubstring);
        }

        private static CoReTypeName GetTypeNameString(string relativeModelFilePath)
        {
            var filePathWithoutExtension = relativeModelFilePath.Substring(0, relativeModelFilePath.LastIndexOf('.'));
            Asserts.NotNull(filePathWithoutExtension);
            var typeNameSubstring = filePathWithoutExtension.Substring(0, filePathWithoutExtension.LastIndexOf('.'));
            return new CoReTypeName(typeNameSubstring);
        }
    }
}