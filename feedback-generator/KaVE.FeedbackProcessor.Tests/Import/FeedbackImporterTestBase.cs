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
using System.Linq;
using Ionic.Zip;
using KaVE.Commons.TestUtils.Utils.Exceptions;
using KaVE.Commons.Utils.Exceptions;
using KaVE.FeedbackProcessor.Database;
using KaVE.FeedbackProcessor.Import;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Import
{
    internal abstract class FeedbackImporterTestBase : FeedbackDatabaseBasedTest
    {
        private readonly TestLogger _logger = new TestLogger(true);
        private readonly ICollection<FeedbackArchiveBuilder> _inputArchiveBuilders = new List<FeedbackArchiveBuilder>();

        [SetUp]
        public void EnsureCleanDatabase()
        {
            CollectionAssert.IsEmpty(TestFeedbackDatabase.GetDeveloperCollection().FindAll());
            CollectionAssert.IsEmpty(TestFeedbackDatabase.GetEventsCollection().FindAll());
        }

        [TearDown]
        public void ClearInput()
        {
            _inputArchiveBuilders.Clear();
        }

        protected FeedbackArchiveBuilder GivenInputArchive(string filename)
        {
            var archive = FeedbackArchiveBuilder.AnArchive(filename);
            _inputArchiveBuilders.Add(archive);
            return archive;
        }

        protected void WhenImportIsRun()
        {
            var uut = new TestImporter(TestFeedbackDatabase, _logger, _inputArchiveBuilders);
            uut.Import();
        }

        private class TestImporter : FeedbackImporter
        {
            private readonly ICollection<FeedbackArchiveBuilder> _inputArchiveBuilders;

            internal TestImporter(IFeedbackDatabase database,
                ILogger logger,
                ICollection<FeedbackArchiveBuilder> inputArchiveBuilders) : base(database, logger)
            {
                _inputArchiveBuilders = inputArchiveBuilders;
            }

            protected override IEnumerable<ZipFile> OpenFeedbackArchives()
            {
                return _inputArchiveBuilders.Select(builder => builder.Create());
            }
        }
    }
}