﻿/*
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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace KaVE.Commons.TestUtils.Utils
{
    public static class ExternalTestCaseProvider
    {
        private const string FileNameExpected = "expected.json";

        public static IEnumerable<TestCase> GetTestCases(string rootFolderPath)
        {
            return RecursiveGetTestCases(new DirectoryInfo(rootFolderPath), rootFolderPath);
        }

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

        private static IEnumerable<TestCase> GetTestCasesInCurrentFolder(DirectoryInfo folderDirectory,
            string rootFolderPath)
        {
            var expectedFile = folderDirectory.GetFiles().FirstOrDefault(file => file.Name.Equals(FileNameExpected));
            if (expectedFile == null)
            {
                return new List<TestCase>();
            }

            var inputFiles = folderDirectory.GetFiles().Where(file => !file.Name.Equals(FileNameExpected));
            return
                inputFiles.Select(
                    inputFile =>
                        new TestCase(
                            GetTestCaseName(rootFolderPath, inputFile),
                            File.ReadAllText(inputFile.FullName),
                            File.ReadAllText(expectedFile.FullName)));
        }

        private static string GetTestCaseName(string rootFolderPath, FileSystemInfo inputFile)
        {
            var testCaseName = Regex.Replace(inputFile.FullName, rootFolderPath, "").TrimStart('\\');
            var extension = Path.GetExtension(testCaseName);
            return Regex.Replace(testCaseName, extension, "");
        }
    }

    public class TestCase
    {
        public readonly string Name;
        public readonly string Input;
        public readonly string Expected;

        public TestCase(string name, string input, string expected)
        {
            Expected = expected;
            Input = input;
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}