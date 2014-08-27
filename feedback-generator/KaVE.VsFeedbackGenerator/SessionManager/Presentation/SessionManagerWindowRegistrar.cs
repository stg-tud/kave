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
 *    - Uli Fahrer
 */

using JetBrains.ActionManagement;
using JetBrains.Application;
using JetBrains.DataFlow;
using JetBrains.UI.CrossFramework;
using JetBrains.UI.ToolWindowManagement;
using KaVE.JetBrains.Annotations;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.Utils.Logging;

namespace KaVE.VsFeedbackGenerator.SessionManager.Presentation
{
    [ShellComponent]
    public class SessionManagerWindowRegistrar
    {
        [UsedImplicitly]
        private readonly Lifetime _lifetime;

        [UsedImplicitly]
        private readonly ToolWindowClass _toolWindowClass;

        public SessionManagerWindowRegistrar(Lifetime lifetime,
            ToolWindowManager toolWindowManager,
            SessionManagerWindowDescriptor descriptor,
            IActionManager actionManager,
            ILogManager logManager,
            ISettingsStore settingsStore,
            IDateUtils dateUtils)
        {
            // objects are kept in fields to prevent garbage collection
            _lifetime = lifetime;
            _toolWindowClass = toolWindowManager.Classes[descriptor];
            _toolWindowClass.RegisterEmptyContent(
                lifetime,
                lt =>
                {
                    var visibilitySignal = _toolWindowClass.Visible.Change;
                    var control = new SessionManagerControl(
                        new FeedbackViewModel(logManager),
                        actionManager,
                        dateUtils,
                        settingsStore);
                    visibilitySignal.Advise(lt, control.OnVisibilityChanged);
                    var wrapper = new EitherControl(control);
                    return wrapper.BindToLifetime(lt);
                });
        }
    }
}