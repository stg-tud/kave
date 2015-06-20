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
 *    - Sebastian Proksch
 *    - Sven Amann
 */

using System;
using System.Collections.Generic;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure.LookupItems;
using JetBrains.ReSharper.Feature.Services.CSharp.CodeCompletion.Infrastructure;
using JetBrains.ReSharper.Features.Intellisense.CodeCompletion.CSharp.Rules;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Utils.Exceptions;
using KaVE.RS.Commons.Analysis;
using KaVE.RS.Commons.Analysis.CompletionTarget;
using KaVE.RS.Commons.Analysis.Transformer;

namespace KaVE.RS.Commons.Tests_Integration.Analysis
{
    // TODO RS9: removed [ShellComponent] make sure that is correct
    [Language(typeof (CSharpLanguage))]
    public class TestAnalysisTrigger : CSharpItemsProviderBase<CSharpCodeCompletionContext>
    {
        public static bool IsPrintingType = false;

        public ISet<IMethodName> LastEntryPoints { get; private set; }
        public CompletionTargetMarker LastCompletionMarker { get; set; }
        public Context LastContext { get; private set; }
        public ISST LastSST { get; private set; }
        public Tuple<Exception, string> LastException { get; private set; }

        public bool HasFailed
        {
            get { return LastException != null; }
        }

        protected override bool AddLookupItems(CSharpCodeCompletionContext context, GroupedItemsCollector collector)
        {
            LastException = null;
            var analysisResult = ContextAnalysis.Analyze(context, MockLogger());
            LastContext = analysisResult.Context;
            LastEntryPoints = analysisResult.EntryPoints;
            LastCompletionMarker = analysisResult.CompletionMarker;
            if (IsPrintingType)
            {
                PrintType(context, LastCompletionMarker);
            }

            
            
            LastSST = LastContext.SST;

            return false;
        }

        private static void PrintType(CSharpCodeCompletionContext context, CompletionTargetMarker marker)
        {
            for (var node = context.NodeInFile; node != null; node = node.Parent)
            {
                var type = node as ITypeDeclaration;
                if (type != null)
                {
                    JetbrainsTreeDebugger.Dump(type, marker);
                    return;
                }
            }
        }

        private ILogger MockLogger()
        {
            var logger = new KaVE.Commons.TestUtils.Utils.Exceptions.TestLogger(false);
            logger.ErrorLogged += RememberError;
            return logger;
        }

        private void RememberError(Exception exception, string msg)
        {
            LastException = new Tuple<Exception, string>(exception, msg);
        }
    }
}