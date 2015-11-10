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
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils.CodeCompletion.Impl;
using KaVE.Commons.Utils.CodeCompletion.Impl.Stores.UsageModelSources;
using KaVE.Commons.Utils.CodeCompletion.Stores;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.IO;
using KaVE.Commons.Utils.Json;
using Moq;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.CodeCompletion.Impl.Stores.UsageModelSources
{
    internal class FilePathUsageModelsSourceTest
    {
        private const string BasePath = @"C:\";

        private static IEnumerable<UsageModelDescriptor> ExpectedModels
        {
            get
            {
                return Lists.NewList(
                    new UsageModelDescriptor(new CoReTypeName("LSystem\\Char"), 1),
                    new UsageModelDescriptor(new CoReTypeName("LSystem\\String"), 1));
            }
        }

        private FilePathUsageModelsSource _uut;
        private IIoUtils _testIoUtil;

        [SetUp]
        public void Setup()
        {
            _testIoUtil = Mock.Of<IIoUtils>();
            Mock.Get(_testIoUtil)
                .Setup(io => io.FileExists(Path.Combine(BasePath, @"LSystem\Char.1.zip")))
                .Returns(true);

            SetIndexFileContent(ExpectedModels);

            _uut = new FilePathUsageModelsSource(_testIoUtil, new TypePathUtil(), new Uri(BasePath));
        }

        [Test]
        public void ShouldProvideModelsFromLocalPath()
        {
            CollectionAssert.AreEquivalent(ExpectedModels, _uut.GetUsageModels());
        }

        [Test]
        public void ShouldBeEmptyIfPathIsInvalid()
        {
            _uut = new FilePathUsageModelsSource(_testIoUtil, new TypePathUtil(), new Uri(@"file://notvalid"));

            CollectionAssert.IsEmpty(_uut.GetUsageModels());
        }

        [Test]
        public void CreatesFileOnLoad()
        {
            _uut.Load(new UsageModelDescriptor(new CoReTypeName("LSystem\\Char"), 1), @"D:\");
            Mock.Get(_testIoUtil).Verify(ioUtil => ioUtil.CreateFile(@"D:\LSystem\Char.1.zip"));
        }

        [Test]
        public void ShouldLoadModelToPath()
        {
            _uut.Load(new UsageModelDescriptor(new CoReTypeName("LSystem\\Char"), 1), @"D:\");
            Mock.Get(_testIoUtil)
                .Verify(
                    ioUtil => ioUtil.CopyFile(Path.Combine(BasePath, @"LSystem\Char.1.zip"), @"D:\LSystem\Char.1.zip"));
        }

        [Test]
        public void ShouldLoadWithoutVersionNumber()
        {
            Mock.Get(_testIoUtil)
                .Setup(io => io.FileExists(Path.Combine(BasePath, @"LSystem\Char.1.zip")))
                .Returns(false);

            _uut.Load(new UsageModelDescriptor(new CoReTypeName("LSystem\\Char"), 1), @"D:\");
            Mock.Get(_testIoUtil)
                .Verify(
                    ioUtil => ioUtil.CopyFile(Path.Combine(BasePath, @"LSystem\Char.zip"), @"D:\LSystem\Char.1.zip"));
        }

        private void SetIndexFileContent(IEnumerable<UsageModelDescriptor> content)
        {
            Mock.Get(_testIoUtil)
                .Setup(io => io.ReadZippedFile(Path.Combine(BasePath, "index.json.gz")))
                .Returns(content.ToCompactJson());
        }
    }
}