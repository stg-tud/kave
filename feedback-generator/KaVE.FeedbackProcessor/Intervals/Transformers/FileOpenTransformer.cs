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

using System.Collections.Generic;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Model.Names.VisualStudio;
using KaVE.FeedbackProcessor.Intervals.Model;

namespace KaVE.FeedbackProcessor.Intervals.Transformers
{
    internal class FileOpenTransformer : IEventToIntervalTransformer<FileOpenInterval>
    {
        private IList<FileOpenInterval> _intervals; 
        private readonly IDictionary<DocumentName, FileOpenInterval> _currentIntervals;

        public FileOpenTransformer()
        {
            _intervals = new List<FileOpenInterval>();
            _currentIntervals = new Dictionary<DocumentName, FileOpenInterval>();
        }

        public void ProcessEvent(IDEEvent @event)
        {
            if (@event.ActiveDocument != null)
            {
                if (_currentIntervals.ContainsKey(@event.ActiveDocument))
                {
                    TransformerUtils.AdaptIntervalTimeData(_currentIntervals[@event.ActiveDocument], @event);

                    var documentEvent = @event as DocumentEvent;
                    if (documentEvent != null && documentEvent.Action == DocumentEvent.DocumentAction.Closing)
                    {
                        _intervals.Add(_currentIntervals[@event.ActiveDocument]);
                        _currentIntervals.Remove(@event.ActiveDocument);
                    }
                }
                else
                {
                    var documentEvent = @event as DocumentEvent;
                    if (documentEvent != null && documentEvent.Action == DocumentEvent.DocumentAction.Opened)
                    {
                        _currentIntervals[@event.ActiveDocument] =
                            TransformerUtils.CreateIntervalFromFirstEvent<FileOpenInterval>(@event);
                        _currentIntervals[@event.ActiveDocument].Filename = @event.ActiveDocument.FileName;
                    }
                }
            }
        }

        public IEnumerable<FileOpenInterval> SignalEndOfEventStream()
        {
            foreach (var interval in _currentIntervals.Values)
            {
                _intervals.Add(interval);
            }

            return _intervals;
        }
    }
}