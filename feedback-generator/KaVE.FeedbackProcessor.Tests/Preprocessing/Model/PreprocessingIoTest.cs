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
using KaVE.FeedbackProcessor.Preprocessing.Model;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Preprocessing.Model
{
    internal class PreprocessingIoTest : FileBasedPreprocessingTestBase
    {
        private PreprocessingIo _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new PreprocessingIo(RawDir, MergedDir, FinalDir);
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void PathHasToExist_Raw()
        {
            const string nonExisting = "C:\\does\\not\\exist\\";
            _sut = new PreprocessingIo(nonExisting, MergedDir, FinalDir);
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void PathHasToExist_Merged()
        {
            const string nonExisting = "C:\\does\\not\\exist\\";
            _sut = new PreprocessingIo(RawDir, nonExisting, FinalDir);
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void PathHasToExist_Final()
        {
            const string nonExisting = "C:\\does\\not\\exist\\";
            _sut = new PreprocessingIo(RawDir, MergedDir, nonExisting);
        }

        [Test]
        public void FullPathRaw()
        {
            Assert.IsFalse(RawDir.EndsWith(@"\"));
            var actual = _sut.GetFullPath_In("a.zip");
            var expected = Path.Combine(RawDir, "a.zip");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FullPathMerged()
        {
            Assert.IsFalse(MergedDir.EndsWith(@"\"));
            var actual = _sut.GetFullPath_Merged("a.zip");
            var expected = Path.Combine(MergedDir, "a.zip");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FullPathFinal()
        {
            Assert.IsFalse(FinalDir.EndsWith(@"\"));
            var actual = _sut.GetFullPath_Out("a.zip");
            var expected = Path.Combine(FinalDir, "a.zip");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FindZips()
        {
            Touch(RawDir, "a.zip");
            Touch(RawDir, Path.Combine("sub", "b.zip"));
            Touch(MergedDir, "c.zip");
            Touch(FinalDir, "d.zip");

            var actuals = _sut.FindRelativeZipPaths();
            var expecteds = new[] {"a.zip", @"sub\b.zip"};
            CollectionAssert.AreEquivalent(expecteds, actuals);
        }

        [Test]
        public void FindZips_DoesOnlyIncludesZips()
        {
            Touch(RawDir, "a.zip");
            Touch(RawDir, "b.txt");

            var actuals = _sut.FindRelativeZipPaths();
            var expecteds = new[] {"a.zip"};
            CollectionAssert.AreEquivalent(expecteds, actuals);
        }

        [Test]
        public void FindZips_DoesNotIncludeOtherFolders()
        {
            Touch(RawDir, "a.zip");
            Touch(MergedDir, "b.zip");
            Touch(FinalDir, "c.zip");

            var actuals = _sut.FindRelativeZipPaths();
            var expecteds = new[] {"a.zip"};
            CollectionAssert.AreEquivalent(expecteds, actuals);
        }

        private static void Touch(string dir, string file)
        {
            var fullPath = Path.Combine(dir, file);
            var parent = Path.GetDirectoryName(fullPath);
            // ReSharper disable once AssignNullToNotNullAttribute
            Directory.CreateDirectory(parent);
            File.Create(fullPath).Close();
        }

        [Test]
        public void EnsureParent()
        {
            var parent = Path.Combine(RawDir, "sub");
            var file = Path.Combine(parent, "a.zip");

            Assert.IsFalse(Directory.Exists(parent));
            _sut.EnsureParentExists(file);
            Assert.IsTrue(Directory.Exists(parent));
        }

        [Test]
        public void Size_Empty()
        {
            var file = Path.Combine(RawDir, "a.zip");

            File.Create(file).Close();
            Assert.AreEqual(0, _sut.GetSize_In("a.zip"));
        }

        [Test]
        public void Size_Small()
        {
            var file = Path.Combine(RawDir, "a.zip");

            using (var fs = File.Create(file))
            {
                fs.WriteByte(0);
            }

            Assert.AreEqual(1, _sut.GetSize_In("a.zip"));
        }

        [Test]
        public void Size_Large()
        {
            var file = Path.Combine(RawDir, "a.zip");

            using (var fs = File.Create(file))
            {
                for (var i = 0; i < 1000; i++)
                {
                    fs.WriteByte(0);
                }
            }

            Assert.AreEqual(1000, _sut.GetSize_In("a.zip"));
        }
    }
}