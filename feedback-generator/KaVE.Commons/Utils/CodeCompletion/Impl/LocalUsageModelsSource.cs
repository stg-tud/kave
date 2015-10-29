using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.IO;

namespace KaVE.Commons.Utils.CodeCompletion.Impl
{
    public class LocalUsageModelsSource : IUsageModelsSource
    {
        public Uri Source { get; set; }

        protected readonly IIoUtils IoUtils;
        protected readonly TypePathUtil TypePathUtil;

        public LocalUsageModelsSource(IIoUtils ioUtils, TypePathUtil typePathUtil)
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