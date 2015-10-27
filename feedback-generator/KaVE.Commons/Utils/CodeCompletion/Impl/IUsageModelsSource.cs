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
using System.Linq;
using System.Windows;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.IO;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Utils.CodeCompletion.Impl
{
    public interface IUsageModelsSource
    {
        [CanBeNull]
        Uri Source { get; set; }

        [Pure, NotNull]
        IEnumerable<UsageModelDescriptor> GetUsageModels();

        void Load(UsageModelDescriptor model, string baseTargetDirectory);
    }

    public class UsageModelsSource : IUsageModelsSource
    {
        public Uri Source { get; set; }

        protected readonly IIoUtils IoUtils;
        protected readonly TypePathUtil TypePathUtil;

        public UsageModelsSource(IIoUtils ioUtils, TypePathUtil typePathUtil)
        {
            TypePathUtil = typePathUtil;
            IoUtils = ioUtils;
        }

        public IEnumerable<UsageModelDescriptor> GetUsageModels()
        {
            if (Source == null)
            {
                MessageBox.Show("Source was null");
                return Lists.NewList<UsageModelDescriptor>();
            }

            var localPath = Source.LocalPath;
            if (localPath != "/")
            {
                return
                    IoUtils.GetFilesRecursive(localPath, "*.zip")
                           .Select(
                               modelFilePath =>
                                   new UsageModelDescriptor(
                                       TypePathUtil.GetTypeNameString(
                                           modelFilePath.Replace(localPath, "").TrimStart('\\')),
                                       TypePathUtil.GetVersionNumber(
                                           modelFilePath.Replace(localPath, "").TrimStart('\\'))));
            }

            // TODO handle remote hosted models

            return Lists.NewList<UsageModelDescriptor>();
        }

        public void Load(UsageModelDescriptor model, string baseTargetDirectory)
        {
            Asserts.NotNull(Source);

            var localPath = Source.LocalPath;
            if (localPath != "/")
            {
                var modelPath = TypePathUtil.GetNestedFileName(localPath, model.TypeName, model.Version, "zip");
                var targetPath = TypePathUtil.GetNestedFileName(
                    baseTargetDirectory,
                    model.TypeName,
                    model.Version,
                    "zip");

                if (!IoUtils.FileExists(modelPath))
                    modelPath = TypePathUtil.GetNestedFileName(localPath, model.TypeName, "zip");

                IoUtils.CreateFile(targetPath);
                IoUtils.CopyFile(modelPath, targetPath);
            }

            // TODO handle remote hosted models
        }
    }
}