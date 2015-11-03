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
using KaVE.Commons.Utils.CodeCompletion.Stores;
using KaVE.Commons.Utils.IO;

namespace KaVE.Commons.Utils.CodeCompletion.Impl.Stores.UsageModelSources
{
    public class FilePathUsageModelsSource : UsageModelsSourceBase
    {
        protected readonly IIoUtils IoUtils;
        protected readonly TypePathUtil TypePathUtil;

        public FilePathUsageModelsSource(IIoUtils ioUtils, TypePathUtil typePathUtil, Uri source)
            : base(source)
        {
            TypePathUtil = typePathUtil;
            IoUtils = ioUtils;
        }

        public override void Load(UsageModelDescriptor model, string baseTargetDirectory)
        {
            if (Source.IsFile)
            {
                var modelPath = TypePathUtil.GetNestedFileName(Source.LocalPath, model.TypeName, model.Version, "zip");
                var targetPath = TypePathUtil.GetNestedFileName(
                    baseTargetDirectory,
                    model.TypeName,
                    model.Version,
                    "zip");

                if (!IoUtils.FileExists(modelPath))
                {
                    modelPath = TypePathUtil.GetNestedFileName(Source.LocalPath, model.TypeName, "zip");
                }

                IoUtils.CreateFile(targetPath);
                IoUtils.CopyFile(modelPath, targetPath);
            }
        }

        protected override string GetIndexContent()
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