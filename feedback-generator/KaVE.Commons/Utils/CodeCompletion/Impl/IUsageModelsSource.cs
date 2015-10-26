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
using System.Windows;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.IO;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Utils.CodeCompletion.Impl
{
    public interface IUsageModelsSource
    {
        IEnumerable<UsageModelDescriptor> GetUsageModels();
        void LoadZip(CoReTypeName typeName);
    }

    public class UsageModelsSource : IUsageModelsSource
    {
        [CanBeNull]
        protected Uri Source;

        protected readonly IIoUtils IoUtils;

        public UsageModelsSource(IIoUtils ioUtils, Uri source)
        {
            IoUtils = ioUtils;
            Source = source;
        }

        public IEnumerable<UsageModelDescriptor> GetUsageModels()
        {
            if (Source == null)
            {
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
                                       GetTypeNameString(modelFilePath.Replace(localPath, "").TrimStart('\\')),
                                       GetVersionNumber(modelFilePath.Replace(localPath, "").TrimStart('\\'))));
            }
            
            // TODO handle remote hosted models

            return Lists.NewList<UsageModelDescriptor>();
        }

        public void LoadZip(CoReTypeName typeName)
        {
            // TODO implement this
            MessageBox.Show("Loading type " + typeName, "(Prototype)");
        }

        private static int GetVersionNumber(string relativeModelFilePath)
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

        private static CoReTypeName GetTypeNameString(string relativeModelFilePath)
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
    }
}