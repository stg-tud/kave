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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using KaVE.Commons.Utils.Assertion;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.TestUtils.ExternalTests
{
    public static class ExternalTestCaseProvider
    {
        private const string ExpectedCompactFileName = "expected-compact.json";
        private const string ExpectedPrettyFileName = "expected-pretty.json";
        private const string TypeHintFileName = "settings.ini";

        [Pure, NotNull]
        public static IEnumerable<TestCase> GetTestCases(string rootFolderPath)
        {
            return RecursiveGetTestCases(
                new DirectoryInfo(rootFolderPath),
                Regex.Escape(rootFolderPath));
        }

        [Pure, NotNull]
        private static IEnumerable<TestCase> RecursiveGetTestCases(DirectoryInfo folderDirectory,
            string rootFolderPath)
        {
            var testCases = GetTestCasesInCurrentFolder(folderDirectory, rootFolderPath).ToList();

            foreach (var subdirectory in folderDirectory.GetDirectories())
            {
                testCases.AddRange(RecursiveGetTestCases(subdirectory, rootFolderPath));
            }

            return testCases;
        }

        [Pure, NotNull]
        private static IEnumerable<TestCase> GetTestCasesInCurrentFolder(DirectoryInfo directory,
            string rootFolderPath)
        {
            var compactExpectedFile =
                directory.GetFiles().FirstOrDefault(file => file.Name.Equals(ExpectedCompactFileName));
            var prettyExpectedFile =
                directory.GetFiles().FirstOrDefault(file => file.Name.Equals(ExpectedPrettyFileName));

            if (compactExpectedFile == null || prettyExpectedFile == null)
            {
                return new List<TestCase>();
            }

            var serializedType = GetSerializedType(directory);
            Asserts.NotNull(serializedType, "ini file missing or invalid!");

            var inputFiles = directory.GetFiles().Where(IsTestCaseFile);
            return
                inputFiles.Select(
                    inputFile =>
                        new TestCase(
                            GetTestCaseName(rootFolderPath, inputFile.FullName),
                            serializedType,
                            File.ReadAllText(inputFile.FullName),
                            File.ReadAllText(compactExpectedFile.FullName),
                            File.ReadAllText(prettyExpectedFile.FullName)));
        }

        [Pure, CanBeNull]
        private static Type GetSerializedType(DirectoryInfo directory)
        {
            var typeHintFile = directory.GetFiles().FirstOrDefault(file => file.Name.Equals(TypeHintFileName));
            if (typeHintFile == null)
            {
                // don't run tests with missing typehint file. (some tests will fail without type information - e.g. PrettyPrint tests)
                return null;
            }

            var typeHint = File.ReadAllText(typeHintFile.FullName);
            return Type.GetType(typeHint);
        }

        [Pure]
        private static bool IsTestCaseFile([NotNull] FileInfo file)
        {
            return
                !file.Name.Equals(ExpectedCompactFileName) &&
                !file.Name.Equals(ExpectedPrettyFileName) &&
                !file.Name.Equals(TypeHintFileName);
        }

        [Pure, NotNull]
        private static string GetTestCaseName([NotNull] string rootFolderPath, [NotNull] string testCaseFile)
        {
            return Regex.Replace(TrimFileExtension(testCaseFile), rootFolderPath + @"\\", "");
        }

        [Pure, NotNull]
        private static string TrimFileExtension([NotNull] string fullName)
        {
            return Regex.Replace(fullName, Path.GetExtension(fullName), "");
        }
    }
}