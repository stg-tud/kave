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
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.IO.Archives;
using KaVE.FeedbackProcessor.Preprocessing;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Preprocessing
{
    internal class GroupMergerTest : FileBasedPreprocessingTestBase
    {
        #region setup & helpers

        private GroupMerger _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new GroupMerger(Io);
        }

        private static CommandEvent Event(string id)
        {
            return new CommandEvent {CommandId = id};
        }

        private void Add(string relFile, params IDEEvent[] events)
        {
            var file = Path.Combine(RawDir, relFile);
            var parent = Path.GetDirectoryName(file);
            // ReSharper disable once AssignNullToNotNullAttribute
            Directory.CreateDirectory(parent);
            using (var wa = new WritingArchive(file))
            {
                wa.AddAll(events);
            }
        }

        private void AssertNonExistence(params string[] relFiles)
        {
            foreach (var relFile in relFiles)
            {
                var file = Path.Combine(MergedDir, relFile);
                Assert.IsFalse(File.Exists(file));
            }
        }

        private void Expect(string relFile, params IDEEvent[] expecteds)
        {
            var file = Path.Combine(MergedDir, relFile);
            var ra = new ReadingArchive(file);
            var actuals = ra.GetAll<IDEEvent>();
            CollectionAssert.AreEqual(expecteds, actuals);
        }

        private void Merge(params string[] relFiles)
        {
            _sut.Merge(Sets.NewHashSetFrom(relFiles));
        }

        #endregion

        [Test]
        public void MergingWorks()
        {
            Add(@"a.zip", Event("a"));
            Add(@"b.zip", Event("b"));
            Add(@"c.zip", Event("c"));

            Merge(@"a.zip", @"b.zip", @"c.zip");

            Expect(@"a.zip", Event("a"), Event("b"), Event("c"));
            AssertNonExistence(@"b.zip", @"sub\c.zip");
        }

        [Test]
        public void SubfoldersWork()
        {
            Add(@"sub\a.zip", Event("a"));
            Merge(@"sub\a.zip");
            Expect(@"sub\a.zip", Event("a"));
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void GroupMustNotBeEmpty()
        {
            Merge();
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void FilesHaveToExist()
        {
            Add(@"a.zip", Event("a"));
            Merge(@"a.zip", "x.zip");
        }
    }
}