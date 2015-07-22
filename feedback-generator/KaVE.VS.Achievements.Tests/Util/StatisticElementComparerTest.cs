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

using System.ComponentModel;
using KaVE.VS.Achievements.UI.StatisticUI;
using KaVE.VS.Achievements.Util;
using NUnit.Framework;

namespace KaVE.VS.Achievements.Tests.Util
{
    [TestFixture]
    public class StatisticElementComparerTest
    {
        private StatisticElementComparer _uut;

        [TestCaseSource("StringTestValues"), TestCaseSource("IntegerTestValues"), TestCaseSource("DoubleTestValues"),
         TestCaseSource("BigIntegerTestValues"), TestCaseSource("TimeSpanValues")]
        public void CompareTest(string critera,
            string name1,
            string value1,
            string name2,
            string value2,
            int expectedResult,
            ListSortDirection direction)
        {
            _uut = new StatisticElementComparer(critera, direction);

            var s1 = new StatisticElement {Name = name1, Value = value1};
            var s2 = new StatisticElement {Name = name2, Value = value2};

            var actualResult = _uut.Compare(s1, s2);

            switch (expectedResult)
            {
                case -1:
                    Assert.Less(actualResult, 0);
                    break;
                case 0:
                    Assert.AreEqual(0, actualResult);
                    break;
                case 1:
                    Assert.Greater(actualResult, 0);
                    break;
            }
        }

        private static readonly object[][] StringTestValues =
        {
            new object[]
            {
                "Name",
                "File.SaveAll",
                "10",
                "Options...",
                "12",
                -1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Name",
                "File.Open",
                "1",
                "CodeCompleteBasic",
                "1",
                1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Name",
                "File.SaveAll",
                "10",
                "Options...",
                "12",
                1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Name",
                "File.Open",
                "1",
                "CodeCompleteBasic",
                "2",
                -1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Name",
                "NewFile",
                "2",
                "NewFile",
                "2",
                0,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Name",
                "NewFile",
                "2",
                "NewFile",
                "2",
                0,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Name",
                "",
                "",
                "",
                "",
                0,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Name",
                "NotEmpty",
                "",
                "",
                "",
                1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Name",
                "",
                "",
                "NotEmpty",
                "",
                -1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Name",
                "",
                "",
                "",
                "",
                0,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Name",
                "NotEmpty",
                "",
                "",
                "",
                -1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Name",
                "",
                "",
                "NotEmpty",
                "",
                1,
                ListSortDirection.Descending
            }
        };

        private static readonly object[][] IntegerTestValues =
        {
            new object[]
            {
                "Value",
                "",
                "",
                "",
                "",
                0,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "10",
                "",
                "12",
                -1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "12",
                "",
                "10",
                1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "10",
                "",
                "12",
                1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "12",
                "",
                "10",
                -1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "0",
                "",
                "0",
                0,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "0",
                "",
                "0",
                0,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "-10",
                "",
                "20",
                -1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "-10",
                "",
                "20",
                1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "20",
                "",
                "-10",
                1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "20",
                "",
                "-10",
                -1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "-25",
                "",
                "-10",
                -1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "-25",
                "",
                "-10",
                1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "-25",
                "",
                "10",
                -1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "-25",
                "",
                "10",
                1,
                ListSortDirection.Descending
            }
        };

        private static readonly object[][] DoubleTestValues =
        {
            new object[]
            {
                "Value",
                "",
                "10,5",
                "",
                "12,5",
                -1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "10,5",
                "",
                "12,5",
                1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "1,333",
                "",
                "1,25",
                1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "1,333",
                "",
                "1,25",
                -1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "-1,25",
                "",
                "3,5",
                -1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "-1,25",
                "",
                "3,5",
                1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "2,5",
                "",
                "2",
                1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "2,5",
                "",
                "2",
                -1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "13,33",
                "",
                "22",
                -1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "13,33",
                "",
                "22",
                1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "-1,25",
                "",
                "-3,5",
                1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "-1,25",
                "",
                "-3,5",
                -1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "-12,5",
                "",
                "10,5",
                -1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "-12,5",
                "",
                "10,5",
                1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "0.0,0.0",
                "",
                "0,0.0,0",
                0,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "12,0",
                "",
                "0,0.0,0",
                0,
                ListSortDirection.Ascending
            }
        };

        private static readonly object[][] BigIntegerTestValues =
        {
            new object[]
            {
                "Value",
                "",
                "100000000000",
                "",
                "120000000000",
                -1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "100000000000",
                "",
                "120000000000",
                1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "12",
                "",
                "1000000000000000",
                -1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "12",
                "",
                "1000000000000000",
                1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "1000000000000000",
                "",
                "-10000000000000000",
                1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "1000000000000000",
                "",
                "-10000000000000000",
                -1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "-1000000000000000000",
                "",
                "-100000000000000000000000",
                1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "-1000000000000000000",
                "",
                "-100000000000000000000000",
                -1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "12",
                "",
                "-1000000000000000000000",
                1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "12",
                "",
                "-1000000000000000000000",
                -1,
                ListSortDirection.Descending
            }
        };

        private static readonly object[][] TimeSpanValues =
        {
            new object[]
            {
                "Value",
                "",
                "12,5 d",
                "",
                "10,333 d",
                1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "12,5 d",
                "",
                "10,333 d",
                -1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "15 d",
                "",
                "30 d",
                -1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "15 d",
                "",
                "30 d",
                1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "10,5 d",
                "",
                "10 hr",
                1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "10,5 d",
                "",
                "10 hr",
                -1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "10 d",
                "",
                "10 min",
                1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "10 d",
                "",
                "10 min",
                -1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "10 d",
                "",
                "10 s",
                1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "10 d",
                "",
                "10 s",
                -1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "10 d",
                "",
                "10 ms",
                1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "10 d",
                "",
                "10 ms",
                -1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "10,25 hr",
                "",
                "20,21 hr",
                -1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "10,25 hr",
                "",
                "20,21 hr",
                1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "11,33 hr",
                "",
                "10,25 hr",
                1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "11,33 hr",
                "",
                "10,25 hr",
                -1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "10 hr",
                "",
                "10 min",
                1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "10 hr",
                "",
                "10 min",
                -1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "10 hr",
                "",
                "10 s",
                1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "10 hr",
                "",
                "10 s",
                -1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "10 hr",
                "",
                "10 ms",
                1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "10 hr",
                "",
                "10 ms",
                -1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "11,23 min",
                "",
                "9,25 min",
                1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "11,23 min",
                "",
                "9,25 min",
                -1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "8 min",
                "",
                "15 min",
                -1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "8 min",
                "",
                "15 min",
                1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "10 min",
                "",
                "10 s",
                1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "10 min",
                "",
                "10 s",
                -1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "10 min",
                "",
                "10 ms",
                1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "10 min",
                "",
                "10 ms",
                -1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "20 s",
                "",
                "11 s",
                1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "20 s",
                "",
                "11 s",
                -1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "11,25 s",
                "",
                "33,2 s",
                -1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "11,25 s",
                "",
                "33,2 s",
                1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "10 s",
                "",
                "10 ms",
                1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "10 s",
                "",
                "10 ms",
                -1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "10 ms",
                "",
                "20 ms",
                -1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "10 ms",
                "",
                "20 ms",
                1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "25,33 ms",
                "",
                "11,25 ms",
                1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "25,33 ms",
                "",
                "11,25 ms",
                -1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "10 ms",
                "",
                "10 s",
                -1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "10 ms",
                "",
                "10 s",
                1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "10 ms",
                "",
                "10 min",
                -1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "10 ms",
                "",
                "10 min",
                1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "10 ms",
                "",
                "10 hr",
                -1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "10 ms",
                "",
                "10 hr",
                1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "10 ms",
                "",
                "10 d",
                -1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "10 ms",
                "",
                "10 d",
                1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "10 s",
                "",
                "10 min",
                -1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "10 s",
                "",
                "10 min",
                1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "10 s",
                "",
                "10 hr",
                -1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "10 s",
                "",
                "10 hr",
                1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "10 s",
                "",
                "10 d",
                -1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "10 s",
                "",
                "10 d",
                1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "10 min",
                "",
                "10 hr",
                -1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "10 min",
                "",
                "10 hr",
                1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "10 min",
                "",
                "10 d",
                -1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "10 min",
                "",
                "10 d",
                1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "10 hr",
                "",
                "10 d",
                -1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "10 hr",
                "",
                "10 d",
                1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "10 d",
                "",
                "10",
                1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "10 d",
                "",
                "10",
                -1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "10",
                "",
                "10 d",
                -1,
                ListSortDirection.Ascending
            },
            new object[]
            {
                "Value",
                "",
                "10",
                "",
                "10 d",
                1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "10 ms",
                "",
                "10",
                -1,
                ListSortDirection.Descending
            },
            new object[]
            {
                "Value",
                "",
                "10",
                "",
                "10 ms",
                1,
                ListSortDirection.Descending
            }
        };

        [Test]
        public void CompareNullReturnsZeroTest()
        {
            _uut = new StatisticElementComparer("Name", ListSortDirection.Ascending);

            Assert.AreEqual(0, _uut.Compare(null, null));
        }

        [Test]
        public void CompareOtherObjectsReturnsZeroTest()
        {
            _uut = new StatisticElementComparer("Name", ListSortDirection.Ascending);

            Assert.AreEqual(0, _uut.Compare(new object(), new object()));
        }

        [Test]
        public void InvalidPrimaryCriteriaReturnsZeroTest()
        {
            _uut = new StatisticElementComparer("", ListSortDirection.Ascending);

            Assert.AreEqual(0, _uut.Compare(new StatisticElement(), new StatisticElement()));
        }
    }
}