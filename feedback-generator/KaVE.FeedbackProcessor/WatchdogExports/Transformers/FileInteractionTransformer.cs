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
using System.IO;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Events.TestRunEvents;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Utils.Assertion;
using KaVE.FeedbackProcessor.WatchdogExports.Model;

namespace KaVE.FeedbackProcessor.WatchdogExports.Transformers
{
    internal class FileInteractionTransformer : IEventToIntervalTransformer<FileInteractionInterval>
    {
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(16);

        private readonly TransformerContext _context;

        private readonly IList<FileInteractionInterval> _intervals;
        private FileInteractionInterval _cur;

        private readonly DateTime _referenceTime;

        public FileInteractionTransformer(TransformerContext context)
        {
            _context = context;
            _intervals = new List<FileInteractionInterval>();
            _referenceTime = DateTime.MinValue;
        }

        public void ProcessEvent(IDEEvent e)
        {
            Asserts.That(e.TriggeredAt.HasValue);
            Asserts.That(e.TerminatedAt.HasValue);

            /*if (e.TriggeredAt.Value > new DateTime(2016, 08, 19, 19, 17, 38))
            {
                Console.WriteLine();
            }*/

            if (e.ActiveDocument == null || e.TerminatedAt < _referenceTime)
            {
                // TODO include case: doc.Type != "CSharp"
                return;
            }

            if (IsTimedOut(e))
            {
                _cur = null;
            }

            // untested (
            if (_cur != null && (e is TestRunEvent || HasDocumentChanged(e)))
            {
                _context.UpdateDurationForIntervalToThis(_cur, e.TriggeredAt.Value);
                _cur = null;
            }
            // )

            var classification = ClassifyEventType(e);
            if (classification.HasValue)
            {
                if (_cur == null)
                {
                    CreateNewInterval(e, classification.Value);
                }
                else
                {
                    TransformerUtils.SetDocumentTypeIfNecessary(_cur, e); // updates
                    _cur.Project = _context.CurrentProject; // might not be available from the beginning

                    if (_cur.Type == classification.Value)
                    {
                        // extend to max duration
                        _context.UpdateDurationForIntervalToMaximum(_cur, e.TerminatedAt.Value);
                    }
                    else
                    {
                        // cut duration to current trigger point
                        _context.UpdateDurationForIntervalToThis(_cur, e.TriggeredAt.Value);
                        CreateNewInterval(e, classification.Value);
                    }
                }
            }
        }

        private void CreateNewInterval(IDEEvent e, FileInteractionType classification)
        {
            _intervals.Add(_cur = _context.CreateIntervalFromEvent<FileInteractionInterval>(e));
            _cur.FileName = e.ActiveDocument.FileName;
            _cur.Type = classification;
            TransformerUtils.SetDocumentTypeIfNecessary(_cur, e); // initial
        }

        private bool HasDocumentChanged(IDEEvent e)
        {
            return _cur != null && _cur.FileName != e.ActiveDocument.FileName;
        }

        private bool IsTimedOut(IDEEvent e)
        {
            return _cur != null && e.TriggeredAt - _cur.EndTime > Timeout;
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

            if (ShouldFilterOutToolWindowEvents(e))
            {
                return null;
            }

            // not directly assignable to "type" or "read"... we prolong the existing type
            if (e is NavigationEvent || e is WindowEvent)
            {
                return _cur == null || HasDocumentChanged(e) ? FileInteractionType.Reading : _cur.Type;
            }
            if (_cur != null && e is ActivityEvent)
            {
                return _cur.Type;
            }
            return null;
        }

        private static bool ShouldFilterOutToolWindowEvents(IDEEvent e)
        {
            var we = e as WindowEvent;
            if (we == null)
            {
                return false;
            }

            var expectedCaptionForFileWindow = Path.GetFileName(e.ActiveDocument.FileName);
            var isFileWindowEvent = we.Window.Caption.Equals(expectedCaptionForFileWindow);

            return !isFileWindowEvent;
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