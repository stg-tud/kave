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
using System.Linq;
using JetBrains.Application;
using JetBrains.ReSharper.Feature.Services.CSharp.CodeCompletion.Infrastructure;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names;
using KaVE.VsFeedbackGenerator.Analysis;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis
{
    [ShellComponent, Language(typeof (CSharpLanguage))]
    public class TestAnalysisTrigger : CSharpItemsProviderBase<CSharpCodeCompletionContext>
    {
        public IEnumerable<IMethodName> LastEntryPoints { get; set; }
        public Context LastContext { get; private set; }
        public Exception LastException { get; private set; }

        public bool HasFailed
        {
            get { return LastException != null; }
        }

        protected override bool AddLookupItems(CSharpCodeCompletionContext context, GroupedItemsCollector collector)
        {
            try
            {
                LastException = null;
                LastContext = ContextAnalysis.Analyze(context);

                var typeDeclaration = ContextAnalysis.FindEnclosing<ITypeDeclaration>(context.NodeInFile);
                if (typeDeclaration != null)
                {
                    var typeShape = new TypeShapeAnalysis().Analyze(typeDeclaration);
                    LastEntryPoints =
                        new EntryPointSelector(typeDeclaration, typeShape).GetEntrypoints().Select(ep => ep.Name);
                }
            }
            catch (Exception e)
            {
                LastException = e;
                LastContext = null;
            }
            return false;
        }
    }
}