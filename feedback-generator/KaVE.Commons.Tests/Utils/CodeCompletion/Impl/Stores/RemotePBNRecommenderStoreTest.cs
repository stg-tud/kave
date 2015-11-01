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
using KaVE.Commons.Utils.CodeCompletion.Impl.Stores;
using KaVE.Commons.Utils.CodeCompletion.Stores;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.IO;
using Moq;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.CodeCompletion.Impl.Stores
{
    internal class RemotePBNRecommenderStoreTest
    {
        #region helper

        private const string LocalPath = @"C:\";

        private static CoReTypeName SomeType
        {
            get { return new CoReTypeName("LSomePackage/SomeType"); }
        }

        private static CoReTypeName SomeOtherType
        {
            get { return new CoReTypeName("LSomePackage/SomeOtherType"); }
        }

        private static IEnumerable<UsageModelDescriptor> AvailableModels
        {
            get
            {
                return new List<UsageModelDescriptor>
                {
                    new UsageModelDescriptor(SomeType, 1),
                    new UsageModelDescriptor(SomeOtherType, 2)
                };
            }
        }

        #endregion

        private IUsageModelsSource _testSource;
        private RemotePBNRecommenderStore _sut;

        [SetUp]
        public void SetUp()
        {
            _testSource = Mock.Of<IUsageModelsSource>();
            Mock.Get(_testSource).Setup(source => source.GetUsageModels()).Returns(AvailableModels);

            _sut = new RemotePBNRecommenderStore(_testSource, LocalPath);
        }

        [Test]
        public void IsAvailable_HappyPath()
        {
            Assert.IsTrue(_sut.IsAvailable(SomeType));
        }

        [Test]
        public void IsAvailable_NotAvailable()
        {
            Assert.IsFalse(_sut.IsAvailable(new CoReTypeName("LNotAvailable")));
        }

        [Test]
        public void Load_HappyPath()
        {
            _sut.Load(SomeType);
            Mock.Get(_testSource)
                .Verify(
                    source =>
                        source.Load(
                            It.Is<UsageModelDescriptor>(
                                model => Equals(SomeType, model.TypeName) && Equals(1, model.Version)),
                            LocalPath));
        }

        [Test]
        public void LoadingNonAvailableTypesShouldDoNothing()
        {
            _sut.Load(new CoReTypeName("LNotAvailable"));
            Mock.Get(_testSource)
                .Verify(
                    testSource => testSource.Load(It.IsAny<UsageModelDescriptor>(), It.IsAny<string>()),
                    Times.Never);
        }

        [Test]
        public void ShouldProvideAvailableModels()
        {
            Assert.AreEqual(AvailableModels, _sut.GetAvailableModels());
        }

        [Test]
        public void ShouldLoadAllModelsOnLoadAll()
        {
            _sut.LoadAll();
            foreach (var testModel in AvailableModels)
            {
                Mock.Get(_testSource).Verify(testSource => testSource.Load(testModel, It.IsAny<string>()), Times.Once);
            }
        }

        [Test]
        public void ShouldUpdateAvailableModelsOnReload()
        {
            var newAvailableModels = new List<UsageModelDescriptor>
            {
                new UsageModelDescriptor(new CoReTypeName("LNewModel"), 5)
            };
            Mock.Get(_testSource).Setup(source => source.GetUsageModels()).Returns(newAvailableModels);

            _sut.ReloadAvailableModels();

            CollectionAssert.AreEquivalent(newAvailableModels, _sut.GetAvailableModels());
        }
    }

    internal class LocalUsageModelsSourceTest
    {
        private const string BasePath = @"C:\";

        private static string[] TestFiles
        {
            get
            {
                return new[]
                {
                    Path.Combine(BasePath, @"LSystem\Char.1.zip"),
                    Path.Combine(BasePath, @"LSystem\String.1.zip")
                };
            }
        }

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
            SetModelFileExists(true);
            Mock.Get(_testIoUtil).Setup(io => io.GetFilesRecursive(BasePath, "*.zip")).Returns(TestFiles);

            _uut = new FilePathUsageModelsSource(_testIoUtil, new TypePathUtil()) {Source = new Uri(BasePath)};
        }

        [Test]
        public void ShouldProvideModelsFromLocalPath()
        {
            CollectionAssert.AreEquivalent(ExpectedModels, _uut.GetUsageModels());
        }

        [Test]
        public void ShouldBeEmptyIfPathIsInvalid()
        {
            _uut = new FilePathUsageModelsSource(_testIoUtil, new TypePathUtil()) {Source = new Uri(@"file://notvalid")};

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
            SetModelFileExists(false);

            _uut.Load(new UsageModelDescriptor(new CoReTypeName("LSystem\\Char"), 1), @"D:\");
            Mock.Get(_testIoUtil)
                .Verify(
                    ioUtil => ioUtil.CopyFile(Path.Combine(BasePath, @"LSystem\Char.zip"), @"D:\LSystem\Char.1.zip"));
        }

        private void SetModelFileExists(bool value)
        {
            Mock.Get(_testIoUtil)
                .Setup(io => io.FileExists(Path.Combine(BasePath, @"LSystem\Char.1.zip")))
                .Returns(value);
        }
    }
}