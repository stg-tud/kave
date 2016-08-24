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
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.Preprocessing.Logging;
using KaVE.FeedbackProcessor.Preprocessing.Model;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Preprocessing.Logging
{
    internal class GrouperLoggerTest : LoggerTestBase
    {
        private GrouperLogger _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new GrouperLogger(Log);
        }

        [Test]
        public void Integration()
        {
            _sut.Init();
            _sut.Zips(
                new Dictionary<string, IKaVESet<string>>
                {
                    {"a", Sets.NewHashSet("1", "2")},
                    {"b", Sets.NewHashSet("3")}
                });
            _sut.Users(
                Sets.NewHashSet(
                    new User
                    {
                        Files = {"a", "b"},
                        Identifiers = {"1", "2"}
                    },
                    new User
                    {
                        Files = {"a", "b"},
                        Identifiers = {"1", "2"}
                    }));

            AssertLog(
                "",
                "############################################################",
                "# identifying users",
                "############################################################",
                "",
                "2 zips as input:",
                "",
                "#### zip: a",
                "ids: 1, 2, ",
                "",
                "#### zip: b",
                "ids: 3, ",
                "",
                "------------------------------------------------------------",
                "",
                "identified 1 users:",
                "",
                "#### user 0",
                "",
                "Files:",
                "a, b, ",
                "",
                "Identifier:",
                "1, 2, ");
        }
    }
}