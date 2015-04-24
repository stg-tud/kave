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
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.Exceptions;
using KaVE.VsFeedbackGenerator.Analysis;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis
{
    [ShellComponent, Language(typeof (CSharpLanguage))]
    public class TestAnalysisTrigger : CSharpItemsProviderBase<CSharpCodeCompletionContext>
    {
        public ISet<IMethodName> LastEntryPoints { get; private set; }
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
            LastContext = ContextAnalysis.Analyze(context, MockLogger());
            LastSST = LastContext.SST;

            var typeDeclaration = ContextAnalysis.FindEnclosing<ITypeDeclaration>(context.NodeInFile);
            if (typeDeclaration != null)
            {
                var typeShape = new TypeShapeAnalysis().Analyze(typeDeclaration);
                var entryPoints = new EntryPointSelector(typeDeclaration, typeShape).GetEntryPoints();
                LastEntryPoints = Sets.NewHashSetFrom(entryPoints.Select(ep => ep.Name));
            }

            return false;
        }

        private ILogger MockLogger()
        {
            var logger = new Commons.TestUtils.Utils.Exceptions.TestLogger(false);
            logger.ErrorLogged += RememberError;
            return logger;
        }

        private void RememberError(Exception exception, string msg)
        {
            LastException = new Tuple<Exception, string>(exception, msg);
        }
    }
}