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

using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils.CodeCompletion.Stores;
using KaVE.JetBrains.Annotations;

namespace KaVE.VS.FeedbackGenerator.UserControls.OptionPage.UsageModelOptions
{
    public interface IUsageModelsTableRow
    {
        [NotNull]
        CoReTypeName TypeName { get; set; }

        [CanBeNull]
        int? LoadedVersion { get; set; }

        [CanBeNull]
        int? NewestAvailableVersion { get; set; }

        bool IsInstallable { get; }
        bool IsUpdateable { get; }
        bool IsRemoveable { get; }

        void LoadModel();
        void RemoveModel();
    }

    public class UsageModelsTableRow : IUsageModelsTableRow
    {
        [CanBeNull]
        private readonly ILocalPBNRecommenderStore _localStore;

        [CanBeNull]
        private readonly IRemotePBNRecommenderStore _remoteStore;

        public CoReTypeName TypeName { get; set; }

        public int? LoadedVersion { get; set; }

        public int? NewestAvailableVersion { get; set; }

        public bool IsInstallable
        {
            get { return !_alreadyLoaded && LoadedVersion == null && NewestAvailableVersion != null; }
        }

        public bool IsUpdateable
        {
            get
            {
                return !_alreadyLoaded && LoadedVersion != null && NewestAvailableVersion != null &&
                       LoadedVersion < NewestAvailableVersion;
            }
        }

        public bool IsRemoveable
        {
            get { return !_alreadyRemoved && LoadedVersion != null; }
        }

        private bool _alreadyLoaded;

        private bool _alreadyRemoved;

        public UsageModelsTableRow([CanBeNull] ILocalPBNRecommenderStore localStore,
            [CanBeNull] IRemotePBNRecommenderStore remoteStore,
            [NotNull] CoReTypeName typeName,
            [CanBeNull] int? loadedVersion,
            [CanBeNull] int? newestAvailableVersion)
        {
            _localStore = localStore;
            _remoteStore = remoteStore;
            TypeName = typeName;
            LoadedVersion = loadedVersion;
            NewestAvailableVersion = newestAvailableVersion;
        }

        public void LoadModel()
        {
            _alreadyLoaded = true;
            if (_remoteStore != null)
            {
                _remoteStore.Load(TypeName);
            }
        }

        public void RemoveModel()
        {
            _alreadyRemoved = true;
            if (_localStore != null)
            {
                _localStore.Remove(TypeName);
            }
        }

        public override bool Equals(object obj)
        {
            var other = obj as UsageModelsTableRow;
            return other != null &&
                   Equals(TypeName, other.TypeName) &&
                   Equals(LoadedVersion, other.LoadedVersion) &&
                   Equals(NewestAvailableVersion, other.NewestAvailableVersion);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override string ToString()
        {
            var loadedVersionString = LoadedVersion != null ? LoadedVersion.ToString() : "-";
            var newestAvailableVersionString = NewestAvailableVersion != null ? NewestAvailableVersion.ToString() : "-";
            return "(" + TypeName + "," + loadedVersionString + "," + newestAvailableVersionString + ")";
        }
    }
}