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
using KaVE.Commons.Utils.CodeCompletion.Impl;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.CodeCompletion.Impl
{
    internal class ExamplePBNRecommenderTest
    {
        private ExamplePBNRecommender _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new ExamplePBNRecommender();
            //_sut.GetNetwork().WriteFile(@"C:\Users\seb\Desktop\networks\exampleNetwork.xdsl");
        }

        [Test]
        public void ProbabilitiesWithoutSelection()
        {
            var actual = _sut.GetProbabilities();
            var expected = new[]
            {
                _("Init", 0.55),
                _("Execute", 0.48),
                _("Finish", 0.43)
            };
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void UnknownSelection()
        {
            _sut.Set("XYZ");
        }

        [Test]
        public void ProbabilitiesWithSelection()
        {
            _sut.Set("Init");

            var actual = _sut.GetProbabilities();
            var expected = new[]
            {
                _("Execute", 0.64),
                _("Finish", 0.15)
            };
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ResetWorks()
        {
            _sut.Set("Init");
            _sut.Reset();

            var actual = _sut.GetProbabilities();
            var expected = new[]
            {
                _("Init", 0.55),
                _("Execute", 0.48),
                _("Finish", 0.43)
            };
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ProposalsAreOrdered()
        {
            _sut.Set("Finish");

            var actual = _sut.GetProbabilities();
            var expected = new[]
            {
                _("Execute", 0.28),
                _("Init", 0.20)
            };
            Assert.AreEqual(expected, actual);
        }

        private Tuple<string, double> _(string name, double prob)
        {
            return new Tuple<string, double>(name, prob);
        }
    }
}