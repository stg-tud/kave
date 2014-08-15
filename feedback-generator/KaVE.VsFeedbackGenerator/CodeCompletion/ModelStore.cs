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
 * 
 * Contributors:
 *    - Dennis Albrecht
 */

using System;
using System.IO;
using Ionic.Zip;
using KaVE.Model.ObjectUsage;
using KaVE.VsFeedbackGenerator.Generators;
using KaVE.VsFeedbackGenerator.Utils;

namespace KaVE.VsFeedbackGenerator.CodeCompletion
{
    public interface IModelStore
    {
        UsageModel GetModel(CoReTypeName typeName);
        UsageModel GetModel(string assembly, CoReTypeName typeName);
        UsageModel GetModel(string assembly, CoReTypeName typeName, bool forceReload);
    }

    public class ModelStore : IModelStore
    {
        private readonly string _basePath;
        private readonly IIoUtils _utils;
        private readonly ILogger _logger;

        public ModelStore(string basePath, IIoUtils utils, ILogger logger)
        {
            _basePath = basePath;
            _utils = utils;
            _logger = logger;
        }

        public UsageModel GetModel(CoReTypeName typeName)
        {
            return GetModel(GetFileName(typeName), typeName);
        }

        public UsageModel GetModel(string assembly, CoReTypeName typeName)
        {
            return GetModel(assembly, typeName, false);
        }

        public UsageModel GetModel(string assembly, CoReTypeName typeName, bool forceReload)
        {
            var fileName = GetFileName(typeName) + ".xdsl";
            var path = _utils.Combine(_basePath, assembly, fileName);
            if (!forceReload)
            {
                var model = GetModel(path);
                if (model != null)
                {
                    return model;
                }
            }
            var zipPath = _utils.Combine(_basePath, assembly + ".zip");
            if (!_utils.FileExists(zipPath))
            {
                return null;
            }
            ExtractModel(zipPath, path, fileName);
            return GetModel(path);
        }

        public static string GetFileName(CoReTypeName type)
        {
            return type.Name.Replace('/', '_');
        }

        private UsageModel GetModel(string path)
        {
            if (!_utils.FileExists(path))
            {
                return null;
            }
            return new UsageModel(_utils.LoadNetwork(path));
        }

        private void ExtractModel(string zipPath, string path, string fileName)
        {
            try
            {
                var zip = ZipFile.Read(_utils.OpenFile(zipPath, FileMode.Open, FileAccess.Read));
                var entry = zip[fileName];
                if (entry == null)
                {
                    return;
                }
                using (var stream = new MemoryStream())
                {
                    entry.Extract(stream);
                    _utils.CreateDirectory(_utils.GetDirectoryName(path));
                    _utils.WriteAllByte(stream.ToArray(), path);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
        }
    }
}