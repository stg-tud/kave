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
 *    - Sven Amann
 */

using System;
using EnvDTE;
using JetBrains.Application;
using JetBrains.Application.Components;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Assertion;
using KaVE.ReSharper.Commons.Utils;
using KaVE.VsFeedbackGenerator.MessageBus;

namespace KaVE.VsFeedbackGenerator.Generators.VisualStudio
{
    [ShellComponent(ProgramConfigurations.VS_ADDIN)]
    internal class BuildEventGenerator : EventGeneratorBase
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly BuildEvents _buildEvents;
        private BuildEvent _currentEvent;
        private BuildTarget _currentTarget;

        public BuildEventGenerator(IRSEnv env, IMessageBus messageBus, IDateUtils dateUtils)
            : base(env, messageBus, dateUtils)
        {
            _buildEvents = DTE.Events.BuildEvents;
            _buildEvents.OnBuildBegin += _buildEvents_OnBuildBegin;
            _buildEvents.OnBuildDone += _buildEvents_OnBuildDone;
            _buildEvents.OnBuildProjConfigBegin += _buildEvents_OnBuildProjConfigBegin;
            _buildEvents.OnBuildProjConfigDone += _buildEvents_OnBuildProjConfigDone;
        }

        /// <summary>
        ///     Invoked when a user starts a build (clean, rebuild, ...) of a solution, project, or batch build.
        /// </summary>
        private void _buildEvents_OnBuildBegin(vsBuildScope scope, vsBuildAction action)
        {
            Asserts.Null(_currentEvent, "another build is running.");
            _currentEvent = Create<BuildEvent>();
            _currentEvent.Scope = scope.ToString();
            _currentEvent.Action = action.ToString();
        }

        /// <summary>
        ///     Called for each combination of project and configuration that is part of the build.
        /// </summary>
        private void _buildEvents_OnBuildProjConfigBegin(string project,
            string projectConfig,
            string platform,
            string solutionConfig)
        {
            Asserts.Null(_currentTarget, "another build target is currently being processed.");
            _currentTarget = new BuildTarget
            {
                Project = project,
                ProjectConfiguration = projectConfig,
                Platform = platform,
                SolutionConfiguration = solutionConfig,
                StartedAt = DateTime.Now
            };
        }

        private void _buildEvents_OnBuildProjConfigDone(string project,
            string projectConfig,
            string platform,
            string solutionConfig,
            bool success)
        {
            Asserts.NotNull(_currentTarget, "no build-target processing has been started.");
            _currentTarget.Duration = DateTime.Now - _currentTarget.StartedAt;
            _currentTarget.Successful = success;
            _currentEvent.Targets.Add(_currentTarget);
            _currentTarget = null;
        }

        private void _buildEvents_OnBuildDone(vsBuildScope scope, vsBuildAction action)
        {
            Asserts.NotNull(_currentEvent, "no build processing has been started");
            FireNow(_currentEvent);
            _currentEvent = null;
        }
    }
}