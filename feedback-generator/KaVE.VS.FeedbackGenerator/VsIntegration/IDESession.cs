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
using KaVE.JetBrains.Annotations;

namespace KaVE.VS.FeedbackGenerator.VsIntegration
{
    [ShellComponent(ProgramConfigurations.VS_ADDIN)]
    public class IDESession : IIDESession
    {
        private readonly DTE _dte;
        private readonly string _sessionUUID;

        public IDESession([NotNull] DTE dte)
        {
            _dte = dte;
            _sessionUUID = Guid.NewGuid().ToString();
        }

        public string UUID
        {
            get { return _sessionUUID; }
        }

        public DTE DTE
        {
            get { return _dte; }
        }
    }
}