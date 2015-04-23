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
 *    - Mattis Manfred Kämmerer
 *    - Markus Zimmermann
 */

using System.Collections.Generic;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Utils.Collections;

namespace KaVE.FeedbackProcessor.Cleanup.Processors
{
    internal class CloseEventProcessor : IIDEEventProcessor
    {
        private bool _filterNextDocumentEvent;

        public ISet<IDEEvent> Process(IDEEvent @event)
        {
            var commandEvent = @event as CommandEvent;
            if (commandEvent != null)
            {
                return ProcessCommandEvent(commandEvent);
            }

            var documentEvent = @event as DocumentEvent;
            if (documentEvent != null)
            {
                return ProcessDocumentEvent(documentEvent);
            }

            var windowEvent = @event as WindowEvent;
            if (windowEvent != null)
            {
                return ProcessWindowEvent(windowEvent);
            }

            return new KaVEHashSet<IDEEvent>{@event};
        }

        private ISet<IDEEvent> ProcessCommandEvent(CommandEvent commandEvent)
        {
            if (commandEvent.CommandId == "Close")
            {
                _filterNextDocumentEvent = true;
            }

            return new KaVEHashSet<IDEEvent>{commandEvent};
        }

        private static ISet<IDEEvent> ProcessWindowEvent(WindowEvent windowEvent)
        {
            return windowEvent.Action == WindowEvent.WindowAction.Close ? null : new KaVEHashSet<IDEEvent>{windowEvent};
        }

        private ISet<IDEEvent> ProcessDocumentEvent(DocumentEvent documentEvent)
        {
            if (_filterNextDocumentEvent)
            {
                _filterNextDocumentEvent = false;
                return null;
            }

            if (documentEvent.Action != DocumentEvent.DocumentAction.Closing)
            {
                return new KaVEHashSet<IDEEvent>{documentEvent};
            }

            var commandEvent = new CommandEvent
            {
                ActiveDocument = documentEvent.ActiveDocument,
                ActiveWindow = documentEvent.ActiveWindow,
                CommandId = "Close",
                Duration = documentEvent.Duration,
                IDESessionUUID = documentEvent.IDESessionUUID,
                TerminatedAt = documentEvent.TerminatedAt,
                TriggeredAt = documentEvent.TriggeredAt,
                TriggeredBy = documentEvent.TriggeredBy
            };

            return new KaVEHashSet<IDEEvent>{commandEvent};
        }
    }
}