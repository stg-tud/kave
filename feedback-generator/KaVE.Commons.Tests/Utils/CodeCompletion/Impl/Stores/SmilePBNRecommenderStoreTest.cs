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
using KaVE.Commons.Utils.CodeCompletion.Impl.Stores;
using KaVE.Commons.Utils.CodeCompletion.Stores;
using KaVE.Commons.Utils.IO;
using Moq;
using NUnit.Framework;
using Fix = KaVE.Commons.Tests.Utils.CodeCompletion.Impl.SmilePBNRecommenderFixture;

namespace KaVE.Commons.Tests.Utils.CodeCompletion.Impl.Stores
{
    internal class SmilePBNRecommenderStoreTest
    {
        private IIoUtils _io;
        private SmilePBNRecommenderStore _sut;
        private string _basePath;
        private string _tmpPath;

        private static bool _shouldZipBeFound;
        private static bool _shouldXdslBeFound;
        private IRemotePBNRecommenderStore _remoteStore;

        [SetUp]
        public void SetUp()
        {
            _basePath = "some path -- the value irrelevant, it is only used for mocking";
            _tmpPath = Path.Combine(Path.GetTempPath(), "Test_SmilePBNNetworkStoreTest_TmpPath");

            var fullZipFileName = Path.Combine(_basePath, ZipFileForSomeType);
            var fullXdslFileName = Path.Combine(_tmpPath, XdslFileForSomeType);
            Directory.CreateDirectory(_tmpPath);
            File.WriteAllText(fullXdslFileName, Fix.CreateNetworkAsString());

            _shouldZipBeFound = true;
            _shouldXdslBeFound = true;

            _io = Mock.Of<IIoUtils>();
            Mock.Get(_io).Setup(io => io.DirectoryExists(_basePath)).Returns(true);
            Mock.Get(_io).Setup(io => io.UnzipToTempFolder(fullZipFileName)).Returns(_tmpPath);
            Mock.Get(_io).Setup(io => io.FileExists(fullXdslFileName)).Returns(() => _shouldXdslBeFound);
            Mock.Get(_io).Setup(io => io.GetFilesRecursive(_basePath, "*.zip")).Returns(() => TestTypeFiles);

            _remoteStore = Mock.Of<IRemotePBNRecommenderStore>();

            _sut = new SmilePBNRecommenderStore(_basePath, _io, new TypePathUtil(), _remoteStore);
        }

        [TearDown]
        public void TearDown()
        {
            Directory.Delete(_tmpPath, true);
        }

        [Test]
        public void IsAvailable_HappyPath()
        {
            Assert.True(_sut.IsAvailable(SomeType));
        }

        [Test]
        public void IsAvailable_NoRootFolder()
        {
            _sut = new SmilePBNRecommenderStore("non existing path", _io, new TypePathUtil(), _remoteStore);
            Assert.False(_sut.IsAvailable(SomeType));
            // verify non unzip
        }

        [Test]
        public void IsAvailable_NoZipExists()
        {
            _shouldZipBeFound = false;
            _sut.ReloadAvailableModels();
            Assert.False(_sut.IsAvailable(SomeType));
            // verify non unzip
        }

        [Test]
        public void Load_HappyPath()
        {
            var a = Fix.CreateNetwork();
            var b = Fix.CreateNetwork();
            Assert.AreEqual(a, b);

            var actual = _sut.Load(SomeType);
            var expected = new SmilePBNRecommender(SomeType, Fix.CreateNetwork());
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Load_NoRootFolder()
        {
            _sut = new SmilePBNRecommenderStore("non existing path", _io, new TypePathUtil(), _remoteStore);
            Assert.IsNull(_sut.Load(SomeType));
        }

        [Test]
        public void Load_NoZipExists()
        {
            _shouldZipBeFound = false;
            _sut.ReloadAvailableModels();
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
            var expected = new List<UsageModelDescriptor>
            {
                new UsageModelDescriptor(SomeType, 1),
                new UsageModelDescriptor(SomeOtherType, 2)
            };
            CollectionAssert.AreEquivalent(expected, _sut.GetAvailableModels());
        }

        [Test]
        public void ShouldUseDefaultValueIfVersionCannotBeParsed()
        {
            Mock.Get(_io)
                .Setup(io => io.GetFilesRecursive(_basePath, "*.zip"))
                .Returns(new[] {"LSomePackage/SomeType.zip"});
            _sut.ReloadAvailableModels();

            var expected = new List<UsageModelDescriptor>
            {
                new UsageModelDescriptor(SomeType, 0)
            };
            CollectionAssert.AreEquivalent(expected, _sut.GetAvailableModels());
        }

        [Test]
        public void ShouldDeleteModelFileOnRemove()
        {
            _sut.Remove(SomeType);
            Mock.Get(_io).Verify(io => io.DeleteFile(Path.Combine(_basePath, ZipFileForSomeType)), Times.Once);
        }

        [Test]
        public void ShouldLoadModelsOnReload()
        {
            Mock.Get(_io)
                .Setup(io => io.GetFilesRecursive(_basePath, "*.zip"))
                .Returns(new[] {"LSomePackage/SomeType.5.zip"});

            _sut.ReloadAvailableModels();

            CollectionAssert.AreEquivalent(
                new[] {new UsageModelDescriptor(new CoReTypeName("LSomePackage/SomeType"), 5)},
                _sut.GetAvailableModels());
        }

        [Test]
        public void ShouldTriggerRemoteLoadIfEnabled()
        {
            _sut.EnableAutoRemoteLoad = true;
            var testType = new CoReTypeName("LNotLocallyAvailable");

            _sut.Load(testType);

            Mock.Get(_remoteStore).Verify(remoteStore => remoteStore.Load(testType), Times.Once);
        }

        [Test]
        public void ShouldNotTriggerRemoteLoadIfDisabled()
        {
            _sut.EnableAutoRemoteLoad = false;
            var testType = new CoReTypeName("LNotLocallyAvailable");

            _sut.Load(testType);

            Mock.Get(_remoteStore).Verify(remoteStore => remoteStore.Load(testType), Times.Never);
        }

        #region helper

        private static string[] TestTypeFiles
        {
            get
            {
                return _shouldZipBeFound
                    ? new[]
                    {
                        ZipFileForSomeType,
                        "LSomePackage/SomeOtherType.2.zip"
                    }
                    : new string[0];
            }
        }

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

        #endregion
    }
}