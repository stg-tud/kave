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
 *    - Sebastian Proksch
 */

using System;
using System.Collections.Generic;
using JetBrains.Application;
using JetBrains.ReSharper.Feature.Services.CodeCompletion;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.TextControl;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names;
using KaVE.Model.SSTs;
using KaVE.Model.SSTs.Impl;
using KaVE.VsFeedbackGenerator.RS8Tests.Analysis;

namespace KaVE.VsFeedbackGenerator.RS8Tests
{
    internal abstract class BaseCSharpCodeCompletionTest : BaseCodeCompletionTest
    {
        protected void CompleteInMethod(string methodBody)
        {
            CompleteInClass(string.Format(@"
                public void M() {{
                    {0}
                }}", methodBody));
        }

        protected void CompleteInClass(string classBody)
        {
            CompleteInCSharpFile(string.Format(@"
                namespace N {{
                    public class C {{
                        {0}
                    }}
                }}", classBody));
        }

        /// <summary>
        ///     Triggers completion in .cs-File.
        /// </summary>
        protected void CompleteInCSharpFile(string fileContent)
        {
            fileContent = @"
                using System;
                using System.Collections.Generic;
                using System.IO;
                
                " + fileContent;

            CompleteInFile(fileContent, "cs");
        }

        protected override void ExecuteCodeCompletion(Suffix suffix,
            ITextControl textControl,
            IntellisenseManager intellisenseManager,
            bool automatic,
            string documentText)
        {
            base.ExecuteCodeCompletion(suffix, textControl, intellisenseManager, automatic, documentText);

            if (TestAnalysisComponent.HasFailed)
            {
                throw new Exception(
                    TestAnalysisComponent.LastException.Item2,
                    TestAnalysisComponent.LastException.Item1);
            }
        }

        protected Context ResultContext
        {
            get { return TestAnalysisComponent.LastContext; }
        }

        protected ISST ResultSST
        {
            get { return TestAnalysisComponent.LastSST; }
        }

        protected IEnumerable<IMethodName> AnalyzedEntryPoints
        {
            get { return TestAnalysisComponent.LastEntryPoints; }
        }

        private static TestAnalysisTrigger TestAnalysisComponent
        {
            get { return Shell.Instance.GetComponent<TestAnalysisTrigger>(); }
        }
    }
}