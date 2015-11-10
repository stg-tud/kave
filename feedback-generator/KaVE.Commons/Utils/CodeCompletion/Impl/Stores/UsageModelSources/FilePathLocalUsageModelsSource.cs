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
using KaVE.Commons.Utils.CodeCompletion.Stores;
using KaVE.Commons.Utils.IO;
using KaVE.Commons.Utils.Json;
using KaVE.JetBrains.Annotations;
using Smile;

namespace KaVE.Commons.Utils.CodeCompletion.Impl.Stores.UsageModelSources
{
    public class FilePathLocalUsageModelsSource : ILocalUsageModelsSource
    {
        [NotNull]
        private readonly IIoUtils _ioUtils;

        [NotNull]
        private readonly ITypePathUtil _typePathUtil;

        [NotNull]
        public string SourcePath { get; private set; }

        private string IndexFile
        {
            get { return Path.Combine(SourcePath, "index.json.gz"); }
        }

        public FilePathLocalUsageModelsSource([NotNull] string sourcePath,
            [NotNull] IIoUtils ioUtils,
            [NotNull] ITypePathUtil typePathUtil)
        {
            _ioUtils = ioUtils;
            _typePathUtil = typePathUtil;
            SourcePath = sourcePath;
        }

        [Pure]
        public IEnumerable<UsageModelDescriptor> GetUsageModels()
        {
            IEnumerable<UsageModelDescriptor> result;

            try
            {
                result = GetIndexFileContent().ParseJsonTo<IEnumerable<UsageModelDescriptor>>();
            }
            catch
            {
                result = new UsageModelDescriptor[0];
            }

            return result ?? new UsageModelDescriptor[0];
        }

        [Pure]
        private string GetIndexFileContent()
        {
            try
            {
                return _ioUtils.ReadZippedFile(IndexFile);
            }
            catch
            {
                return "";
            }
        }

        [Pure]
        public IPBNRecommender Load(CoReTypeName type)
        {
            var availableModel = GetUsageModels().FirstOrDefault(model => model.TypeName.Equals(type));
            if (availableModel == null)
            {
                return null;
            }

            var zipFileName = _typePathUtil.GetNestedFileName(SourcePath, type, availableModel.Version, "zip");
            if (!_ioUtils.FileExists(zipFileName))
            {
                return null;
            }

            var tmpFolder = _ioUtils.UnzipToTempFolder(zipFileName);
            var xdslFileName = GetFlatFileName(tmpFolder, type, "xdsl");
            if (!_ioUtils.FileExists(xdslFileName))
            {
                return null;
            }

            try
            {
                return new SmilePBNRecommender(type, ReadNetwork(xdslFileName));
            }
            catch (AssertException)
            {
                return null;
            }
        }

        public void Remove(CoReTypeName type)
        {
            var availableModels = GetUsageModels().ToList();
            var modelForType = availableModels.FirstOrDefault(model => model.TypeName.Equals(type));
            if (modelForType == null)
            {
                return;
            }

            availableModels.Remove(modelForType);
            _ioUtils.WriteZippedFile(availableModels.ToCompactJson(), IndexFile);

            var modelZip = _typePathUtil.GetNestedFileName(SourcePath, type, modelForType.Version, "zip");
            if (_ioUtils.FileExists(modelZip))
            {
                _ioUtils.DeleteFile(modelZip);
            }
        }

        private string GetFlatFileName(string basePath, CoReTypeName typeName, string extension)
        {
            var typePart = _typePathUtil.ToFlatPath(typeName);
            var fileName = Path.Combine(basePath, typePart + '.' + extension);
            return fileName;
        }

        [Pure]
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
    }
}