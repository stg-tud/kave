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

using System.Collections.Generic;
using System.Linq;
// ReSharper disable RedundantUsingDirective
using JetBrains.Application;
using JetBrains.ReSharper.Feature.Services.CSharp.CodeCompletion.Infrastructure;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
// ReSharper restore RedundantUsingDirective
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names;
using KaVE.Model.Names.CSharp;
using KaVE.Model.ObjectUsage;
using KaVE.VsFeedbackGenerator.Analysis;
using KaVE.VsFeedbackGenerator.Generators;
using KaVE.VsFeedbackGenerator.Utils;

namespace KaVE.VsFeedbackGenerator.CodeCompletion
{
    //[ShellComponent]
    public class MyModelStore : IModelStore
    {
        public IUsageModel GetModel(CoReTypeName typeName)
        {
            return GetModel(typeName.Name, typeName);
        }

        public IUsageModel GetModel(string assembly, CoReTypeName typeName)
        {
            return new MyUsageModel();
        }

        private class MyUsageModel : IUsageModel
        {
            public KeyValuePair<CoReMethodName, double>[] Query(Query query)
            {
                return new[]
                {
                    new KeyValuePair<CoReMethodName, double>(new CoReMethodName("LSystem/Object.Equals(LSystem/Object;)LSystem/Boolean;"), 0.85),
                    new KeyValuePair<CoReMethodName, double>(new CoReMethodName("LSystem/Object.ToString()LSystem/String;"), 0.6),
                    new KeyValuePair<CoReMethodName, double>(new CoReMethodName("LSystem/Object.GetHashCode()LSystem/Int32;"), 0.35),
                    new KeyValuePair<CoReMethodName, double>(new CoReMethodName("LSystem/Object.GetType()LSystem/Type;"), 0.1)
                };
            }
        }
    }

    //[Language(typeof (CSharpLanguage))]
    public class ExemplaryProposalProvider : CSharpItemsProviderBasic
    {
        private readonly IModelStore _store;
        private readonly ILogger _logger;

        private Context _context;
        private IUsageModel _model;

        public ExemplaryProposalProvider(IModelStore store, ILogger logger)
        {
            _store = store;
            _logger = logger;
        }

        protected override bool IsAvailable(CSharpCodeCompletionContext context)
        {
            _context = ContextAnalysis.Analyze(context, _logger);
            var typeName = ExtractTypeName(_context.TriggerTarget);
            _model = typeName == null ? null : _store.GetModel(typeName.ToCoReName());
            return _model != null;
        }

        protected override bool AddLookupItems(CSharpCodeCompletionContext context, GroupedItemsCollector collector)
        {
            var proposals = ConvertPropabilities(_model.Query(CreateQuery(_context))).ToList();
            foreach (var item in collector.Items)
            {
                ConditionallyAddWrappedLookupItem(collector, proposals, item);
            }
            collector.ItemAdded += (item => ConditionallyAddWrappedLookupItem(collector, proposals, item));
            return base.AddLookupItems(context, collector);
        }

        private static IEnumerable<KeyValuePair<CoReMethodName, int>> ConvertPropabilities(
            IEnumerable<KeyValuePair<CoReMethodName, double>> proposals)
        {
            return
                proposals.Select(
                    proposal => new KeyValuePair<CoReMethodName, int>(proposal.Key, (int) (proposal.Value*100)));
        }

        private static void ConditionallyAddWrappedLookupItem(GroupedItemsCollector collector,
            IEnumerable<KeyValuePair<CoReMethodName, int>> proposals,
            ILookupItem candidate)
        {
            if (candidate is LookupItemWrapper)
            {
                return;
            }
            var representation = candidate.ToProposal().ToCoReName();
            if (representation != null)
            {
                var matchingProposals = proposals.Where(p => p.Key.Equals(representation));
                foreach (var probability in matchingProposals.Select(proposal => proposal.Value))
                {
                    collector.AddToTop(new LookupItemWrapper(candidate, probability));
                }
            }
        }

        private static Query CreateQuery(Context context)
        {
            return new Query(new List<CallSite>())
            {
                //definition = new DefinitionSite(), // we are not object-sensitive yet
                classCtx = context.TypeShape.TypeHierarchy.Element.ToCoReName(),
                methodCtx = (context.EnclosingMethod == null) ? null : context.EnclosingMethod.ToCoReName(),
                type = (context.TriggerTarget is ITypeName) ? (context.TriggerTarget as ITypeName).ToCoReName() : null
            };
        }

        private static ITypeName ExtractTypeName(IName triggerTarget)
        {
            var typeName = triggerTarget as ITypeName;
            if (typeName != null)
            {
                return typeName;
            }
            var paramName = triggerTarget as IParameterName;
            if (paramName != null)
            {
                return paramName.ValueType;
            }
            var fieldName = triggerTarget as IFieldName;
            if (fieldName != null)
            {
                return fieldName.ValueType;
            }
            var localVarName = triggerTarget as LocalVariableName;
            if (localVarName != null)
            {
                return localVarName.ValueType;
            }
            return null;
        }
    }
}