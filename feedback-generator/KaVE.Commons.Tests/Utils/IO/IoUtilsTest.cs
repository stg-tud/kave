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
using System.Linq;
using System.Net.Http;
using System.Text;
using Ionic.Zip;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.IO;
using Moq;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.IO
{
    internal class IoUtilsTest
    {
        private const string Extension = "ext";
        private IoUtils _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new IoUtils();
        }

        [Test, ExpectedException(typeof (NotImplementedException))]
        public void TransferByHttpIsNotImplemented()
        {
            _sut.TransferByHttp(new Mock<HttpContent>().Object, new Uri("http://any"));
        }

        [Test]
        public void ShouldReturnExistingTempFileName()
        {
            var fileName = _sut.GetTempFileName();
            Assert.True(File.Exists(fileName));
        }

        [Test]
        public void ShouldReturnExistingTempFileNameWithExtension()
        {
            var fileName = _sut.GetTempFileName(Extension);
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
        public void ShouldNotReturnTempFileNameWithExtensionTwice()
        {
            var a = _sut.GetTempFileName(Extension);
            var b = _sut.GetTempFileName(Extension);
            Assert.AreNotEqual(a, b);
        }

        [Test]
        public void ShouldReturnTempFileNameWithExtension()
        {
            var fileName = _sut.GetTempFileName(Extension);
            Assert.AreEqual("." + Extension, Path.GetExtension(fileName));
        }

        [Test]
        public void ShouldReturnExistingTempDirectory()
        {
            var dir = _sut.GetTempDirectoryName();

            Assert.IsTrue(Directory.Exists(dir));
        }

        [Test]
        public void ShouldNotReturnTempDirectoryTwice()
        {
            var a = _sut.GetTempDirectoryName();
            var b = _sut.GetTempDirectoryName();
            Assert.AreNotEqual(a, b);
        }

        [Test]
        public void ShouldReturnDirectoryRootedInTempFolder()
        {
            var dir = _sut.GetTempDirectoryName();

            Assert.AreEqual(Path.GetTempPath(), Directory.GetParent(dir) + "\\");
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
        public void ShouldCreateFileInNonExistentDirectory()
        {
            var path = Path.Combine(GetNonExistentTempFileName(), "ActualFile");

            _sut.CreateFile(path);

            Assert.IsTrue(File.Exists(path));
        }

        [Test]
        public void EmptyFileShouldHaveZeroSize()
        {
            var filename = Path.GetTempFileName();

            var actual = _sut.GetFileSize(filename);
            const long expected = 0;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldHaveCorrectSize()
        {
            var filename = Path.GetTempFileName();
            File.WriteAllBytes(filename, new byte[100]);

            var actual = _sut.GetFileSize(filename);
            const long expected = 100;

            Assert.AreEqual(expected, actual);
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

        [Test]
        public void ShouldFindAllFilesRecursively()
        {
            var dir = IoTestHelper.GetTempDirectoryName();
            File.Create(Path.Combine(dir, "a")).Close();
            File.Create(Path.Combine(dir, "b")).Close();

            Directory.CreateDirectory(Path.Combine(dir, "b"));

            File.Create(Path.Combine(dir, "b", "b")).Close();
            var expected = new List<string> {"ABC", "DEF", "XYZ"};
            expected.ForEach(f => { using (File.Create(Path.Combine(dir, f))) {} });

            var actual = _sut.EnumerateFiles(dir).Select(Path.GetFileName);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldWriteBytes()
        {
            var file = Path.GetTempFileName();
            const string expected = "String";
            var asBytes = expected.AsBytes();
            _sut.WriteAllByte(asBytes, file);

            var actual = File.ReadAllText(file, Encoding.Unicode);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DirectoryExists()
        {
            var tempFolder = Path.GetTempPath();
            var nonExistingFolder = Path.Combine(tempFolder, "TempFolder_For_IOUtilsTest.DirectoryExists");
            Assert.False(Directory.Exists(nonExistingFolder));
            try
            {
                Assert.False(_sut.DirectoryExists(nonExistingFolder));

                Directory.CreateDirectory(nonExistingFolder);
                Assert.True(Directory.Exists(nonExistingFolder));

                Assert.True(_sut.DirectoryExists(nonExistingFolder));
            }
            finally
            {
                Directory.Delete(nonExistingFolder);
            }
        }

        [Test]
        public void GetFiles()
        {
            var folder = Path.Combine(Path.GetTempPath(), "TempFolder_For_IOUtilsTest.GetFiles");
            try
            {
                Assert.False(Directory.Exists(folder), "test folder is preexisting");

                Directory.CreateDirectory(folder);
                Assert.AreEqual(new string[0], _sut.GetFiles(folder, "*"));

                File.Create(Path.Combine(folder, "a")).Close();
                File.Create(Path.Combine(folder, "b")).Close();
                Directory.CreateDirectory(Path.Combine(folder, "C"));

                var actuals = _sut.GetFiles(folder, "*");
                var expecteds = new[] {Path.Combine(folder, "a"), Path.Combine(folder, "b")};
                Assert.AreEqual(expecteds, actuals);
            }
            finally
            {
                Directory.Delete(folder, true);
            }
        }

        [Test]
        public void GetFilesRecursive()
        {
            var folder = Path.Combine(Path.GetTempPath(), "TempFolder_For_IOUtilsTest.GetFilesRecursive");
            try
            {
                Assert.False(Directory.Exists(folder), "test folder is preexisting");

                Directory.CreateDirectory(folder);
                Assert.AreEqual(new string[0], _sut.GetFilesRecursive(folder, "*"));

                File.Create(Path.Combine(folder, "a.txt")).Close();
                File.Create(Path.Combine(folder, "a.zip")).Close();

                Directory.CreateDirectory(Path.Combine(folder, "b"));
                File.Create(Path.Combine(folder, "b", "b.zip")).Close();

                Directory.CreateDirectory(Path.Combine(folder, "c"));

                var actuals = _sut.GetFilesRecursive(folder, "*.zip");
                var expecteds = new[] {Path.Combine(folder, "a.zip"), Path.Combine(folder, "b", "b.zip")};
                Assert.AreEqual(expecteds, actuals);
            }
            finally
            {
                Directory.Delete(folder, true);
            }
        }

        [Test]
        public void UnzipToTempFolder()
        {
            var temp = Path.GetTempPath();
            var zipFileName = Path.Combine(temp, "ZipFile_for_IoUtilsTest.UnzipToTempFolder.zip");

            try
            {
                var expectedA = "a" + new Random().Next();
                var expectedB = "b" + new Random().Next();

                CreateZipFileWithContents(zipFileName, expectedA, expectedB);

                var unzipFolder = _sut.UnzipToTempFolder(zipFileName);

                try
                {
                    var isExtractedInTempFolder = unzipFolder.StartsWith(temp);
                    Assert.True(isExtractedInTempFolder);

                    var actualA = AssertExistsAndRead(unzipFolder, "file0.txt");
                    Assert.AreEqual(expectedA, actualA);

                    var actualB = AssertExistsAndRead(unzipFolder, "file1.txt");
                    Assert.AreEqual(expectedA, actualA);
                }
                finally
                {
                    Directory.Delete(unzipFolder, true);
                }
            }
            finally
            {
                File.Delete(zipFileName);
            }
        }

        private object AssertExistsAndRead(string folder, string fileName)
        {
            var fullFileName = Path.Combine(folder, fileName);
            Assert.True(File.Exists(fullFileName));
            return File.ReadAllText(fullFileName);
        }

        private static void CreateZipFileWithContents(string zipFileName, params string[] contents)
        {
            using (var outStream = new FileStream(zipFileName, FileMode.Create))
            {
                using (var zipFile = new ZipFile())
                {
                    zipFile.UseZip64WhenSaving = Zip64Option.AsNecessary;
                    for (var i = 0; i < contents.Length; i++)
                    {
                        zipFile.AddEntry("file" + i + ".txt", contents[i]);
                    }
                    zipFile.Save(outStream);
                }
            }
        }

        private static string GetNonExistentTempFileName()
        {
            return Path.Combine(IoTestHelper.GetTempDirectoryName(), "ArbitraryFileName" + System.DateTime.Now.Ticks);
        }
    }
}