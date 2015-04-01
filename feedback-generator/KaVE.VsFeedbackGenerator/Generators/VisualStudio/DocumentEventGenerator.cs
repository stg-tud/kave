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

using EnvDTE;
using JetBrains.Application;
using JetBrains.Application.Components;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.Utils.Names;

namespace KaVE.VsFeedbackGenerator.Generators.VisualStudio
{
    [ShellComponent(ProgramConfigurations.VS_ADDIN)]
    internal class DocumentEventGenerator : EventGeneratorBase
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly DocumentEvents _documentEvents;

        public DocumentEventGenerator(IRSEnv env, IMessageBus messageBus, IDateUtils dateUtils)
            : base(env, messageBus, dateUtils)
        {
            _documentEvents = DTE.Events.DocumentEvents;
            _documentEvents.DocumentOpened += HandleDocumentOpened;
            _documentEvents.DocumentSaved += HandleDocumentSaved;
            _documentEvents.DocumentClosing += HandleDocumentClosing;
        }

        private void HandleDocumentOpened(Document document)
        {
            Fire(document, DocumentEvent.DocumentAction.Opened);
        }

        private void HandleDocumentSaved(Document document)
        {
            Fire(document, DocumentEvent.DocumentAction.Saved);
        }

        private void HandleDocumentClosing(Document document)
        {
            Fire(document, DocumentEvent.DocumentAction.Closing);
        }

        private void Fire(Document document, DocumentEvent.DocumentAction action)
        {
            var documentEvent = Create<DocumentEvent>();
            documentEvent.Document = document.GetName();
            documentEvent.Action = action;
            FireNow(documentEvent);
        }
    }
}