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
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names;
using KaVE.Model.Names.CSharp;
using KaVE.Model.ObjectUsage;
using KaVE.VsFeedbackGenerator.Analysis;
using KaVE.VsFeedbackGenerator.Utils;
using JetBrains.ReSharper.Feature.Services.CSharp.CodeCompletion.Infrastructure;
using JetBrains.ReSharper.Feature.Services.Lookup;
using KaVELogger = KaVE.VsFeedbackGenerator.Generators.ILogger;

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
            public CoReProposal[] Query(Query query)
            {
                return new[]
                {
                    new CoReProposal(new CoReMethodName("LSystem/Object.Equals(LSystem/Object;)LSystem/Boolean;"), 0.85),
                    new CoReProposal(new CoReMethodName("LSystem/Object.ToString()LSystem/String;"), 0.6),
                    new CoReProposal(new CoReMethodName("LSystem/Object.GetHashCode()LSystem/Int32;"), 0.35),
                    new CoReProposal(new CoReMethodName("LSystem/Object.GetType()LSystem/Type;"), 0.1)
                };
            }
        }
    }

    //[Language(typeof (CSharpLanguage))]
    public class ExemplaryProposalProvider : CSharpItemsProviderBasic
    {
        private readonly IModelStore _store;
        private readonly KaVELogger _logger;

        private Context _context;
        private IUsageModel _model;

        public ExemplaryProposalProvider(IModelStore store, KaVELogger logger)
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
            var query = CreateQuery(_context);
            var proposals = _model.Query(query);
            foreach (var item in collector.Items)
            {
                ConditionallyAddWrappedLookupItem(collector, proposals, item);
            }
            collector.ItemAdded += (item => ConditionallyAddWrappedLookupItem(collector, proposals, item));
            return base.AddLookupItems(context, collector);
        }

        private static void ConditionallyAddWrappedLookupItem(GroupedItemsCollector collector,
            IEnumerable<CoReProposal> proposals,
            ILookupItem candidate)
        {
            if (candidate is LookupItemWrapper)
            {
                return;
            }
            var representation = candidate.ToProposal().ToCoReName();
            if (representation != null)
            {
                var matchingProposal = proposals.FirstOrDefault(p => p.Name.Equals(representation));
                if (matchingProposal != null)
                {
                    collector.AddToTop(new LookupItemWrapper(candidate, matchingProposal.Probability));
                }
            }
        }

        private static Query CreateQuery(Context context)
        {
            var triggerType = ExtractTypeName(context.TriggerTarget);
            if (context.EnclosingMethod == null || triggerType == null)
            {
                return null;
            }
            return new Query(new List<CallSite>())
            {
                //definition = new DefinitionSite(), // we are not object-sensitive yet
                classCtx = context.TypeShape.TypeHierarchy.Element.ToCoReName(),
                methodCtx = context.EnclosingMethod.ToCoReName(),
                type = triggerType.ToCoReName()
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
            var propertyName = triggerTarget as IPropertyName;
            if (propertyName != null)
            {
                return propertyName.ValueType;
            }
            var methodName = triggerTarget as IMethodName;
            if (methodName != null)
            {
                return methodName.ReturnType;
            }
            return null;
        }
    }
}