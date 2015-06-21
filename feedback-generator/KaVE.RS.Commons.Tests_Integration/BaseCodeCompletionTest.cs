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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Application.Settings;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.CodeCompletion;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure.LookupItems;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure.LookupItems.Impl;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Settings;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.FeaturesTestFramework.Completion;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TestFramework;
using JetBrains.TextControl;
using JetBrains.Util;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Utils.Assertion;
using KaVE.RS.Commons.Utils;
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

        protected void MyExecuteCodeCompletion(Suffix suffix,
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

            var languageManager = Solution.GetComponent<ILanguageManager>();
            var service = languageManager.TryGetService<CodeCompletionModifierProvider>(
                psiSourceFile.PrimaryPsiLanguage);


            CodeCompletionParameters completionParameters;
            var codeCompletionTypes = _myCodeCompletionTypes.Take(_myCodeCompletionTypes.Count - 1).ToList();
            var myCodeCompletionType = _myCodeCompletionTypes[_myCodeCompletionTypes.Count - 1];
            if (service == null)
            {
                completionParameters = service.GetModifier(_myCodeCompletionTypes, automatic, settingsStore);
            }
            else
            {
                completionParameters = CodeCompletionModifierProvider.GetModifierBasic(
                    codeCompletionTypes,
                    myCodeCompletionType,
                    automatic,
                    (settingsStore.GetValue(
                        (Expression<Func<CodeCompletionSettingsKey, bool>>) (key => key.ImportItemsInBasicCompletion),
                        null)
                        ? 1
                        : 0) != 0,
                    1 != 0,
                    1 != 0,
                    1 != 0,
                    0 != 0,
                    0 != 0);
            }
            //CodeCompletionParameters parameters = completionParameters;


            var parameters = service != null
                ? service.GetModifier(_myCodeCompletionTypes, automatic, settingsStore)
                : CodeCompletionModifierProvider.GetModifierBasic(
                    codeCompletionTypes,
                    myCodeCompletionType,
                    automatic,
                    true);

            var codeCompletionResult = intellisenseManager.GetCompletionResult(parameters, textControl);
            if (codeCompletionResult != null)
            {
                /* var lis = codeCompletionResult.LookupItems;
                
                var best =
                    ((SelectionStrategyWithPreferences) codeCompletionResult.SelectionStrategy).GetAllPreferredItems(
                        codeCompletionResult.GetFilteredLookupItems()).ToHashSet();
                ResultProposalCollection = GetItemsFromResult(codeCompletionResult, best).ToProposalCollection();
                 * */
                ResultProposalCollection = new ProposalCollection();
            }
            else
            {
                ResultProposalCollection = new ProposalCollection();
            }


            settingsStore.SetValue(
                (Expression<Func<CodeCompletionSettingsKey, LookupListSorting>>) (key => key.LookupListSorting),
                Sorting,
                null);
            Asserts.That(TestType == CodeCompletionTestType.List);

            ExecuteCodeCompletionList(
                textControl,
                intellisenseManager,
                documentText,
                parameters,
                projectFile,
                settingsStore);
        }

        // TODO RS9: this is currently a 1:1 copy from disassembled JetBrains Version
        protected override void ExecuteCodeCompletion(Suffix suffix,
            ITextControl textControl,
            IntellisenseManager intellisenseManager,
            bool automatic,
            string documentText,
            IContextBoundSettingsStore settingsStore)
        {
            // TODO RS9
            ResultProposalCollection = new ProposalCollection();
            //

            IPsiSourceFile psiSourceFile = textControl.Document.GetPsiSourceFile(Solution);
            Assert.IsNotNull(psiSourceFile, "psiSourceFile == null");
            IProjectFile projectFile = psiSourceFile.ToProjectFile();
            Assert.IsNotNull(projectFile, "projectFile == null");
            CodeCompletionModifierProvider service =
                Solution.GetComponent<ILanguageManager>()
                        .TryGetService<CodeCompletionModifierProvider>(psiSourceFile.PrimaryPsiLanguage);
            CodeCompletionParameters completionParameters;
            if (service == null)
            {
                completionParameters =
                    CodeCompletionModifierProvider.GetModifierBasic(
                        _myCodeCompletionTypes.Take(_myCodeCompletionTypes.Count - 1).ToList(),
                        _myCodeCompletionTypes[_myCodeCompletionTypes.Count - 1],
                        (automatic ? 1 : 0) != 0,
                        (settingsStore.GetValue(
                            (Expression<Func<CodeCompletionSettingsKey, bool>>)
                                (key => key.ImportItemsInBasicCompletion),
                            null)
                            ? 1
                            : 0) != 0,
                        1 != 0,
                        1 != 0,
                        1 != 0,
                        0 != 0,
                        0 != 0);
            }
            else
            {
                completionParameters = service.GetModifier(_myCodeCompletionTypes, automatic, settingsStore);
            }
            CodeCompletionParameters parameters = completionParameters;
            settingsStore.SetValue(
                (Expression<Func<CodeCompletionSettingsKey, LookupListSorting>>) (key => key.LookupListSorting),
                Sorting,
                null);
            Asserts.That(TestType == CodeCompletionTestType.List);
            ExecuteCodeCompletionList(
                textControl,
                intellisenseManager,
                documentText,
                parameters,
                projectFile,
                settingsStore);
        }

        // TODO RS9: this is currently a 1:1 copy from disassembled JetBrains Version
        private void ExecuteCodeCompletionList(ITextControl textControl,
            IntellisenseManager intellisenseManager,
            string documentText,
            CodeCompletionParameters parameters,
            IProjectFile projectFile,
            IContextBoundSettingsStore settingsStore)
        {
            LookupListSorting sorting =
                settingsStore.GetValue(
                    (Expression<Func<CodeCompletionSettingsKey, LookupListSorting>>) (key => key.LookupListSorting),
                    null);
            FilteredLookupItems filteredItems;
            ICodeCompletionResult listResult = GetCompletionResult(
                textControl,
                intellisenseManager,
                parameters,
                sorting,
                out filteredItems);

            //
            string setting1 = BaseTest.GetSetting(documentText, "FILTERS");
            JetHashSet<string> jetHashSet;
            if (string.IsNullOrEmpty(setting1))
            {
                jetHashSet = null;
            }
            else
            {
                jetHashSet = setting1.Split(';').ToHashSet();
            }
            JetHashSet<string> filterIds = jetHashSet;

            var res = intellisenseManager.GetCompletionResult(parameters, textControl);
            // var itemsFromResult = GetItemsFromResult(listResult, filterIds, filteredItems);

            ResultProposalCollection = res == null ? new ProposalCollection() : res.LookupItems.ToProposalCollection();
            //

            /*ExecuteWithGold(
                projectFile,
                writer =>
                {
                    string setting1 = BaseTest.GetSetting(documentText, "FILTERS");
                    JetHashSet<string> jetHashSet;
                    if (string.IsNullOrEmpty(setting1))
                    {
                        jetHashSet = null;
                    }
                    else
                    {
                        jetHashSet = setting1.Split(';').ToHashSet();
                    }
                    JetHashSet<string> filterIds = jetHashSet;
                    bool flag1 = true;
                    string setting2 = BaseTest.GetSetting(documentText, "CHECK_EXPLICIT_INVOCATION");
                    if (!string.IsNullOrEmpty(setting2))
                    {
                        flag1 = bool.Parse(setting2);
                    }
                    if (flag1)
                    {
                        CheckResultList(listResult, writer, filterIds, filteredItems, sorting);
                    }
                    bool flag2 = CheckAutomaticCompletionDefault();
                    string setting3 = BaseTest.GetSetting(documentText, "CHECK_AUTO_POPUP");
                    if (!string.IsNullOrEmpty(setting3))
                    {
                        flag2 = bool.Parse(setting3);
                    }
                    if (_myCodeCompletionTypes.Count != 1 ||
                        !(_myCodeCompletionTypes[0] == CodeCompletionType.BasicCompletion) || !flag2)
                    {
                        return;
                    }
                    Solution.GetPsiServices().Files.CommitAllDocuments();
                    CodeCompletionParameters single =
                        CodeCompletionParameters.CreateSingle(CodeCompletionType.BasicCompletion, false);
                    single.IsAutomaticCompletion = true;
                    listResult = GetCompletionResult(
                        textControl,
                        intellisenseManager,
                        single,
                        sorting,
                        out filteredItems);
                    writer.WriteLine("# AUTOMATIC #");
                    CheckResultList(listResult, writer, filterIds, filteredItems, sorting);
                },
                true,
                null);*/
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