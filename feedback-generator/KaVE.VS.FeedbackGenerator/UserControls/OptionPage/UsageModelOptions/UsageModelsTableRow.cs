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

using System.Windows.Input;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.CodeCompletion;
using KaVE.Commons.Utils.CodeCompletion.Stores;
using KaVE.JetBrains.Annotations;
using MsgBox.Commands;

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

        [NotNull]
        ICommand InstallModel { get; }

        [NotNull]
        ICommand UpdateModel { get; }

        [NotNull]
        ICommand RemoveModel { get; }

        void Install();
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

        public ICommand InstallModel
        {
            get { return new RelayCommand(Install, IsInstallable); }
        }

        public ICommand UpdateModel
        {
            get { return new RelayCommand(Update, IsUpdateable); }
        }

        public ICommand RemoveModel
        {
            get { return new RelayCommand(Remove, IsRemoveable); }
        }

        private bool _alreadyRemoved;
        private bool _alreadyInstalled;
        private bool _alreadyUpdated;

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

        public void Install()
        {
            _alreadyInstalled = true;
            if (_remoteStore != null)
            {
                _remoteStore.Load(TypeName);
            }
        }

        private void Update()
        {
            _alreadyUpdated = true;
            if (_remoteStore != null)
            {
                _remoteStore.Load(TypeName);
            }
        }

        private void Remove()
        {
            _alreadyRemoved = true;
            if (_localStore != null)
            {
                _localStore.Remove(TypeName);
            }
        }

        private bool IsInstallable()
        {
            return !_alreadyInstalled && LoadedVersion == null && NewestAvailableVersion != null;
        }

        public bool IsUpdateable()
        {
            return !_alreadyUpdated && LoadedVersion != null && NewestAvailableVersion != null && LoadedVersion < NewestAvailableVersion;
        }

        private bool IsRemoveable()
        {
            return !_alreadyRemoved && LoadedVersion != null;
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
            return this.ToStringReflection();
        }
    }
}