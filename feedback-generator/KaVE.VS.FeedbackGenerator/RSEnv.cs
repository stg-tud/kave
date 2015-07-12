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

using JetBrains.Application;
using JetBrains.Application.Components;
using JetBrains.UI.Resources;
using KaVE.Commons.Utils;
using KaVE.JetBrains.Annotations;
using KaVE.VS.FeedbackGenerator.VsIntegration;

namespace KaVE.VS.FeedbackGenerator
{
    public interface IRSEnv
    {
        IIDESession IDESession { get; }

        string KaVEVersion { get; }
    }

    [ShellComponent(ProgramConfigurations.VS_ADDIN)]
    internal class RSEnv : IRSEnv
    {
        public const string ExtensionId = "KaVE.VsFeedbackGenerator";

        private readonly OptionsThemedIcons.ExtensionManager _extensionManager;
        private readonly IIDESession _ideSession;
        private readonly VersionUtil _versionUtil;

        public RSEnv(OptionsThemedIcons.ExtensionManager extensionManager,
            IIDESession ideSession,
            VersionUtil versionUtil)
        {
            _extensionManager = extensionManager;
            _ideSession = ideSession;
            _versionUtil = versionUtil;
        }

        [NotNull]
        public IIDESession IDESession
        {
            get { return _ideSession; }
        }

        public string KaVEVersion
        {
            get { return _versionUtil.GetCurrentInformalVersion(); }
        }
    }
}