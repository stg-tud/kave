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
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.IO.Archives;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.IO.Archives
{
    // ReSharper disable ObjectCreationAsStatement
    internal class ZipFolderLRUCacheTest
    {
        private string _root;
        private ZipFolderLRUCache<string> _sut;

        [SetUp]
        public void SetUp()
        {
            _root = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(_root);
            _sut = new ZipFolderLRUCache<string>(_root, 2);
        }

        [TearDown]
        public void TearDown()
        {
            Directory.Delete(_root, true);
        }

        [Test]
        public void SizeAndCache()
        {
            Assert.AreEqual(0, _sut.Size);
            Assert.False(_sut.IsCached("a"));

            _sut.GetArchive("a");

            Assert.AreEqual(1, _sut.Size);
            Assert.True(_sut.IsCached("a"));
        }

        [Test]
        public void FilesAreCreatedInCorrectSubfolders()
        {
            _sut.GetArchive("a");
            _sut.GetArchive("b/b");
            _sut.GetArchive("c/c/c");
            _sut.GetArchive("d/d/d/d");

            _sut.Dispose();

            Assert.True(Directory.Exists(_root));
            Assert.True(File.Exists(Path.Combine(_root, "a/.zipfolder")));
            Assert.True(File.Exists(Path.Combine(_root, "b/b/.zipfolder")));
            Assert.True(File.Exists(Path.Combine(_root, "c/c/c/.zipfolder")));
            Assert.True(File.Exists(Path.Combine(_root, "d/d/d/d/.zipfolder")));
        }

        [Test]
        public void DoubleSlashIsNotAnIssue()
        {
            _sut.GetArchive("La//a");

            _sut.Dispose();

            Assert.True(Directory.Exists(_root));
            Assert.True(File.Exists(Path.Combine(_root, "La/a/.zipfolder")));
        }

        [Test]
        public void KeyIsPassedAsMetaDataWithoutReplacement()
        {
            const string key = "La/A.1/_!";

            _sut.GetArchive(key);
            _sut.Dispose();

            var metaFile = Path.Combine(_root, "La/A/1/__/.zipfolder");
            var actual = File.ReadAllText(metaFile);
            Assert.AreEqual(key, actual);
        }

        [Test]
        public void ReplacementInKeysWorks()
        {
            var a = @"aA1_!";
            var e = @"aA1__\0.zip";
            using (var wa = _sut.GetArchive(a))
            {
                wa.Add("something");
            }

            Assert.True(Directory.Exists(_root));
            Assert.True(File.Exists(Path.Combine(_root, e)));
        }

        [Test]
        public void CacheDoesNotGrowLargerThanCapacity()
        {
            _sut.GetArchive("a");
            _sut.GetArchive("b");
            _sut.GetArchive("c");

            Assert.AreEqual(2, _sut.Size);
        }

        [Test]
        public void LeastRecentlyUsedKeyIsRemoved()
        {
            _sut.GetArchive("a");
            _sut.GetArchive("b");
            _sut.GetArchive("c");

            Assert.False(_sut.IsCached("a"));
            Assert.True(_sut.IsCached("b"));
            Assert.True(_sut.IsCached("c"));
        }

        [Test]
        public void RefreshingWorks()
        {
            _sut.GetArchive("a");
            _sut.GetArchive("b");
            _sut.GetArchive("a");
            _sut.GetArchive("c");

            Assert.False(_sut.IsCached("b"));
            Assert.True(_sut.IsCached("a"));
            Assert.True(_sut.IsCached("c"));
        }

        [Test]
        public void CacheRemoveClosesOpenArchive()
        {
            var expectedFileName = Path.Combine(_root, "a/0.zip");

            var wa = _sut.GetArchive("a");
            wa.Add("something");
            _sut.GetArchive("b");

            Assert.False(File.Exists(expectedFileName));
            _sut.GetArchive("c");
            Assert.True(File.Exists(expectedFileName));
        }

        [Test]
        public void DisposeClosesAllOpenArchives()
        {
            var wa = _sut.GetArchive("a");
            wa.Add("something");

            _sut.Dispose();

            Assert.False(_sut.IsCached("a"));
            Assert.AreEqual(0, _sut.Size);
            Assert.True(File.Exists(Path.Combine(_root, "a/0.zip")));
        }


        [Test, ExpectedException(typeof (AssertException))]
        public void DirectoryHasToExist()
        {
            new ZipFolderLRUCache<string>(@"c:\does\not\exist\", 10);
        }

        [Test, ExpectedException(typeof (AssertException))]
        public void DirectoryMustNotBeAFile()
        {
            new ZipFolderLRUCache<string>(@"c:\Windows\notepad.exe", 10);
        }

        [Test, ExpectedException(typeof (AssertException))]
        public void CapacityMustBeLargerThanZero()
        {
            new ZipFolderLRUCache<string>(Path.GetTempPath(), 0);
        }
    }
}