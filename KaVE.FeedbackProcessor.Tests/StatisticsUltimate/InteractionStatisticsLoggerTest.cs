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
using KaVE.Commons.Model.Events.UserProfiles;
using KaVE.FeedbackProcessor.StatisticsUltimate;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.StatisticsUltimate
{
    internal class InteractionStatisticsLoggerTest
    {
        [Test]
        public void IntegrationTest()
        {
            var sut = new InteractionStatisticsLogger();

            sut.ReportTimeout();
            sut.SearchingZips("C:\\a\\b\\");
            sut.FoundZips(123);

            sut.StartingStatCreation(1);
            sut.StartingStatCreation(2);
            sut.CreatingStats(1, "a/b.zip");
            sut.CreatingStats(1, "c.zip");
            sut.CreatingStats(1, "d.zip");
            sut.FinishedStatCreation(1);
            sut.FinishedStatCreation(2);

            var res = new Dictionary<string, InteractionStatistics>();
            res["a.zip"] = new InteractionStatistics
            {
                DayFirst = DateTime.Now.AddDays(-1),
                DayLast = DateTime.Now,
                NumDays = 1,
                NumMonth = 2,
                NumEvents = 3,
                Education = Educations.Bachelor,
                Position = Positions.Student,
                NumCodeCompletion = 4,
                NumTestRuns = 5,
                ActiveTime = TimeSpan.FromSeconds(123456.789)
            };
            res["b/c.zip"] = new InteractionStatistics
            {
                DayFirst = DateTime.Now.AddDays(-5),
                DayLast = DateTime.Now.AddDays(-3),
                NumDays = 6,
                NumMonth = 7,
                NumEvents = 8,
                Education = Educations.Master,
                Position = Positions.SoftwareEngineer,
                NumCodeCompletion = 9,
                NumTestRuns = 10,
                ActiveTime = TimeSpan.FromSeconds(234567.8901)
            };
            res["d.zip"] = new InteractionStatistics
            {
                DayFirst = DateTime.Now.AddDays(-5),
                DayLast = DateTime.Now.AddDays(-3),
                NumDays = 6,
                NumMonth = 7,
                NumEvents = 8,
                Education = Educations.Master,
                Position = Positions.SoftwareEngineer,
                NumCodeCompletion = 9,
                NumTestRuns = 10,
                ActiveTime = TimeSpan.FromDays(2).Add(TimeSpan.FromHours(10))
            };
            res["e.zip"] = new InteractionStatistics
            {
                DayFirst = DateTime.Now.AddDays(-5),
                DayLast = DateTime.Now.AddDays(-3),
                NumDays = 6,
                NumMonth = 7,
                NumEvents = 8,
                Education = Educations.Master,
                Position = Positions.SoftwareEngineer,
                NumCodeCompletion = 9,
                NumTestRuns = 10,
                ActiveTime = TimeSpan.FromSeconds(1)
            };


            sut.Result(res);
        }
    }
}