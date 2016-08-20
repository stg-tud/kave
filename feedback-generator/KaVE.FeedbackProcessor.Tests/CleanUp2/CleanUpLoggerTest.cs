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
using KaVE.FeedbackProcessor.CleanUp2;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.CleanUp2
{
    internal class CleanUpLoggerTest
    {
        [Test]
        public void IntegrationTest()
        {
            var sut = new CleanUpLogger();

            sut.FoundZips(Lists.NewList("a", "b", "c"));

            sut.ReadingZip("f1");

            sut.ApplyingFilters();

            sut.ApplyingFilter("f1");
            sut.ApplyingFilter("f2");

            sut.RemovingDuplicates();
            sut.OrderingEvents();
            sut.WritingEvents();

            sut.IntermediateResult(
                "aaaa",
                new Dictionary<string, int>
                {
                    {"a", 1},
                    {"b", 2}
                });

            sut.IntermediateResult(
                "bbb",
                new Dictionary<string, int>
                {
                    {"b", 4},
                    {"c", 8}
                });

            sut.Finish();
        }
    }
}