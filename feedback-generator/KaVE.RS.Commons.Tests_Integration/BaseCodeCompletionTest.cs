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
 */

using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Application.Settings;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.CodeCompletion;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.FeaturesTestFramework.Completion;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TextControl;
using JetBrains.Util;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Utils.Assertion;
using NUnit.Framework;

namespace KaVE.RS.Commons.Tests_Integration
{
    [TestNetFramework4]
    internal abstract class BaseCodeCompletionTest : CodeCompletionTestBase
    {
        private const string Caret = "$";

        private readonly List<CodeCompletionType> _myCodeCompletionTypes = new List<CodeCompletionType>
        {
            CodeCompletionType.BasicCompletion
        };

        protected override string RelativeTestDataPath
        {
            get
            {
                var defaultNamespace = GetType().Namespace;
                var concreteNamespace = GetType().FullName;

                Asserts.NotNull(defaultNamespace, GetType() + " cannot be in global namespace!");

                string basePath;
                if (concreteNamespace.StartsWith(defaultNamespace))
                {
                    basePath = concreteNamespace.Substring(defaultNamespace.Length + 1);
                }
                else
                {
                    basePath = concreteNamespace;
                }

                return Path.Combine(basePath.Split('.'));
            }
        }
        
        protected ProposalCollection ResultProposalCollection { get; private set; }

        protected override void ExecuteCodeCompletion(Suffix suffix,
            ITextControl textControl,
            IntellisenseManager intellisenseManager,
            bool automatic,
            string documentText,
            IContextBoundSettingsStore settingsStore)
        {
            var psiSourceFile = textControl.Document.GetPsiSourceFile(Solution);
            Assert.IsNotNull(psiSourceFile, "psiSourceFile == null");
            var projectFile = psiSourceFile.ToProjectFile();
            Assert.IsNotNull(projectFile, "projectFile == null");
            var service =
                Solution.GetComponent<ILanguageManager>()
                        .TryGetService<CodeCompletionModifierProvider>(psiSourceFile.PrimaryPsiLanguage);
            // TODO RS9: meaning of (now necessary) variable?
            var importItemsInBasic = true;
            var parameters = service != null
                ? service.GetModifier(_myCodeCompletionTypes, automatic, settingsStore)
                : CodeCompletionModifierProvider.GetModifierBasic(
                    _myCodeCompletionTypes.Take(_myCodeCompletionTypes.Count - 1).ToList(),
                    _myCodeCompletionTypes[_myCodeCompletionTypes.Count - 1], automatic, importItemsInBasic);

            // TODO RS9
            /*var codeCompletionResult = intellisenseManager.GetCompletionResult(parameters, textControl);
            if (codeCompletionResult != null)
            {
                var best =
                    ((SelectionStrategyWithPreferences) codeCompletionResult.SelectionStrategy).GetAllPreferredItems(
                        codeCompletionResult.GetFilteredLookupItems()).ToHashSet();
                ResultProposalCollection = GetItemsFromResult(codeCompletionResult, best).ToProposalCollection();
            }
            else
            {*/
                ResultProposalCollection = new ProposalCollection();
            //}
        }

        [TearDown]
        public void ClearResults()
        {
            ResultProposalCollection = null;
        }

        protected void CompleteInFile(string fileContent, string newExtension)
        {
            fileContent = fileContent.Replace(Caret, "{caret}");
            var file = GetTestDataFilePath2("adhoc_test_snippet").ChangeExtension(newExtension);
            var parentPath = Path.GetDirectoryName(file.FullPath);
            Asserts.NotNull(parentPath, "impossible, since file is alway an absolute path");
            Directory.CreateDirectory(parentPath);
            WriteContentToFile(fileContent, file);
            WhenCodeCompletionIsInvokedInFile("adhoc_test_snippet");
        }

        private static void WriteContentToFile(string fileContent, FileSystemPath file)
        {
            using (var stream = file.OpenStream(FileMode.Create))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(fileContent);
                }
            }
        }

        protected void WhenCodeCompletionIsInvokedInFile(string fileName)
        {
            DoOneTest(fileName);
        }
    }
}