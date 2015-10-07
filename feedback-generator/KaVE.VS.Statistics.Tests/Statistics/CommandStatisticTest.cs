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
using KaVE.VS.Statistics.Statistics;
using KaVE.VS.Statistics.UI;
using NUnit.Framework;

namespace KaVE.VS.Statistics.Tests.Statistics
{
    [TestFixture]
    public class CommandStatisticTest
    {
        [Test]
        public void DefaultValues()
        {
            var uut = new CommandStatistic();

            CollectionAssert.IsEmpty(uut.CommandTypeValues);
        }

        [Test]
        public void GetCollectionTest()
        {
            var uut = new CommandStatistic();
            uut.CommandTypeValues.Add("ShowOptions", 10);
            uut.CommandTypeValues.Add("{66BD4C1D-3401-4BCC-A942-E4990827E6F7}:8289:", 1000);
            uut.CommandTypeValues.Add("{5EFC7975-14BC-11CF-9B2B-00AA00573819}:26:Edit.Paste", 10000000);
            uut.CommandTypeValues.Add("TextControl.Backspace", 1);

            var expectedCollection = new List<StatisticElement>
            {
                new StatisticElement
                {
                    Name = "ShowOptions",
                    Value = "10"
                },
                new StatisticElement
                {
                    Name = "Edit.Paste",
                    Value = "10.000.000"
                },
                new StatisticElement
                {
                    Name = "TextControl.Backspace",
                    Value = "1"
                }
            };

            var actualCollection = uut.GetCollection();

            CollectionAssert.AreEqual(expectedCollection, actualCollection);
        }
    }
}