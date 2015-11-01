using System;
using System.Collections.Generic;
using System.Linq;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.CodeCompletion.Stores;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.IO;

namespace KaVE.Commons.Utils.CodeCompletion.Impl.Stores
{
    public class FilePathUsageModelsSource : IUsageModelsSource
    {
        public Uri Source { get; set; }

        protected readonly IIoUtils IoUtils;
        protected readonly TypePathUtil TypePathUtil;

        public FilePathUsageModelsSource(IIoUtils ioUtils, TypePathUtil typePathUtil)
        {
            TypePathUtil = typePathUtil;
            IoUtils = ioUtils;
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
                string[] modelFiles;

                try
                {
                    modelFiles = IoUtils.GetFilesRecursive(localPath, "*.zip");
                }
                catch
                {
                    modelFiles = new string[0];
                }

                return
                    modelFiles
                        .Select(
                            modelFilePath =>
                                new UsageModelDescriptor(
                                    TypePathUtil.GetTypeNameString(
                                        modelFilePath.Replace(localPath, "").TrimStart('\\')),
                                    TypePathUtil.GetVersionNumber(
                                        modelFilePath.Replace(localPath, "").TrimStart('\\'))));
            }

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
                {
                    modelPath = TypePathUtil.GetNestedFileName(localPath, model.TypeName, "zip");
                }

                IoUtils.CreateFile(targetPath);
                IoUtils.CopyFile(modelPath, targetPath);
            }

            // TODO handle remote hosted models
        }
    }
}