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
using System.Collections.Generic;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.FeedbackProcessor.Intervals.Model;

namespace KaVE.FeedbackProcessor.Intervals.Transformers
{
    internal class FileInteractionTransformer : IEventToIntervalTransformer<FileInteractionInterval>
    {
        private readonly IList<FileInteractionInterval> _intervals;
        private FileInteractionInterval _currentInterval;
        private DateTime _referenceTime;
        private readonly TransformerContext _context;

        public FileInteractionTransformer(TransformerContext context)
        {
            _intervals = new List<FileInteractionInterval>();
            _referenceTime = DateTime.MinValue;
            _context = context;
        }

        public void ProcessEvent(IDEEvent @event)
        {
            if (@event.ActiveDocument == null || @event.TerminatedAt.GetValueOrDefault() < _referenceTime)
            {
                return;
            }

            var classification = ClassifyEventType(@event);

            if (_currentInterval != null)
            {
                var activeDocumentChanged = _currentInterval.FileName != @event.ActiveDocument.FileName;
                var classificationChanged = classification != null && _currentInterval.Type != classification;

                if (activeDocumentChanged || classificationChanged)
                {
                    _currentInterval = null;
                }
            }

            if (_currentInterval == null && classification != null)
            {
                _currentInterval = _context.CreateIntervalFromEvent<FileInteractionInterval>(@event);
                _intervals.Add(_currentInterval);

                if (_currentInterval.StartTime < _referenceTime)
                {
                    _currentInterval.Duration -= _referenceTime - _currentInterval.StartTime;
                    _currentInterval.StartTime = _referenceTime;
                }

                _currentInterval.FileName = @event.ActiveDocument.FileName;
                _currentInterval.Type = classification.GetValueOrDefault();
            }

            if (_currentInterval != null)
            {
                _context.AdaptIntervalTimeData(_currentInterval, @event);
                _referenceTime = @event.TerminatedAt.GetValueOrDefault();
                TransformerUtils.SetDocumentTypeIfNecessary(_currentInterval, @event);
            }
        }

        // The debugger creates EditEvents for some reason. We want to ignore these EditEvents
        // and not switch to typing mode.
        private bool _currentlyDebugging;

        private FileInteractionType? ClassifyEventType(IDEEvent e)
        {
            if ((!_currentlyDebugging && e is EditEvent) || e is CompletionEvent || IsTypingDocumentEvent(e) ||
                IsTypingCommandEvent(e))
            {
                return FileInteractionType.Typing;
            }
            if (e is BuildEvent || IsReadingDocumentEvent(e) || IsDebuggerEvent(e))
            {
                return FileInteractionType.Reading;
            }
            return null;
        }

        private bool IsDebuggerEvent(IDEEvent ideEvent)
        {
            var de = ideEvent as DebuggerEvent;
            if (de != null)
            {
                if (de.Mode == DebuggerMode.Run)
                {
                    _currentlyDebugging = true;
                }
                if (de.Mode == DebuggerMode.Design)
                {
                    _currentlyDebugging = false;
                }
                return true;
            }
            return false;
        }

        private bool IsTypingCommandEvent(IDEEvent ideEvent)
        {
            var ce = ideEvent as CommandEvent;
            return ce != null &&
                   (ce.CommandId.StartsWith("TextControl") || ce.CommandId.StartsWith("Completion") ||
                    ce.CommandId.StartsWith("VsAction:1:Edit"));
        }

        private bool IsReadingDocumentEvent(IDEEvent ideEvent)
        {
            var de = ideEvent as DocumentEvent;
            return de != null && de.Action != DocumentAction.Saved;
        }

        private bool IsTypingDocumentEvent(IDEEvent ideEvent)
        {
            var de = ideEvent as DocumentEvent;
            return de != null && de.Action == DocumentAction.Saved;
        }

        public IEnumerable<FileInteractionInterval> SignalEndOfEventStream()
        {
            return _intervals;
        }
    }
}