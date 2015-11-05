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
using KaVE.Commons.Utils.CodeCompletion.Stores;
using KaVE.Commons.Utils.IO;
using KaVE.Commons.Utils.Json;

namespace KaVE.Commons.Utils.CodeCompletion.Impl.Stores.UsageModelSources
{
    public class FilePathUsageModelsSource : IUsageModelsSource
    {
        protected readonly IIoUtils IoUtils;
        protected readonly TypePathUtil TypePathUtil;
        private readonly Uri _myUri;

        public FilePathUsageModelsSource(IIoUtils ioUtils, TypePathUtil typePathUtil, Uri myUri)
        {
            TypePathUtil = typePathUtil;
            IoUtils = ioUtils;
            _myUri = myUri;
        }

        public IEnumerable<UsageModelDescriptor> GetUsageModels()
        {
            IEnumerable<UsageModelDescriptor> result;

            try
            {
                result = GetIndexFileContent().ParseJsonTo<IEnumerable<UsageModelDescriptor>>();
            }
            catch
            {
                result = null;
            }

            return result ?? new UsageModelDescriptor[0];
        }

        public void Load(UsageModelDescriptor model, string baseTargetDirectory)
        {
            if (_myUri.IsFile)
            {
                var modelPath = TypePathUtil.GetNestedFileName(_myUri.LocalPath, model.TypeName, model.Version, "zip");
                var targetPath = TypePathUtil.GetNestedFileName(
                    baseTargetDirectory,
                    model.TypeName,
                    model.Version,
                    "zip");

                if (!IoUtils.FileExists(modelPath))
                {
                    modelPath = TypePathUtil.GetNestedFileName(_myUri.LocalPath, model.TypeName, "zip");
                }

                IoUtils.CreateFile(targetPath);
                IoUtils.CopyFile(modelPath, targetPath);
            }
        }

        private string GetIndexFileContent()
        {
            try
            {
                return IoUtils.ReadFile(Path.Combine(Source.LocalPath, "index.json"));
            }
            catch
            {
                return "";
            }
        }
    }
}