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

using System.Collections.Generic;
using System.IO;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.CodeCompletion.Impl;
using KaVE.Commons.Utils.CodeCompletion.Stores;
using KaVE.Commons.Utils.IO;
using KaVE.Commons.Utils.Json;
using Moq;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils
{
    internal class UsageModelsIndexFileGeneratorTest
    {
        private const string TestPath = @"C:\kave-models";

        private const string File1 = @"File1.zip";
        private const string File2 = @"File2.zip";
        private const string File3 = @"File3.zip";

        private static readonly string[] TestFiles =
        {
            Path.Combine(TestPath, File1),
            Path.Combine(TestPath, File2),
            Path.Combine(TestPath, File3)
        };

        private static readonly CoReTypeName TestType1 = new CoReTypeName("LType1");
        private static readonly CoReTypeName TestType2 = new CoReTypeName("LType2");
        private static readonly CoReTypeName TestType3 = new CoReTypeName("LType3");
        private const int TestVersion1 = 1;
        private const int TestVersion2 = 2;
        private const int TestVersion3 = 3;

        private IIoUtils _testIoUtils;
        private ITypePathUtil _testTypePathUtil;

        [SetUp]
        public void Setup()
        {
            _testIoUtils = Mock.Of<IIoUtils>();
            _testTypePathUtil = Mock.Of<ITypePathUtil>();

            Mock.Get(_testIoUtils).Setup(io => io.GetFilesRecursive(TestPath, "*.zip")).Returns(TestFiles);

            Mock.Get(_testTypePathUtil).Setup(typePath => typePath.GetTypeName(File1)).Returns(TestType1);
            Mock.Get(_testTypePathUtil).Setup(typePath => typePath.GetTypeName(File2)).Returns(TestType2);
            Mock.Get(_testTypePathUtil).Setup(typePath => typePath.GetTypeName(File3)).Returns(TestType3);
            Mock.Get(_testTypePathUtil).Setup(typePath => typePath.GetVersionNumber(File1)).Returns(TestVersion1);
            Mock.Get(_testTypePathUtil).Setup(typePath => typePath.GetVersionNumber(File2)).Returns(TestVersion2);
            Mock.Get(_testTypePathUtil).Setup(typePath => typePath.GetVersionNumber(File3)).Returns(TestVersion3);
        }

        [Test]
        public void ShouldGenerateContent()
        {
            var expected = new List<UsageModelDescriptor>
            {
                new UsageModelDescriptor(TestType1, TestVersion1),
                new UsageModelDescriptor(TestType2, TestVersion2),
                new UsageModelDescriptor(TestType3, TestVersion3)
            }.ToCompactJson();

            var actualContent = UsageModelsIndexFileGenerator.GenerateIndexFileContent(
                TestPath,
                _testIoUtils,
                _testTypePathUtil);

            Assert.AreEqual(expected, actualContent);
        }
    }
}