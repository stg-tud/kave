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
using KaVE.FeedbackProcessor.Preprocessing.Logging;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Preprocessing.Logging
{
    internal class MultiThreadedPreprocessingLoggerTest : LoggerTestBase
    {
        private MultiThreadedPreprocessingLogger _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new MultiThreadedPreprocessingLogger(Log);
            //_sut = new MultiThreadedPreprocessingLogger(new ConsoleLogger(new DateUtils()));
        }

        [Test]
        public void LoggerTestBase()
        {
            _sut.Init(3, "<in>", "<merged>", "<out>");

            _sut.ReadingIds(5678);
            _(taskId => _sut.StartWorkerReadIds(taskId));
            _sut.ReadIds(0, "x");
            _sut.ReadIds(1, "y");
            _sut.ReadIds(2, "z");
            _(taskId => _sut.StopWorkerReadIds(taskId));

            _sut.GroupZipsByIds();

            _sut.MergeGroups(123);
            _(taskId => _sut.StartWorkerMergeGroup(taskId));
            _sut.MergeGroup(0, 3);
            _sut.MergeGroup(1, 4);
            _sut.MergeGroup(2, 5);
            _(taskId => _sut.StopWorkerMergeGroup(taskId));

            _sut.Cleaning(117);
            _(taskId => _sut.StartWorkerCleanZip(taskId));
            _sut.CleanZip(0, "x");
            _sut.CleanZip(1, "y");
            _sut.CleanZip(2, "z");
            _(taskId => _sut.StopWorkerCleanZip(taskId));

            AssertLog(
                "############################################################",
                "# MultiThreadedPreprocessing",
                "############################################################",
                "",
                "workers: 3",
                "dirIn: <in>",
                "dirMerged: <merged>",
                "dirOut: <out>",
                "",
                "------------------------------------------------------------",
                "Reading ids from 5678 zips",
                "------------------------------------------------------------",
                "(0) Starting worker",
                "(1) Starting worker",
                "(0) Reading zip 1/5678 (0.02% started): x",
                "(1) Reading zip 2/5678 (0.04% started): y",
                "(2) Reading zip 3/5678 (0.05% started): z",
                "(0) Stopping worker",
                "(1) Stopping worker",
                "",
                "------------------------------------------------------------",
                "GroupZipsByIds",
                "------------------------------------------------------------",
                "",
                "------------------------------------------------------------",
                "Merging 123 groups",
                "------------------------------------------------------------",
                "(0) Starting worker",
                "(1) Starting worker",
                "(0) Merging group 1/123 (0.8% started), contains 3 zips",
                "(1) Merging group 2/123 (1.6% started), contains 4 zips",
                "(2) Merging group 3/123 (2.4% started), contains 5 zips",
                "(0) Stopping worker",
                "(1) Stopping worker",
                "",
                "------------------------------------------------------------",
                "Cleaning 117 zips",
                "------------------------------------------------------------",
                "(0) Starting worker",
                "(1) Starting worker",
                "(0) Cleaning zip 1/117 (0.9% started): x",
                "(1) Cleaning zip 2/117 (1.7% started): y",
                "(2) Cleaning zip 3/117 (2.6% started): z",
                "(0) Stopping worker",
                "(1) Stopping worker");
        }

        [Test]
        public void ErrorsCanBeReported()
        {
            _sut.Error(1, CatchException());
            _sut.StartWorkerCleanZip(0); // just for some follow up logging

            var actual = Log.LoggedLines;
            Assert.IsTrue(actual[2].StartsWith("#    at"));
            Assert.IsTrue(actual[2].Contains("MultiThreadedPreprocessingLoggerTest.ThrowException"));
            actual[2] = "# ...";
            Assert.IsTrue(actual[3].StartsWith("#    at"));
            Assert.IsTrue(actual[3].Contains("MultiThreadedPreprocessingLoggerTest.CatchException"));
            actual[3] = "# ...";

            var expected = new[]
            {
                "############################################################",
                "# exception for worker 1: test exception",
                "# ...",
                "# ...",
                "############################################################",
                "(0) Starting worker"
            };

            CollectionAssert.AreEqual(expected, actual);
        }

        private static Exception CatchException()
        {
            try
            {
                ThrowException();
            }
            catch (Exception e)
            {
                return e;
            }
            return null;
        }

        private static void ThrowException()
        {
            throw new Exception("test exception");
        }

        private static void _(Action<int> action)
        {
            for (var i = 0; i < 2; i++)
            {
                action(i);
            }
        }
    }
}