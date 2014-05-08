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
using System.IO;
using System.Net.Http;
using KaVE.Utils.Assertion;
using KaVE.Utils.IO;
using KaVE.VsFeedbackGenerator.Utils;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils
{
    [TestFixture]
    internal class IoUtilsTest
    {
        private IoUtils _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new IoUtils();
        }

        [Test, ExpectedException(typeof (AssertException), ExpectedMessage = "Http-Upload erwartet Http-Adresse")]
        public void ShouldFailOnDifferentScheme()
        {
            _sut.TransferByHttp(new Mock<HttpContent>().Object, new Uri("ftp://www.google.de"), 5);
        }

        // TODO review
        //[Test, Ignore]
        //public void TransferByHttpNotImplementedYet()
        //[Test, ExpectedException(typeof (AssertException), ExpectedMessage = "Server nicht erreichbar")]
        //public void ShouldThrowExceptionOnUnreachableServer()

        [Test]
        public void ShouldReturnExistingTempFileName()
        {
            var fileName = _sut.GetTempFileName();
            Assert.True(File.Exists(fileName));
        }

        [Test]
        public void ShouldNotReturnTempFileNameTwice()
        {
            var a = _sut.GetTempFileName();
            var b = _sut.GetTempFileName();
            Assert.AreNotEqual(a, b);
        }

        [Test]
        public void ShouldCreateDirectory()
        {
            var directory = Path.Combine(Path.GetTempPath(), "MyTestTempDir");

            _sut.CreateDirectory(directory);

            Assert.IsTrue(Directory.Exists(directory));
        }

        [Test]
        public void ShouldDeleteDirectory()
        {
            var directory = GetNonExistentTempFileName();
            Directory.CreateDirectory(directory);
            File.Create(Path.Combine(directory, "file.ext")).Close();

            _sut.DeleteDirectoryWithContent(directory);

            Assert.IsFalse(Directory.Exists(directory));
        }

        [Test]
        public void ShouldCreateFile()
        {
            var path = GetNonExistentTempFileName();

            _sut.CreateFile(path);

            Assert.IsTrue(File.Exists(path));
        }

        [Test]
        public void ShouldDetectIfFileExists()
        {
            var fileName = Path.GetTempFileName();
            Assert.True(_sut.FileExists(fileName));
            File.Delete(fileName);
            Assert.False(_sut.FileExists(fileName));
        }

        [Test]
        public void ShouldCopyFile()
        {
            var a = Path.GetTempFileName();
            var b = Path.GetTempFileName();
            File.Delete(b);

            const string expected = "blubb";
            File.WriteAllText(a, expected);
            _sut.CopyFile(a, b);
            Assert.True(File.Exists(a));
            Assert.True(File.Exists(b));

            var actual = File.ReadAllText(b);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldReadContentsOfFile()
        {
            var a = Path.GetTempFileName();
            const string expected = "blubb";
            File.WriteAllText(a, expected);
            var actual = _sut.ReadFile(a);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldDeleteFile()
        {
            var filePath = Path.GetTempFileName();
            _sut.DeleteFile(filePath);
            Assert.IsFalse(File.Exists(filePath));
        }

        [Test]
        public void ShouldOpenFileForRead()
        {
            var file = IoTestHelper.GetTempFileName();

            var stream = _sut.OpenFile(file, FileMode.Open, FileAccess.Read);

            Assert.IsTrue(stream.CanRead);
            stream.Close();
        }

        [Test]
        public void ShouldOpenFileForWrite()
        {
            var file = IoTestHelper.GetTempFileName();

            var stream = _sut.OpenFile(file, FileMode.Open, FileAccess.Write);

            Assert.IsTrue(stream.CanWrite);
            stream.Close();
        }

        [Test]
        public void ShouldMoveFile()
        {
            var source = IoTestHelper.GetTempFileName();
            var target = GetNonExistentTempFileName();

            _sut.MoveFile(source, target);

            Assert.IsTrue(File.Exists(target));
            Assert.IsFalse(File.Exists(source));
        }

        private string GetNonExistentTempFileName()
        {
            return Path.Combine(IoTestHelper.GetTempDirectoryName(), "ArbitraryFileName" + DateTime.Now.Ticks);
        }
    }
}