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
 * 
 * Contributors:
 *    - Mattis Manfred Kämmerer
 */

using System;
using KaVE.FeedbackProcessor.Cleanup.Heuristics;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Cleanup.Heuristics
{
    [TestFixture]
    internal class TestVsProductionClassHeuristicTest
    {
        public static Object[] IsProductionClassTestSources =
        {
            new Object[] {"Foo.cs", true},
            new Object[] {"", false},
            new Object[] {"Class.txt", false},
            new Object[] {"ClassTest.c", false}
        };

        [Test, TestCaseSource("IsProductionClassTestSources")]
        public void ShouldCorrectlyIdentifyProductionClasses(string identifier, bool expectedValuation)
        {
            var actualValuation = TestVsProductionClassHeuristic.IsProductionClass(identifier);

            Assert.AreEqual(expectedValuation, actualValuation);
        }

        public static Object[] IsTestClassTestSources =
        {
            new Object[] {"Test.cs", true},
            new Object[] {"ATest.cs", true},
            new Object[] {"Test.cpp", true},
            new Object[] {"", false},
            new Object[] {"Test.txt", false},
            new Object[] {"TestA.c", false}
        };

        [Test, TestCaseSource("IsTestClassTestSources")]
        public void ShouldCorrectlyIdentifyTestClasses(string identifier, bool expectedValuation)
        {
            var actualValuation = TestVsProductionClassHeuristic.IsTestClass(identifier);

            Assert.AreEqual(expectedValuation, actualValuation);
        }
    }
}