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
using KaVE.JetBrains.Annotations;
using Smile;

namespace KaVE.Commons.Utils.CodeCompletion.Impl.Stores
{
    public class SmilePBNRecommenderStore : ILocalPBNRecommenderStore
    {
        public string BasePath { get; protected set; }

        public bool EnableAutoRemoteLoad = false;

        private readonly IIoUtils _io;
        private readonly TypePathUtil _typePathUtil;

        [NotNull]
        private IEnumerable<UsageModelDescriptor> _availableModels;

        private readonly IRemotePBNRecommenderStore _remoteStore;

        public SmilePBNRecommenderStore(string basePath,
            IIoUtils io,
            TypePathUtil typePathUtil,
            IRemotePBNRecommenderStore remoteStore)
        {
            _remoteStore = remoteStore;
            BasePath = basePath;
            _io = io;
            _typePathUtil = typePathUtil;

            _availableModels = new List<UsageModelDescriptor>();
            ReloadAvailableModels();
        }

        public bool IsAvailable(CoReTypeName type)
        {
            return GetAvailableModels().Any(model => model.TypeName.Equals(type));
        }

        public void Remove(CoReTypeName type)
        {
            var availableModel = GetAvailableModels().FirstOrDefault(model => model.TypeName.Equals(type));
            Asserts.NotNull(availableModel, "No model installed for {0}", type);

            _io.DeleteFile(GetNestedFileName(BasePath, type, availableModel.Version, "zip"));
        }

        public void RemoveAll()
        {
            foreach (var model in GetAvailableModels())
            {
                Remove(model.TypeName);
            }
        }

        public IPBNRecommender Load(CoReTypeName type)
        {
            var availableModel = GetAvailableModels().FirstOrDefault(model => model.TypeName.Equals(type));
            if (availableModel == null)
            {
                LoadFromRemote(type);
                return null;
            }

            var zipFileName = GetNestedFileName(BasePath, type, availableModel.Version, "zip");

            var tmpFolder = _io.UnzipToTempFolder(zipFileName);
            var xdslFileName = GetFlatFileName(tmpFolder, type, "xdsl");
            if (!_io.FileExists(xdslFileName))
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

        private void LoadFromRemote(CoReTypeName type)
        {
            if (EnableAutoRemoteLoad)
            {
                _remoteStore.Load(type);
            }
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
            return _availableModels;
        }

        public void ReloadAvailableModels()
        {
            string[] zipFiles;
            try
            {
                zipFiles = _io.GetFilesRecursive(BasePath, "*.zip");
            }
            catch
            {
                zipFiles = new string[0];
            }

            _availableModels =
                zipFiles.Select(
                    modelFilePath =>
                        new UsageModelDescriptor(
                            _typePathUtil.GetTypeNameString(modelFilePath.Replace(BasePath, "").TrimStart('\\')),
                            _typePathUtil.GetVersionNumber(modelFilePath.Replace(BasePath, "").TrimStart('\\'))));
        }
    }
}