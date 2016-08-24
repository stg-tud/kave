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
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.IO.Archives;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.IO.Archives
{
    internal class WritingArchiveTest
    {
        private string _zipPath;

        [SetUp]
        public void SetUp()
        {
            _zipPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(_zipPath);
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void DirectoryHastoExist()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new WritingArchive(@"c:\does\not\exist.zip");
        }

        [Test]
        public void NonExistingZip_ContainsHint()
        {
            const string fileInUnknownDir = @"C:\does\not\exist.zip";
            try
            {
                // ReSharper disable once ObjectCreationAsStatement
                new ReadingArchive(fileInUnknownDir);
                Assert.Fail();
            }
            catch (AssertException e)
            {
                Assert.That(e.Message.Contains(fileInUnknownDir));
            }
        }

        [Test]
        public void WriteEmptyDoesNotCreateFile()
        {
            Write();
            Assert.False(File.Exists(_zipPath));
        }

        [Test]
        public void WritingOnlyNullDoesNotCreateFile()
        {
            Write(new string[] {null});
            Assert.False(File.Exists(_zipPath));
        }

        [Test]
        public void WriteSomething()
        {
            var expected = new[] {"a", "b", "c"};
            Write(expected);
            var actual = ReadZip();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldCountAdditions()
        {
            var wa = new WritingArchive(_zipPath);
            Assert.AreEqual(0, wa.NumItemsAdded);
            wa.Add("a");
            Assert.AreEqual(1, wa.NumItemsAdded);
            wa.Add((string) null);
            Assert.AreEqual(1, wa.NumItemsAdded);
            wa.Add("a");
            Assert.AreEqual(2, wa.NumItemsAdded);
            wa.Dispose();
            Assert.AreEqual(2, wa.NumItemsAdded);
        }

        [Test]
        public void WriteNullsAreIgnored()
        {
            var input = new[] {null, "a", null, "b", null};
            Write(input);
            var actual = ReadZip();
            var expected = new[] {"a", "b"};
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void WriteAll()
        {
            var expected = new[] {"a", "b", "c"};
            using (var sut = new WritingArchive(_zipPath))
            {
                sut.AddAll(expected);
            }
            var actual = ReadZip();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void NothingHappensOnSecondDispose()
        {
            var sut = new WritingArchive(_zipPath);
            Assert.False(File.Exists(_zipPath));
            sut.Add("x");
            Assert.True(File.Exists(_zipPath));
            sut.Dispose();
            Assert.True(File.Exists(_zipPath));

            File.Delete(_zipPath);
            sut.Dispose();
            Assert.False(File.Exists(_zipPath));
        }

        private void Write(params string[] entries)
        {
            using (var sut = new WritingArchive(_zipPath))
            {
                foreach (var entry in entries)
                {
                    sut.Add(entry);
                }
            }
        }

        private IEnumerable<string> ReadZip()
        {
            using (var ra = new ReadingArchive(_zipPath))
            {
                return ra.GetAll<string>();
            }
        }
    }
}