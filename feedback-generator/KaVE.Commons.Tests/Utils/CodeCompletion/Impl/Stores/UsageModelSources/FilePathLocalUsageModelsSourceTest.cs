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
using KaVE.Commons.Utils.CodeCompletion.Impl;
using KaVE.Commons.Utils.CodeCompletion.Impl.Stores.UsageModelSources;
using KaVE.Commons.Utils.CodeCompletion.Stores;
using KaVE.Commons.Utils.IO;
using KaVE.Commons.Utils.Json;
using Moq;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.CodeCompletion.Impl.Stores.UsageModelSources
{
    internal class FilePathLocalUsageModelsSourceTest
    {
        private FilePathLocalUsageModelsSource _sut;
        private IIoUtils _io;
        private string _basePath;
        private string _tmpPath;

        private static bool _shouldZipBeFound;
        private static bool _shouldXdslBeFound;

        [SetUp]
        public void SetUp()
        {
            _basePath = "some path -- the value irrelevant, it is only used for mocking";
            _tmpPath = Path.Combine(Path.GetTempPath(), "Test_SmilePBNNetworkStoreTest_TmpPath");

            var fullZipFileName = Path.Combine(_basePath, ZipFileForSomeType);
            var fullXdslFileName = Path.Combine(_tmpPath, XdslFileForSomeType);
            Directory.CreateDirectory(_tmpPath);
            File.WriteAllText(fullXdslFileName, SmilePBNRecommenderFixture.CreateNetworkAsString());

            _shouldZipBeFound = true;
            _shouldXdslBeFound = true;

            _io = Mock.Of<IIoUtils>();
            Mock.Get(_io).Setup(io => io.DirectoryExists(_basePath)).Returns(true);
            Mock.Get(_io).Setup(io => io.UnzipToTempFolder(fullZipFileName)).Returns(_tmpPath);
            Mock.Get(_io).Setup(io => io.FileExists(fullZipFileName)).Returns(() => _shouldZipBeFound);
            Mock.Get(_io).Setup(io => io.FileExists(fullXdslFileName)).Returns(() => _shouldXdslBeFound);

            SetAvailableModels(SomeAvailableModels);

            _sut = new FilePathLocalUsageModelsSource(_basePath, _io, new TypePathUtil());
        }

        [TearDown]
        public void TearDown()
        {
            Directory.Delete(_tmpPath, true);
        }

        [Test]
        public void Load_HappyPath()
        {
            var actual = _sut.Load(SomeType);
            var expected = new SmilePBNRecommender(SomeType, SmilePBNRecommenderFixture.CreateNetwork());
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Load_NoRootFolder()
        {
            _sut = new FilePathLocalUsageModelsSource("non existing path", _io, new TypePathUtil());
            Assert.IsNull(_sut.Load(SomeType));
        }

        [Test]
        public void Load_NoZipExists()
        {
            _shouldZipBeFound = false;
            Assert.IsNull(_sut.Load(SomeType));
        }

        [Test]
        public void Load_XdslFileNotFound()
        {
            _shouldXdslBeFound = false;
            Assert.IsNull(_sut.Load(SomeType));
        }

        [Test]
        public void Load_ExceptionWhileInstantiating()
        {
            var fullXdslFileName = Path.Combine(_tmpPath, XdslFileForSomeType);
            File.Delete(fullXdslFileName);
            File.WriteAllText(fullXdslFileName, "this cannot be parsed by smile");
            Assert.IsNull(_sut.Load(SomeType));
        }

        [Test]
        public void ShouldProvideAvailableModels()
        {
            CollectionAssert.AreEquivalent(SomeAvailableModels, _sut.GetUsageModels());
        }

        [Test]
        public void ShouldDeleteModelFileOnRemove()
        {
            _sut.Remove(SomeType);
            Mock.Get(_io).Verify(io => io.DeleteFile(Path.Combine(_basePath, ZipFileForSomeType)), Times.Once);
        }

        [Test]
        public void ShouldRemoveModelFromIndexOnRemove()
        {
            _sut.Remove(SomeType);

            var expectedContent =
                new List<UsageModelDescriptor> {new UsageModelDescriptor(SomeOtherType, 2)}.ToCompactJson();
            Mock.Get(_io).Verify(io => io.WriteZippedFile(expectedContent, Path.Combine(_basePath, "index.json.gz")));
        }

        #region helper

        private static CoReTypeName SomeType
        {
            get { return new CoReTypeName("LSomePackage/SomeType"); }
        }

        private static CoReTypeName SomeOtherType
        {
            get { return new CoReTypeName("LSomePackage/SomeOtherType"); }
        }

        private static string ZipFileForSomeType
        {
            get { return "LSomePackage/SomeType.1.zip"; }
        }

        private static string XdslFileForSomeType
        {
            get { return "LSomePackage_SomeType.xdsl"; }
        }

        private static IEnumerable<UsageModelDescriptor> SomeAvailableModels
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

        private void SetAvailableModels(IEnumerable<UsageModelDescriptor> usageModelDescriptors)
        {
            Mock.Get(_io)
                .Setup(io => io.ReadZippedFile(Path.Combine(_basePath, "index.json.gz")))
                .Returns(usageModelDescriptors.ToCompactJson());
        }

        #endregion
    }
}