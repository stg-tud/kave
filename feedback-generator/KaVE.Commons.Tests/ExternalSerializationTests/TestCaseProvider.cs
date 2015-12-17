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
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Tests.ExternalSerializationTests
{
    public static class TestCaseProvider
    {
        private const string ExpectedCompactFileName = "expected-compact.json";
        private const string ExpectedPrettyFileName = "expected-formatted.json";
        private const string SettingsFileName = "settings.ini";

        [Pure, NotNull]
        public static IEnumerable<TestCase> GetTestCases(string rootFolderPath)
        {
            var directory = new DirectoryInfo(rootFolderPath);
            return RecursiveGetTestCases(
                directory,
                Regex.Escape(directory.FullName));
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
            if (compactExpectedFile == null)
            {
                return new List<TestCase>();
            }
            var compactExpected = File.ReadAllText(compactExpectedFile.FullName);

            var formattedExpectedFile =
                directory.GetFiles().FirstOrDefault(file => file.Name.Equals(ExpectedPrettyFileName));
            var formattedExpected = formattedExpectedFile != null
                ? File.ReadAllText(formattedExpectedFile.FullName)
                : null;

            return
                GetInputFiles(directory).Select(
                    inputFile => new TestCase(
                        GetTestCaseName(rootFolderPath, inputFile.FullName),
                        GetSerializedType(directory),
                        File.ReadAllText(inputFile.FullName),
                        compactExpected,
                        formattedExpected));
        }

        [Pure, NotNull]
        private static IEnumerable<FileInfo> GetInputFiles(DirectoryInfo directory)
        {
            return directory.GetFiles().Where(IsInputFile);
        }

        [Pure]
        private static bool IsInputFile([NotNull] FileInfo file)
        {
            return
                !file.Name.Equals(ExpectedCompactFileName) &&
                !file.Name.Equals(ExpectedPrettyFileName) &&
                !file.Name.Equals(SettingsFileName);
        }

        [Pure, NotNull]
        private static Type GetSerializedType(DirectoryInfo directory)
        {
            var settingsFile = directory.GetFiles().FirstOrDefault(file => file.Name.Equals(SettingsFileName));
            if (settingsFile == null)
            {
                return typeof (object);
            }

            var settings = TestSettingsReader.ReadSection(settingsFile.FullName, "CSharp");
            return Type.GetType(settings[ExternalTestSetting.SerializedType]) ?? typeof (object);
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