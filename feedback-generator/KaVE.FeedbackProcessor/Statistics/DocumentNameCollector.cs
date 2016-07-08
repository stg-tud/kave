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
using KaVE.Commons.Model.Naming.IDEComponents;
using KaVE.Commons.Utils.Collections;

namespace KaVE.FeedbackProcessor.Statistics
{
    class DocumentNameCollector : BaseEventProcessor
    {
        public IMultiset<string> AllDocumentNames { get; private set; }

        public DocumentNameCollector()
        {
            AllDocumentNames = new Multiset<string>();

            RegisterFor<IDEEvent>(e => AddName(e.ActiveDocument));
            RegisterFor<DocumentEvent>(de => AddName(de.Document));
            RegisterFor<IDEStateEvent>(idese => AddNames(idese.OpenDocuments));
        }

        private void AddNames(IEnumerable<IDocumentName> documents)
        {
            foreach (var name in documents)
            {
                AddName(name);
            }
        }

        private void AddName(IDocumentName windowName)
        {
            if (windowName != null)
            {
                AllDocumentNames.Add(windowName.Identifier);
            }
        }
    }
}