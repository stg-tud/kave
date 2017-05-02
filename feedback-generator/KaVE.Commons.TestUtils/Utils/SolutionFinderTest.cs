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
using System.Linq;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.Json;
using NUnit.Framework;

namespace KaVE.Commons.TestUtils.Utils
{
    internal class SolutionFinderTest
    {
        private string _dir;
        private SolutionFinder _sut;

        [SetUp]
        public void Setup()
        {
            _dir = CreateTempDir();
            _sut = new SolutionFinder(_dir, OrderBy.Alphabetical);
        }

        [TearDown]
        public void Teardown()
        {
            if (Directory.Exists(_dir))
            {
                Directory.Delete(_dir, true);
            }
        }

        private static string CreateTempDir()
        {
            var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(path);
            return path;
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void PathDoesNotExist()
        {
            _sut = new SolutionFinder("C:\\doesNotExist\\", OrderBy.Alphabetical);
        }

        [Test]
        public void TrailingSlashIsCorrected()
        {
            _sut = new SolutionFinder("C:\\Windows", OrderBy.Alphabetical);
            Assert.IsTrue(_sut.Root.EndsWith("\\"));
        }

        [Test]
        public void FindsSolutions_Alphabetically()
        {
            CreateSolutions("a.sln", "d.sln", "b\\c.sln");
            Assert.AreEqual(Lists.NewList("a.sln", "b\\c.sln", "d.sln"), _sut.Solutions);
        }

        [Test]
        public void FindsSolutions_Random()
        {
            _sut = new SolutionFinder(_dir, OrderBy.Random);

            var alphabetical = Lists.NewList<string>();
            for (var i = 0; i < 30; i++)
            {
                var sln = "S{0:0000}.sln".FormatEx(i);
                CreateSolution(sln);
                alphabetical.Add(sln);
            }

            var actual = _sut.Solutions.ToArray();
            CollectionAssert.AreNotEqual(alphabetical, actual);
            CollectionAssert.AreEquivalent(alphabetical, actual);
        }

        [Test]
        public void SolutionsAreOrderedCaseInsensitive()
        {
            CreateSolutions("a.sln", "c.sln", "B\\b.sln");
            Assert.AreEqual(Lists.NewList("a.sln", "B\\b.sln", "c.sln"), _sut.Solutions);
        }

        [Test]
        public void DoesNotFindTestData()
        {
            CreateSolutions("test\\data\\a.sln");
            Assert.AreEqual(Lists.NewList<string>(), _sut.Solutions);
        }

        [Test]
        public void DoesNotFindOtherFiles()
        {
            CreateSolutions("a.sln", "b.txt");
            Assert.AreEqual(Lists.NewList("a.sln"), _sut.Solutions);
        }

        [Test]
        public void CanBuildFullPath()
        {
            const string sln = "a\\b.sln";
            var actual = _sut.GetFullPath(sln);
            var expected = PathOf(sln);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void WritesIndex()
        {
            CreateSolutions("a.sln", "b\\c.sln");
            Assert.IsFalse(File.Exists(PathOf("index.json")));
            var slns = _sut.Solutions;
            Assert.IsTrue(File.Exists(PathOf("index.json")));
            var actual = Read<List<string>>("index.json");
            CollectionAssert.AreEqual(slns, actual);
        }

        [Test]
        public void ReadsIndex()
        {
            CreateSolutions("a.sln", "b\\c.sln");
            Write("index.json", Sets.NewHashSet("c.sln"));

            var slns = _sut.Solutions;
            CollectionAssert.AreEqual(Sets.NewHashSet("c.sln"), slns);
        }

        [Test]
        public void CachesSolutionData_NoIndex()
        {
            CreateSolutions("a.sln");
            var expected = Sets.NewHashSet("a.sln");
            CollectionAssert.AreEqual(expected, _sut.Solutions);

            File.Delete(PathOf("index.json"));
            CreateSolutions("b.sln");
            CollectionAssert.AreEqual(expected, _sut.Solutions);
        }

        [Test]
        public void CachesSolutionData_HasIndex()
        {
            Write(PathOf("index.json"), Lists.NewList("a.sln"));
            CreateSolutions("b.sln");
            var expected = Sets.NewHashSet("a.sln");
            CollectionAssert.AreEqual(expected, _sut.Solutions);
            Write(PathOf("index.json"), Lists.NewList("c.sln"));
            CollectionAssert.AreEqual(expected, _sut.Solutions);
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void CannotStartUnknownSolutions()
        {
            _sut.Start("a.sln");
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void CannotStartStartedSolutions()
        {
            CreateSolutions("a.sln");
            _sut.Start("a.sln");
            _sut.Start("a.sln");
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void CannotEndUnknownSolutions()
        {
            _sut.End("a.sln");
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void CannotEndSolutionsThatAreNotRunning()
        {
            CreateSolutions("a.sln");
            _sut.End("a.sln");
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void CannotCrashUnknownSolutions()
        {
            CreateSolutions("a.sln");
            _sut.Crash("a.sln");
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void CannotCrashSolutionsThatAreNotRunning()
        {
            CreateSolutions("a.sln");
            _sut.End("a.sln");
        }

        [Test]
        public void ShouldStartSolutions()
        {
            CreateSolutions("a.sln", "b.sln");
            _sut.Start("a.sln");

            AssertStarted("a.sln");
            Assert.IsFalse(File.Exists(PathOf("ended.json")));
            Assert.IsFalse(File.Exists(PathOf("crashed.json")));
        }

        [Test]
        public void ShouldEndSolutions()
        {
            CreateSolutions("a.sln");
            _sut.Start("a.sln");
            _sut.End("a.sln");

            AssertStarted();
            AssertEnded("a.sln");
            Assert.IsFalse(File.Exists(PathOf("crashed.json")));
        }

        [Test]
        public void ShouldCrashSolutions()
        {
            CreateSolutions("a.sln");
            _sut.Start("a.sln");
            _sut.Crash("a.sln");

            AssertStarted();
            Assert.IsFalse(File.Exists(PathOf("ended.json")));
            AssertCrashed("a.sln");
        }

        [Test]
        public void Integration()
        {
            CreateSolutions("a.sln", "b.sln", "c.sln", "d.sln", "e.sln");
            _sut.Start("a.sln");
            _sut.End("a.sln");
            _sut.Start("b.sln");
            _sut.Crash("b.sln");
            _sut.Start("c.sln");
            _sut.End("c.sln");
            _sut.Start("d.sln");
            _sut.Crash("d.sln");
            _sut.Start("e.sln");

            AssertStarted("e.sln");
            AssertEnded("a.sln", "c.sln");
            AssertCrashed("b.sln", "d.sln");
        }

        [Test]
        public void ShouldNotIgnoreNewSolutions()
        {
            CreateSolutions("a.sln");
            Assert.IsFalse(_sut.ShouldIgnore("a.sln"));
        }

        [Test]
        public void ShouldIgnoreStartedSolutions()
        {
            CreateSolutions("a.sln");
            _sut.Start("a.sln");
            Assert.IsTrue(_sut.ShouldIgnore("a.sln"));
        }

        [Test]
        public void ShouldIgnoreEndedSolutions()
        {
            CreateSolutions("a.sln");
            _sut.Start("a.sln");
            _sut.End("a.sln");
            Assert.IsTrue(_sut.ShouldIgnore("a.sln"));
        }

        [Test]
        public void ShouldIgnoreCrashedSolutions()
        {
            CreateSolutions("a.sln");
            _sut.Start("a.sln");
            _sut.Crash("a.sln");
            Assert.IsTrue(_sut.ShouldIgnore("a.sln"));
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void ShouldRecognizeUnknownSolutions()
        {
            _sut.ShouldIgnore("a.sln");
        }

        [Test]
        public void ShouldCreateTestData()
        {
            CreateSolutions("a.sln", "z.sln", "c\\c.sln", "B\\A\\x.sln");
            var actual = _sut.GetTestData();
            var expected = new[]
            {
                new[] {"0000", "a.sln"},
                new[] {"0001", "B\\A\\x.sln"},
                new[] {"0002", "c\\c.sln"},
                new[] {"0003", "z.sln"}
            };
            CollectionAssert.AreEqual(expected, actual);
        }

        #region helper

        private void AssertStarted(params string[] slns)
        {
            var actual = Read<List<string>>("started.json");
            var expected = Lists.NewList(slns);
            Assert.AreEqual(expected, actual);
        }

        private void AssertEnded(params string[] slns)
        {
            var actual = Read<List<string>>("ended.json");
            var expected = Lists.NewList(slns);
            Assert.AreEqual(expected, actual);
        }

        private void AssertCrashed(params string[] slns)
        {
            var actual = Read<List<string>>("crashed.json");
            var expected = Lists.NewList(slns);
            Assert.AreEqual(expected, actual);
        }

        private T Read<T>(string file)
        {
            var json = File.ReadAllText(PathOf(file));
            return json.ParseJsonTo<T>();
        }

        private void Write(string file, object o)
        {
            var json = o.ToFormattedJson();
            var fullPath = PathOf(file);
            File.WriteAllText(fullPath, json);
        }

        private void CreateSolutions(params string[] slns)
        {
            foreach (var sln in slns)
            {
                CreateSolution(sln);
            }
        }

        private void CreateSolution(string sln)
        {
            var fullPath = PathOf(sln);
            var slnDir = Path.GetDirectoryName(fullPath);
            Asserts.NotNull(slnDir);
            Directory.CreateDirectory(slnDir);
            File.Create(fullPath).Close();
        }

        private string PathOf(string file)
        {
            Assert.IsFalse(string.IsNullOrEmpty(file));
            return Path.Combine(_dir, file);
        }

        #endregion
    }
}