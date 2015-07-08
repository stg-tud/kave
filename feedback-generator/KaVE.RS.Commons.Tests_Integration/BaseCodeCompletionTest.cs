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
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.CodeCompletion;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.FeaturesTestFramework.Completion;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TextControl;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.RS.Commons.Utils;
using NUnit.Framework;

namespace KaVE.RS.Commons.Tests_Integration
{
    [TestNetFramework4]
    internal abstract class BaseCodeCompletionTest : CodeCompletionTestBase
    {
        private const string Caret = "$";

        //private readonly List<CodeCompletionType> _myCodeCompletionTypes = new List<CodeCompletionType>
        //{
        //    CodeCompletionType.BasicCompletion
        //};

        protected override string RelativeTestDataPath
        {
            get { return ""; }
        }

        protected ProposalCollection ResultProposalCollection { get; private set; }

        protected override void ExecuteCodeCompletion(Suffix suffix,
            ITextControl textControl,
            IntellisenseManager intellisenseManager,
            bool automatic,
            string documentText,
            IContextBoundSettingsStore settingsStore)
        {
            ResultProposalCollection = new ProposalCollection();

            var single = CodeCompletionParameters.CreateSingle(CodeCompletionType.BasicCompletion, false);
            single.EvaluationMode = EvaluationMode.LightAndFull;
            //single.IsAutomaticCompletion = false;
            //single.InitialLookupFocusBehaviour = LookupFocusBehaviour.Hard;
            //single.InitialAutocompletionBehavior = AutocompletionBehaviour.DoNotAutocomplete;

            //// This probably does nothing
            //settingsStore.SetValue(
            //    (Expression<Func<CodeCompletionSettingsKey, LookupListSorting>>) (key => key.LookupListSorting),
            //    Sorting,
            //    null);

            //// Settings don't seem to do anything ...
            //var settings = settingsStore.GetKey<CSharpFilterStateSettingsKey>(SettingsOptimization.DoMeSlowly);
            //settings.Parameters = CompletionListFilterState.Include;
            //settingsStore.SetKey(settings, SettingsOptimization.DoMeSlowly);

            //var shit = settingsStore.GetKey<CompletionFiltersSettingsKey>(SettingsOptimization.DoMeSlowly);
            //shit.PersistFilterState = true;
            //var shit2 = settingsStore.GetKey<CodeCompletionSettingsKey>(SettingsOptimization.DoMeSlowly);

            //settingsStore.SetKey(shit, SettingsOptimization.DoMeSlowly);
            //settingsStore.SetKey(shit2, SettingsOptimization.DoMeSlowly);

            var result = intellisenseManager.GetCompletionResult(single, textControl);

            ResultProposalCollection = result == null
                ? new ProposalCollection()
                : result.LookupItems.ToProposalCollection();
        }

        [TearDown]
        public void ClearResults()
        {
            ResultProposalCollection = null;
        }

        protected void CompleteInFile(string fileContent, string newExtension)
        {
            fileContent = fileContent.Replace(Caret, "{caret}");

            var testFile = Path.Combine(BaseTestDataPath.FullPath, "adhoc_test_snippet.cs");

            File.WriteAllText(testFile, fileContent);

            WhenCodeCompletionIsInvokedInFile("adhoc_test_snippet");

            File.Delete(testFile);
        }

        protected void WhenCodeCompletionIsInvokedInFile(string fileName)
        {
            DoOneTest(fileName);
        }
    }
}