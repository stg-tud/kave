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
using Ionic.Zip;
using KaVE.Commons.TestUtils;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.IO.Archives;
using KaVE.Commons.Utils.Json;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.IO.Archives
{
    internal class ReadingArchiveTest : FileBasedTestBase
    {
        private const string EmptyString = "EMPTY_STRING";

        private string _zipPath;

        private ReadingArchive _sut;

        [SetUp]
        public void SetUp()
        {
            _zipPath = DirTestRoot + "\\a.zip";
        }

        [TearDown]
        public void TearDown()
        {
            _sut.Dispose();
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void NonExistingZip()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new ReadingArchive(@"C:\does\not\exist.zip");
        }

        [Test]
        public void NonExistingZip_ContainsHint()
        {
            const string missingFile = @"C:\does\not\exist.zip";
            try
            {
                // ReSharper disable once ObjectCreationAsStatement
                new ReadingArchive(missingFile);
                Assert.Fail();
            }
            catch (AssertException e)
            {
                Assert.That(e.Message.Contains(missingFile));
            }
        }

        [Test]
        public void EmptyZip()
        {
            PrepareZip();

            Assert.False(_sut.HasNext());
        }

        [Test]
        public void NonEmptyZip()
        {
            PrepareZip("a", "b");

            Assert.True(_sut.HasNext());
            Assert.AreEqual("a", _sut.GetNext<string>());
            Assert.True(_sut.HasNext());
            Assert.AreEqual("b", _sut.GetNext<string>());
            Assert.False(_sut.HasNext());
        }

        [Test]
        public void NonEmptyZip_FancyContents()
        {
            PrepareZip("a", "b", "\"\"", "null");

            Assert.True(_sut.HasNext());
            Assert.AreEqual("a", _sut.GetNext<string>());
            Assert.True(_sut.HasNext());
            Assert.AreEqual("b", _sut.GetNext<string>());
            Assert.True(_sut.HasNext());
            Assert.AreEqual("\"\"", _sut.GetNext<string>());
            Assert.True(_sut.HasNext());
            Assert.AreEqual("null", _sut.GetNext<string>());
            Assert.False(_sut.HasNext());
        }

        [Test]
        public void NonEmptyZip_WithNull()
        {
            PrepareZip(null, "a", null, "b", null);

            Assert.True(_sut.HasNext());
            Assert.AreEqual("a", _sut.GetNext<string>());
            Assert.True(_sut.HasNext());
            Assert.AreEqual("b", _sut.GetNext<string>());
            Assert.False(_sut.HasNext());
        }

        [Test]
        public void NonEmptyZip_WithEmpty()
        {
            // used as a marker to prevent json serialization into empty string later
            PrepareZip(EmptyString, "a", EmptyString, "b", EmptyString);

            Assert.True(_sut.HasNext());
            Assert.AreEqual("a", _sut.GetNext<string>());
            Assert.True(_sut.HasNext());
            Assert.AreEqual("b", _sut.GetNext<string>());
            Assert.False(_sut.HasNext());
        }

        [Test]
        public void Count_Empty()
        {
            PrepareZip();
            Assert.AreEqual(0, _sut.Count);
        }

        [Test]
        public void Count_1()
        {
            PrepareZip("a");
            Assert.AreEqual(1, _sut.Count);
        }

        [Test]
        public void Count_2()
        {
            PrepareZip("a", "b");
            Assert.AreEqual(2, _sut.Count);
        }

        [Test]
        public void GetAll()
        {
            PrepareZip("a", "b", "c");

            var actual = _sut.GetAll<string>();
            var expected = new List<string> {"a", "b", "c"};
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetAllLazy()
        {
            PrepareZip("a", "b", "c");

            var actual = _sut.GetAllLazy<string>();
            var expected = new List<string> {"a", "b", "c"};
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void GetAllRemovesNullValues()
        {
            PrepareZip("a", null, "b");

            var actual = _sut.GetAll<string>();
            var expected = new List<string> {"a", "b"};
            Assert.AreEqual(expected, actual);
        }

        private void PrepareZip(params string[] entries)
        {
            using (var zipFile = new ZipFile())
            {
                var i = 0;
                foreach (var entry in entries)
                {
                    var fileName = (i++) + ".json";
                    var content = entry == EmptyString ? "" : entry.ToCompactJson();
                    zipFile.AddEntry(fileName, content);
                }
                zipFile.Save(_zipPath);
            }
            _sut = new ReadingArchive(_zipPath);
        }
    }
}