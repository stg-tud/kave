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
    internal class ZipFolderTest
    {
        private string _root;
        private ZipFolder _sut;

        [SetUp]
        public void SetUp()
        {
            _root = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(_root);
            _sut = new ZipFolder(_root);
        }

        [TearDown]
        public void TearDown()
        {
            Directory.Delete(_root, true);
        }

        [Test]
        public void FoldersAreMarkedOnCreationOfArchive()
        {
            var markerFile = Path.Combine(_root, ZipFolder.MarkerFileName);
            Assert.False(File.Exists(markerFile));

            _sut.CreateNewArchive();

            Assert.True(File.Exists(markerFile));
        }

        [Test]
        public void FolderMarkersContainMetaData()
        {
            _sut = new ZipFolder(_root, "xyz");
            _sut.CreateNewArchive();

            var actual = File.ReadAllText(Path.Combine(_root, ZipFolder.MarkerFileName));
            Assert.AreEqual("xyz", actual);
        }

        [Test]
        public void ArchivesAreCreatedOnRequest()
        {
            Assert.AreEqual(0, _sut.NumArchives());

            using (var wa = _sut.CreateNewArchive())
            {
                wa.Add("a");
                wa.Add("b");
            }

            Assert.AreEqual(1, _sut.NumArchives());

            using (var wa = _sut.CreateNewArchive())
            {
                wa.Add("c");
                wa.Add("d");
            }

            Assert.AreEqual(2, _sut.NumArchives());
        }

        [Test]
        public void NoFilesAreOverwritten()
        {
            using (File.Create(Path.Combine(_root, "0.zip"))) {}
            using (File.Create(Path.Combine(_root, "1.zip"))) {}

            Assert.AreEqual(2, _sut.NumArchives());

            using (var wa = _sut.CreateNewArchive())
            {
                wa.Add("a");
            }

            Assert.AreEqual(3, _sut.NumArchives());
        }

        [Test]
        public void CorrectContentIsWrittenAndRead()
        {
            using (var wa = _sut.CreateNewArchive())
            {
                wa.Add("a");
                wa.Add("b");
            }
            using (var wa = _sut.CreateNewArchive())
            {
                wa.Add("c");
                wa.Add("d");
            }

            var ras = _sut.OpenAll();
            Assert.AreEqual(2, ras.Count);

            foreach (var ra in ras)
            {
                var entries = ra.GetAll<string>();

                Assert.AreEqual(2, entries.Count);

                if (entries.Contains("a"))
                {
                    Assert.True(entries.Contains("b"));
                }
                else
                {
                    Assert.True(entries.Contains("c"));
                    Assert.True(entries.Contains("d"));
                }

                ra.Dispose();
            }
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void DirectoryHasToExist()
        {
            new ZipFolder(@"c:\does\not\exist\");
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void DirectoryMustNotBeAFile()
        {
            new ZipFolder(@"c:\Windows\notepad.exe");
        }
    }
}