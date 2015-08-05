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

using System.IO;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.CodeCompletion.Impl;
using KaVE.Commons.Utils.IO;
using Moq;
using NUnit.Framework;
using Fix = KaVE.Commons.Tests.Utils.CodeCompletion.Impl.SmilePBNRecommenderFixture;

namespace KaVE.Commons.Tests.Utils.CodeCompletion.Impl
{
    internal class SmilePBNRecommenderStoreTest
    {
        private IIoUtils _io;
        private SmilePBNRecommenderStore _sut;
        private string _basePath;
        private string _tmpPath;

        private bool _shouldZipBeFound;
        private bool _shouldXdslBeFound;


        [SetUp]
        public void SetUp()
        {
            _basePath = "some path -- the value irrelevant, it is only used for mocking";
            _tmpPath = Path.Combine(Path.GetTempPath(), "Test_SmilePBNNetworkStoreTest_TmpPath");

            var fullZipFileName = Path.Combine(_basePath, ZipFileForSomeType());
            var fullXdslFileName = Path.Combine(_tmpPath, XdslFileForSomeType());
            Directory.CreateDirectory(_tmpPath);
            File.WriteAllText(fullXdslFileName, Fix.CreateNetworkAsString());

            _shouldZipBeFound = true;
            _shouldXdslBeFound = true;

            _io = Mock.Of<IIoUtils>();
            Mock.Get(_io).Setup(io => io.DirectoryExists(_basePath)).Returns(true);
            Mock.Get(_io).Setup(io => io.FileExists(fullZipFileName)).Returns(() => _shouldZipBeFound);
            Mock.Get(_io).Setup(io => io.UnzipToTempFolder(fullZipFileName)).Returns(_tmpPath);
            Mock.Get(_io).Setup(io => io.FileExists(fullXdslFileName)).Returns(() => _shouldXdslBeFound);

            _sut = new SmilePBNRecommenderStore(_basePath, _io, new TypePathUtil());
        }

        [TearDown]
        public void TearDown()
        {
            Directory.Delete(_tmpPath, true);
        }

        [Test]
        public void IsAvailable_HappyPath()
        {
            Assert.True(_sut.IsAvailable(SomeType()));
        }

        [Test]
        public void IsAvailable_NoRootFolder()
        {
            _sut = new SmilePBNRecommenderStore("non existing path", _io, new TypePathUtil());
            Assert.False(_sut.IsAvailable(SomeType()));
            // verify non unzip
        }

        [Test]
        public void IsAvailable_NoZipExists()
        {
            _shouldZipBeFound = false;
            Assert.False(_sut.IsAvailable(SomeType()));
            // verify non unzip
        }

        [Test]
        public void Load_HappyPath()
        {
            var a = Fix.CreateNetwork();
            var b = Fix.CreateNetwork();
            Assert.AreEqual(a, b);

            var actual = _sut.Load(SomeType());
            var expected = new SmilePBNRecommender(SomeType(), Fix.CreateNetwork());
            Assert.AreEqual(expected, actual);
        }

        [Test, ExpectedException(typeof (AssertException))]
        public void Load_NoRootFolder()
        {
            _sut = new SmilePBNRecommenderStore("non existing path", _io, new TypePathUtil());
            _sut.Load(SomeType());
        }

        [Test, ExpectedException(typeof (AssertException))]
        public void Load_NoZipExists()
        {
            _shouldZipBeFound = false;
            _sut.Load(SomeType());
        }

        [Test, ExpectedException(typeof (AssertException))]
        public void Load_XdslFileNotFound()
        {
            _shouldXdslBeFound = false;
            _sut.Load(SomeType());
        }

        [Test, ExpectedException(typeof (AssertException))]
        public void Load_ExceptionWhileInstantiating()
        {
            var fullXdslFileName = Path.Combine(_tmpPath, XdslFileForSomeType());
            File.Delete(fullXdslFileName);
            File.WriteAllText(fullXdslFileName, "this cannot be parsed by smile");
            _sut.Load(SomeType());
        }

        #region helper

        private CoReTypeName SomeType()
        {
            return new CoReTypeName("LSomePackage/SomeType");
        }

        private string ZipFileForSomeType()
        {
            return "LSomePackage/SomeType.zip";
        }

        private string XdslFileForSomeType()
        {
            return "LSomePackage_SomeType.xdsl";
        }

        #endregion
    }
}