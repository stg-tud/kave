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

using KaVE.Commons.Model.Events.UserProfiles;
using KaVE.Commons.Utils.Collections;
using KaVE.RS.SolutionAnalysis.StatisticsForPapers;
using NUnit.Framework;

namespace KaVE.RS.SolutionAnalysis.Tests.StatisticsForPapersTest
{
    internal class StatisticsPrinterTest
    {
        [Test]
        public void IntegrationExample()
        {
            var sut = new StatisticsPrinter();

            sut.StartZip("a.zip");
            sut.FoundUserKey("auser");
            sut.FoundKeysInZip(Sets.NewHashSet("a1", "a2", "a3"));

            sut.StartZip("b.zip");
            sut.FoundUserKey("buser");
            sut.FoundKeysInZip(Sets.NewHashSet("b1", "b2", "b3"));

            sut.FoundKeys(Sets.NewHashSet("1", "2", "3"));
            sut.FoundUpes(
                Lists.NewList<IUserProfileEvent>(new UserProfileEvent {Id = "1"}, new UserProfileEvent {Id = "2"}));
        }
    }
}