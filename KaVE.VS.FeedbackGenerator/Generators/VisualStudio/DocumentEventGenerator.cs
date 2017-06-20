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

using EnvDTE;
using JetBrains.Application;
using JetBrains.Threading;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Utils;
using KaVE.VS.FeedbackGenerator.MessageBus;
using KaVE.VS.FeedbackGenerator.Utils.Naming;

namespace KaVE.VS.FeedbackGenerator.Generators.VisualStudio
{
    [ShellComponent]
    internal class DocumentEventGenerator : EventGeneratorBase
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly DocumentEvents _documentEvents;

        public DocumentEventGenerator(IRSEnv env,
            IMessageBus messageBus,
            IDateUtils dateUtils,
            IThreading threading)
            : base(env, messageBus, dateUtils, threading)
        {
            _documentEvents = DTE.Events.DocumentEvents;
            _documentEvents.DocumentOpened += HandleDocumentOpened;
            _documentEvents.DocumentSaved += HandleDocumentSaved;
            _documentEvents.DocumentClosing += HandleDocumentClosing;
        }

        private void HandleDocumentOpened(Document document)
        {
            Fire(document, DocumentAction.Opened);
        }

        private void HandleDocumentSaved(Document document)
        {
            Fire(document, DocumentAction.Saved);
        }

        private void HandleDocumentClosing(Document document)
        {
            Fire(document, DocumentAction.Closing);
        }

        private void Fire(Document document, DocumentAction action)
        {
            var documentEvent = Create<DocumentEvent>();
            documentEvent.Document = document.GetName();
            documentEvent.Action = action;
            FireNow(documentEvent);
        }
    }
}