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
using JetBrains.Util;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.CodeCompletion.Impl;
using KaVE.JetBrains.Annotations;
using MsgBox.Commands;

namespace KaVE.VS.FeedbackGenerator.UserControls.OptionPage.UsageModelOptions
{
    public class UsageModelsTableRow
    {
        [CanBeNull]
        private readonly IRemotePBNRecommenderStore _remoteStore;

        [NotNull]
        public CoReTypeName TypeName { get; set; }

        [CanBeNull]
        public int? LoadedVersion { get; set; }

        [CanBeNull]
        public int? NewestAvailableVersion { get; set; }

        public bool IsUpdateable
        {
            get
            {
                return LoadedVersion != null && NewestAvailableVersion != null && LoadedVersion < NewestAvailableVersion;
            }
        }

        public bool IsInstallable
        {
            get { return LoadedVersion == null && NewestAvailableVersion != null; }
        }

        public bool IsRemoveable
        {
            get { return LoadedVersion != null; }
        }

        public ICommand OnInstall
        {
            get { return new RelayCommand(InstallModel); }
        }

        public ICommand OnUpdate
        {
            get { return new RelayCommand(UpdateModel); }
        }

        public ICommand OnRemove
        {
            get { return new RelayCommand(RemoveModel); }
        }

        public UsageModelsTableRow([CanBeNull] IRemotePBNRecommenderStore remoteStore,
            [NotNull] CoReTypeName typeName,
            [CanBeNull] int? loadedVersion,
            [CanBeNull] int? newestAvailableVersion)
        {
            _remoteStore = remoteStore;
            TypeName = typeName;
            LoadedVersion = loadedVersion;
            NewestAvailableVersion = newestAvailableVersion;
        }

        private void InstallModel()
        {
            if (_remoteStore != null)
            {
                _remoteStore.Load(TypeName);
                _remoteStore.ReloadAvailableModels();
                MessageBox.ShowInfo("Installed model for " + TypeName, "KaVE Project");
            }
        }

        private void UpdateModel()
        {
            if (_remoteStore != null)
            {
                _remoteStore.Load(TypeName);
                _remoteStore.ReloadAvailableModels();
            }
            MessageBox.ShowInfo("Updated model for " + TypeName, "KaVE Project");
        }

        private void RemoveModel()
        {
            // TODO implement this
            MessageBox.ShowInfo("Removing " + TypeName, "(Prototype)");
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