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

using KaVE.FeedbackProcessor.Model;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Model
{
    [TestFixture]
    class DeveloperTest
    {
        [Test]
        public void IsAnonymousSessionDeveloperIfItHasNoSessionIds()
        {
            // ReSharper disable once RedundantEmptyObjectOrCollectionInitializer
            var developer = new Developer{SessionIds = {}};

            Assert.True(developer.IsAnonymousSessionDeveloper);
        }

        [Test]
        public void IsNonAnonymousSessionDeveloperIfItHasSessionIds()
        {
            var developer = new Developer { SessionIds = { "sessionA" } };

            Assert.False(developer.IsAnonymousSessionDeveloper);
        }
    }
}
