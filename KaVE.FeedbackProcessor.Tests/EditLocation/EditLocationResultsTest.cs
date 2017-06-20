/*
 * Copyright 2017 Sebastian Proksch
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

using KaVE.Commons.TestUtils;
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.EditLocation;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.EditLocation
{
    internal class EditLocationResultsTest
    {
        [Test]
        public void Defaults()
        {
            var sut = new EditLocationResults();
            Assert.AreEqual("", sut.Zip);
            Assert.AreEqual(0, sut.NumCompletionEvents);
            Assert.AreEqual(0, sut.NumEvents);
            Assert.AreEqual(0, sut.NumLocations);
            Assert.AreEqual(Lists.NewList<RelativeEditLocation>(), sut.AppliedEditLocations);
            Assert.AreEqual(Lists.NewList<RelativeEditLocation>(), sut.OtherEditLocations);

            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new EditLocationResults
            {
                Zip = "a",
                NumCompletionEvents = 1,
                NumEvents = 3,
                NumLocations = 5,
                AppliedEditLocations =
                {
                    new RelativeEditLocation {Location = 1, Size = 2}
                },
                OtherEditLocations =
                {
                    new RelativeEditLocation {Location = 3, Size = 4}
                }
            };
            Assert.AreEqual("a", sut.Zip);
            Assert.AreEqual(1, sut.NumCompletionEvents);
            Assert.AreEqual(3, sut.NumEvents);
            Assert.AreEqual(5, sut.NumLocations);
            Assert.AreEqual(Lists.NewList(new RelativeEditLocation {Location = 1, Size = 2}), sut.AppliedEditLocations);
            Assert.AreEqual(Lists.NewList(new RelativeEditLocation {Location = 3, Size = 4}), sut.OtherEditLocations);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new EditLocationResults();
            var b = new EditLocationResults();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_WithValuesSet()
        {
            var a = new EditLocationResults
            {
                Zip = "a",
                NumCompletionEvents = 1,
                NumEvents = 3,
                NumLocations = 5,
                AppliedEditLocations =
                {
                    new RelativeEditLocation {Location = 1, Size = 2}
                },
                OtherEditLocations =
                {
                    new RelativeEditLocation {Location = 3, Size = 4}
                }
            };
            var b = new EditLocationResults
            {
                Zip = "a",
                NumCompletionEvents = 1,
                NumEvents = 3,
                NumLocations = 5,
                AppliedEditLocations =
                {
                    new RelativeEditLocation {Location = 1, Size = 2}
                },
                OtherEditLocations =
                {
                    new RelativeEditLocation {Location = 3, Size = 4}
                }
            };
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentZip()
        {
            var a = new EditLocationResults
            {
                Zip = "a"
            };
            var b = new EditLocationResults();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentNumCompletionEvents()
        {
            var a = new EditLocationResults
            {
                NumCompletionEvents = 1
            };
            var b = new EditLocationResults();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentNumEvents()
        {
            var a = new EditLocationResults
            {
                NumEvents = 3
            };
            var b = new EditLocationResults();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentNumHistoryTuples()
        {
            var a = new EditLocationResults
            {
                NumLocations = 5
            };
            var b = new EditLocationResults();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentAppliedEditLocations()
        {
            var a = new EditLocationResults
            {
                AppliedEditLocations =
                {
                    new RelativeEditLocation {Location = 1, Size = 2}
                }
            };
            var b = new EditLocationResults();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentOtherEditLocations()
        {
            var a = new EditLocationResults
            {
                OtherEditLocations =
                {
                    new RelativeEditLocation {Location = 1, Size = 2}
                }
            };
            var b = new EditLocationResults();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(new EditLocationResults());
        }

        [Test]
        public void Add()
        {
            var a = new EditLocationResults
            {
                Zip = "a",
                NumCompletionEvents = 1,
                NumEvents = 3,
                NumLocations = 5,
                AppliedEditLocations =
                {
                    new RelativeEditLocation {Location = 1, Size = 2}
                },
                OtherEditLocations =
                {
                    new RelativeEditLocation {Location = 3, Size = 4}
                }
            };
            a.Add(
                new EditLocationResults
                {
                    Zip = "b",
                    NumCompletionEvents = 1 + 1,
                    NumEvents = 3 + 1,
                    NumLocations = 5 + 1,
                    AppliedEditLocations =
                    {
                        new RelativeEditLocation {Location = 1, Size = 3}
                    },
                    OtherEditLocations =
                    {
                        new RelativeEditLocation {Location = 4, Size = 5}
                    }
                });
            var b = new EditLocationResults
            {
                Zip = "a",
                NumCompletionEvents = 2 * 1 + 1,
                NumEvents = 2 * 3 + 1,
                NumLocations = 2 * 5 + 1,
                AppliedEditLocations =
                {
                    new RelativeEditLocation {Location = 1, Size = 2},
                    new RelativeEditLocation {Location = 1, Size = 3}
                },
                OtherEditLocations =
                {
                    new RelativeEditLocation {Location = 3, Size = 4},
                    new RelativeEditLocation {Location = 4, Size = 5}
                }
            };
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }
    }
}