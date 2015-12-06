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

using System.Windows.Forms;
using Gma.System.MouseKeyHook;
using JetBrains.Application;

namespace KaVE.VS.FeedbackGenerator.Generators.Activity
{
    // ReSharper disable once InconsistentNaming
    public interface IKaVEMouseEvents
    {
        event MouseEventHandler MouseMove;
        event MouseEventHandler MouseClick;
        event MouseEventHandler MouseWheel;
    }

    [ShellComponent]
    internal class KaVEMouseEvents : IKaVEMouseEvents
    {
        private readonly IKeyboardMouseEvents _mouseEvents = Hook.GlobalEvents();

        public event MouseEventHandler MouseMove
        {
            add { _mouseEvents.MouseMove += value; }
            remove { _mouseEvents.MouseMove -= value; }
        }

        public event MouseEventHandler MouseClick
        {
            add { _mouseEvents.MouseClick += value; }
            remove { _mouseEvents.MouseClick -= value; }
        }

        public event MouseEventHandler MouseWheel
        {
            add { _mouseEvents.MouseWheel += value; }
            remove { _mouseEvents.MouseWheel -= value; }
        }
    }
}