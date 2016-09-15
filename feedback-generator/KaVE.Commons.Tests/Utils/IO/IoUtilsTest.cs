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
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using Ionic.Zip;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.IO;
using KaVE.Commons.Utils.IO.Archives;
using Moq;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.IO
{
    internal class IoUtilsTest
    {
        private const string Extension = "ext";
        private IoUtils _sut;
        private string _testRoot;

        [SetUp]
        public void SetUp()
        {
            _testRoot = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(_testRoot);

            _sut = new IoUtils();
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_testRoot))
            {
                Directory.Delete(_testRoot, true);
            }
        }

        [Test, ExpectedException(typeof(NotImplementedException))]
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
            Assert.True(_sut.DirectoryExists(_testRoot));

            Directory.Delete(_testRoot, true);
            Assert.False(_sut.DirectoryExists(_testRoot));
        }

        [Test]
        public void GetFiles()
        {
            Assert.AreEqual(new string[0], _sut.GetFiles(_testRoot, "*"));

            File.Create(Path.Combine(_testRoot, "a")).Close();
            File.Create(Path.Combine(_testRoot, "b")).Close();
            Directory.CreateDirectory(Path.Combine(_testRoot, "C"));

            var actuals = _sut.GetFiles(_testRoot, "*");
            var expecteds = new[] {Path.Combine(_testRoot, "a"), Path.Combine(_testRoot, "b")};
            Assert.AreEqual(expecteds, actuals);
        }

        [Test]
        public void GetFilesRecursive()
        {
            Assert.AreEqual(new string[0], _sut.GetFilesRecursive(_testRoot, "*"));

            File.Create(Path.Combine(_testRoot, "a.txt")).Close();
            File.Create(Path.Combine(_testRoot, "a.zip")).Close();

            Directory.CreateDirectory(Path.Combine(_testRoot, "b"));
            File.Create(Path.Combine(_testRoot, "b", "b.zip")).Close();

            Directory.CreateDirectory(Path.Combine(_testRoot, "c"));

            var actuals = _sut.GetFilesRecursive(_testRoot, "*.zip");
            var expecteds = new[] {Path.Combine(_testRoot, "a.zip"), Path.Combine(_testRoot, "b", "b.zip")};
            Assert.AreEqual(expecteds, actuals);
        }

        [Test]
        public void UnzipToTempFolder()
        {
            var zipFileName = Path.Combine(_testRoot, "IoUtilsTest.UnzipToTempFolder.zip");

            var expectedA = "a" + new Random().Next();
            var expectedB = "b" + new Random().Next();

            CreateZipFileWithContents(zipFileName, expectedA, expectedB);

            var unzipFolder = _sut.UnzipToTempFolder(zipFileName);

            try
            {
                var isExtractedInTempFolder = unzipFolder.StartsWith(Path.GetTempPath());
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

        [Test]
        public void ReadArchive_HappyPath()
        {
            var zipFileName = Path.Combine(_testRoot, "a.zip");
            var expecteds = new List<string> {"a", "b"};
            using (var wa = new WritingArchive(zipFileName))
            {
                foreach (var c in expecteds)
                {
                    wa.Add(c);
                }
            }
            using (var ra = _sut.ReadArchive(zipFileName))
            {
                var actuals = ra.GetAll<string>();
                CollectionAssert.AreEquivalent(expecteds, actuals);
            }
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void ReadArchive_NonExisting()
        {
            var zipFileName = Path.Combine(_testRoot, "NonExisting.zip");
            _sut.ReadArchive(zipFileName);
        }

        [Test]
        public void CreateArchive_HappyPath()
        {
            var expecteds = new List<string> {"a", "b"};

            var zipFileName = Path.Combine(_testRoot, "a.zip");
            using (var wa = _sut.CreateArchive(zipFileName))
            {
                foreach (var c in expecteds)
                {
                    wa.Add(c);
                }
            }

            Assert.True(File.Exists(zipFileName));
            var actuals = new ReadingArchive(zipFileName).GetAll<string>();
            CollectionAssert.AreEquivalent(expecteds, actuals);
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void CreateArchive_FileExists()
        {
            var zipFileName = Path.Combine(_testRoot, "Existing.zip");
            File.Create(zipFileName).Close();
            _sut.CreateArchive(zipFileName);
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void CreateArchive_NonExistingParent()
        {
            var zipFileName = Path.Combine(_testRoot, "NonExistingParent", "a.zip");
            _sut.CreateArchive(zipFileName);
        }

        [Test]
        public void ShouldCountLines()
        {
            var file = Path.Combine(_testRoot, "x.txt");

            CreateContentInFile(file, 0);
            Assert.AreEqual(0, _sut.CountLines(file));

            CreateContentInFile(file, 1);
            Assert.AreEqual(1, _sut.CountLines(file));

            CreateContentInFile(file, 10);
            Assert.AreEqual(10, _sut.CountLines(file));
        }

        [Test]
        public void ShouldManageToCountLinesInLargeFilesInReasonableAmountOfTime()
        {
            const int maxLines = 20000;
            const int maxDuration = 5 * 1000; // in sec

            var rnd = new Random();
            var file = Path.Combine(_testRoot, "x.txt");
            var lines = new List<string>();
            for (var i = 0; i < maxLines; i++)
            {
                var sb = new StringBuilder();
                var linelength = 1000 + rnd.Next()%10000;
                for (var j = 0; j < linelength; j++)
                {
                    sb.Append('.');
                }
                lines.Add(sb.ToString());
            }
            File.WriteAllLines(file, lines);

            var sw = Stopwatch.StartNew();
            Assert.AreEqual(maxLines, _sut.CountLines(file));
            var duration = sw.ElapsedMilliseconds;

            Assert.That(duration < maxDuration, "should not take longer than {0:0.0}s, but took {1:0.0}s".FormatEx(maxDuration/1000.0, duration/1000.0)); // 3s
        }

        private static void CreateContentInFile(string fileName, int numLines)
        {
            var lines = new List<string>();
            for (var i = 0; i < numLines; i++)
            {
                var line = "l" + i;
                lines.Add(line);
            }
            File.WriteAllLines(fileName, lines);
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