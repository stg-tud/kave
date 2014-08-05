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
 *    - Dennis Albrecht
 *    - Sebastian Proksch
 */

using System;
using System.Linq;
using JetBrains.ReSharper.Feature.Services.CSharp.CodeCompletion.Infrastructure;
using JetBrains.ReSharper.Feature.Services.Lookup;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names;
using KaVE.VsFeedbackGenerator.Analysis;
using KaVE.VsFeedbackGenerator.Generators;

namespace KaVE.VsFeedbackGenerator.CodeCompletion
{
    //[Language(typeof (CSharpLanguage))]
    public class ExemplaryProposalProvider : CSharpItemsProviderBasic
    {
        private const string ExpectedType = "MyButton";

        private readonly ExampleNetwork _network = new ExampleNetwork();
        private Context _context;
        private readonly ILogger _logger;

        public ExemplaryProposalProvider(ILogger logger)
        {
            _logger = logger;
        }

        protected override bool IsAvailable(CSharpCodeCompletionContext context)
        {
            _context = ContextAnalysis.Analyze(context, _logger);
            var typeName = _context.TriggerTarget as ITypeName;
            return typeName != null && typeName.Name == ExpectedType;
        }

        protected override bool AddLookupItems(CSharpCodeCompletionContext context, GroupedItemsCollector collector)
        {
            try
            {
                _network.Reset();

                SetCalledMethods();


                foreach (var t in _network.GetProbabilities())
                {
                    var probability = (int) (t.Item2*100);
                    var lookupItem = context.LookupItemsFactory.CreateTextLookupItem(t.Item1 + "();");
                    collector.AddToTop(new LookupItemWrapper(lookupItem, t.Item1, probability));
                }
                return base.AddLookupItems(context, collector);
            }
            catch (Exception)
            {
                return base.AddLookupItems(context, collector);
            }
        }

        private void SetCalledMethods()
        {
            var calledMethod = _context.EntryPointToCalledMethods[_context.EnclosingMethod];
            var filtered = calledMethod.Where(m => m.DeclaringType.Name == ExpectedType);
            foreach (var m in filtered)
            {
                _network.Set(m.Name);
            }
        }
    }
}