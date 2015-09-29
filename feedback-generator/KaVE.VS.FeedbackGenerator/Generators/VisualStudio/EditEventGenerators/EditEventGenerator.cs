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

using System;
using System.Timers;
using EnvDTE;
using JetBrains.Application;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Utils;
using KaVE.VS.FeedbackGenerator.Generators.VisualStudio.EditEventGenerators.EventContext;
using KaVE.VS.FeedbackGenerator.MessageBus;

namespace KaVE.VS.FeedbackGenerator.Generators.VisualStudio.EditEventGenerators
{
    [ShellComponent]
    internal class EditEventGenerator : EventGeneratorBase, IDisposable
    {
        // TODO evaluate good threshold value
        private const int InactivityPeriodToCompleteEditAction = 2000;

        private readonly IContextProvider _contextProvider;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly TextEditorEvents _textEditorEvents;
        private EditEvent _currentEditEvent;

        private readonly Timer _editingTimout = new Timer(InactivityPeriodToCompleteEditAction) {AutoReset = false};
        private readonly object _lock = new object();
        private TextPoint _currentStartPoint;

        public EditEventGenerator(IRSEnv env,
            IMessageBus messageBus,
            IDateUtils dateUtils,
            IContextProvider contextProvider)
            : base(env, messageBus, dateUtils)
        {
            _contextProvider = contextProvider;
            _textEditorEvents = DTE.Events.TextEditorEvents;
            _textEditorEvents.LineChanged += TextEditorEvents_LineChanged;
            _editingTimout.Elapsed += FireCurrentEditEvent;
        }

        private void TextEditorEvents_LineChanged(TextPoint startPoint, TextPoint endPoint, int hint)
        {
            lock (_lock)
            {
                _editingTimout.Stop();
                _currentEditEvent = _currentEditEvent ?? Create<EditEvent>();
                _currentEditEvent.NumberOfChanges += 1;
                // TODO subtract whitespaces from change size
                _currentEditEvent.SizeOfChanges += endPoint.AbsoluteCharOffset - startPoint.AbsoluteCharOffset;
                _currentStartPoint = startPoint;
                _editingTimout.Start();
            }
        }

        private void FireCurrentEditEvent(object sender, ElapsedEventArgs e)
        {
            lock (_lock)
            {
                _editingTimout.Stop();
                _currentEditEvent.Context2 = _contextProvider.GetCurrentContext(_currentStartPoint);
                FireNow(_currentEditEvent);
                _currentEditEvent = null;
            }
        }

        public void Dispose()
        {
            _editingTimout.Close();
        }
    }
}