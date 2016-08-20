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
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils.IO.Archives;
using KaVE.RS.SolutionAnalysis.CleanUp;
using NUnit.Framework;

namespace KaVE.RS.SolutionAnalysis.Tests.CleanUp
{
    internal class CleanUpIoTest
    {
        #region setup and helpers

        private CleanUpIo _sut;
        private string _dirIn;
        private string _dirOut;

        [SetUp]
        public void Setup()
        {
            _dirIn = CreateTempDir();
            _dirOut = CreateTempDir();
            _sut = new CleanUpIo(_dirIn, _dirOut);
        }

        private void SetupWithSlashes()
        {
            _sut = new CleanUpIo(_dirIn + @"\", _dirOut + @"\");
        }

        [TearDown]
        public void Teardown()
        {
            if (Directory.Exists(_dirIn))
            {
                Directory.Delete(_dirIn, true);
            }
            if (Directory.Exists(_dirOut))
            {
                Directory.Delete(_dirOut, true);
            }
        }

        private static string CreateTempDir()
        {
            var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(path);
            return path;
        }

        private void AddFile(string fileName, params IDEEvent[] events)
        {
            var fullName = Path.Combine(_dirIn, fileName);
            var dir = Path.GetDirectoryName(fullName);
            if (dir != null)
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }

            using (var wa = new WritingArchive(fullName))
            {
                foreach (var e in events)
                {
                    wa.Add(e);
                }
            }
        }

        private void AssertEvents(string fileName, params IDEEvent[] events)
        {
            var fullName = Path.Combine(_dirOut, fileName);
            var ra = new ReadingArchive(fullName);
            var actuals = ra.GetAll<IDEEvent>();
            var expecteds = events.ToList();
            CollectionAssert.AreEqual(expecteds, actuals);
        }

        private static IDEEvent E(int i)
        {
            return new CommandEvent
            {
                CommandId = "id:" + i
            };
        }

        private static IEnumerable<IDEEvent> Es(params int[] ns)
        {
            return ns.Select(E);
        }

        #endregion

        [Test]
        public void GetZips()
        {
            AddFile(@"1.zip", E(1));
            AddFile(@"sub\2.zip", E(2));

            var zips = _sut.GetZips();

            CollectionAssert.AreEqual(new[] {"1.zip", @"sub\2.zip"}, zips);
        }

        [Test]
        public void GetZips_WithSlashes()
        {
            SetupWithSlashes();
            GetZips();
        }

        [Test]
        public void ReadZip()
        {
            AddFile("1.zip", E(1), E(2), E(3));

            var actuals = _sut.ReadZip("1.zip");
            var expecteds = new[] {E(1), E(2), E(3)};
            CollectionAssert.AreEqual(expecteds, actuals);
        }


        [Test]
        public void ReadZip_WithSlashes()
        {
            SetupWithSlashes();
            ReadZip();
        }

        [Test]
        public void WriteZip()
        {
            _sut.WriteZip(Es(1, 2), "1.zip");
            AssertEvents("1.zip", E(1), E(2));
        }

        [Test]
        public void WriteZip_WithSlashes()
        {
            SetupWithSlashes();
            WriteZip();
        }

        [Test]
        public void WriteZipInSubfolder()
        {
            _sut.WriteZip(Es(1, 2), @"sub\1.zip");
            AssertEvents(@"sub\1.zip", E(1), E(2));
        }
    }
}