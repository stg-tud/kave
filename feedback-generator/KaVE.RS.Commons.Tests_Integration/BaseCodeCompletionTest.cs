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

using System.IO;
using System.Threading;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.CodeCompletion;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure.LookupItems;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure.LookupItems.Impl;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.FeaturesTestFramework.Completion;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TextControl;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.RS.Commons.Utils;

namespace KaVE.RS.Commons.Tests_Integration
{
    [TestNetFramework4]
    internal abstract class BaseCodeCompletionTest : CodeCompletionTestBase
    {
        private const string Caret = "$";

        protected override string RelativeTestDataPath
        {
            get { return ""; }
        }

        protected ProposalCollection ResultProposalCollection { get; private set; }

        protected void CompleteInFile(string content, string newExtension = "cs")
        {
            content = content.Replace(Caret, "{caret}");
            var fileName = Path.GetRandomFileName() + "." + newExtension;
            var testFile = Path.Combine(BaseTestDataPath.FullPath, fileName);

            try
            {
                File.WriteAllText(testFile, content);
                WhenCodeCompletionIsInvokedInFile(fileName);
            }
            finally
            {
                var fileDeleted = !File.Exists(testFile);
                while (!fileDeleted)
                {
                    try
                    {
                        File.Delete(testFile);
                        fileDeleted = true;
                    }
                    catch
                    {
                        Thread.Sleep(100);
                    }
                }
            }
        }

        protected void WhenCodeCompletionIsInvokedInFile(string fileName)
        {
            DoTestFiles(fileName);
        }

        protected override void ExecuteCodeCompletion(Suffix suffix,
            ITextControl textControl,
            IntellisenseManager intellisenseManager,
            bool automatic,
            IContextBoundSettingsStore settingsStore)
        {
            ResultProposalCollection = new ProposalCollection();

            var parameters = CodeCompletionParameters.CreateSingle(CodeCompletionType.BasicCompletion);
            parameters.EvaluationMode = EvaluationMode.LightAndFull;
            FilteredLookupItems filteredItems;
            var result = GetCompletionResult(
                textControl,
                intellisenseManager,
                parameters,
                LookupListSorting.ByRelevance,
                out filteredItems);

            if (result != null)
            {
                ResultProposalCollection = result.LookupItems.ToProposalCollection();
            }
        }
    }
}