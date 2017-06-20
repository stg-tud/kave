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

using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.FeedbackProcessor.Utils;

namespace KaVE.FeedbackProcessor.Cleanup.Processors
{
    internal class CloseEventProcessor : BaseEventMapper
    {
        private bool _filterNextDocumentEvent;

        public CloseEventProcessor()
        {
            RegisterFor<CommandEvent>(ProcessCommandEvent);
            RegisterFor<DocumentEvent>(ProcessDocumentEvent);
            RegisterFor<WindowEvent>(ProcessWindowEvent);
        }

        private void ProcessCommandEvent(CommandEvent commandEvent)
        {
            if (commandEvent.CommandId == "Close")
            {
                _filterNextDocumentEvent = true;
            }
        }

        private void ProcessWindowEvent(WindowEvent windowEvent)
        {
            if (windowEvent.Action == WindowAction.Close) DropCurrentEvent();
        }

        private void ProcessDocumentEvent(DocumentEvent documentEvent)
        {
            if (_filterNextDocumentEvent)
            {
                _filterNextDocumentEvent = false;
                DropCurrentEvent();
            }
            else
            {
                if (documentEvent.Action == DocumentAction.Closing)
                {
                    var commandEvent = new CommandEvent
                    {
                        CommandId = "Close"
                    };
                    commandEvent.CopyIDEEventPropertiesFrom(documentEvent);
                    ReplaceCurrentEventWith(commandEvent);
                }
            }
        }
    }
}