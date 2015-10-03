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

using KaVE.Commons.Utils.Histograms;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.Histograms
{
    internal class FlatHistogramTest
    {
        private FlatHistogram _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new FlatHistogram(3);
        }

        [Test]
        public void a()
        {
            _sut.Add(1, 3);
            AssertBins(1.0, 0.0, 0.0);
        }

        [Test]
        public void b()
        {
            _sut.Add(2, 3);
            AssertBins(0.0, 1.0, 0.0);
        }

        [Test]
        public void c()
        {
            _sut.Add(1, 2);
            AssertBins(0.666, 0.333, 0.0);
        }

        [Test]
        public void c2()
        {
            _sut.Add(1, 2);
            _sut.Add(2, 3);
            AssertBins(0.666, 1.333, 0.0);
        }

        [Test]
        public void d()
        {
            _sut.Add(2, 4);
            AssertBins(0.333, 0.666, 0.0);
        }

        [Test]
        public void ToStringTest()
        {
            _sut.Add(2, 4);

            var actual = _sut.ToString();
            var expected = "1:  33.3% (0.33)\n" +
                           "2:  66.7% (0.67)\n" + "" +
                           "3:   0.0% (0.00)\n" +
                           "(based on 1 values)\n";
            Assert.AreEqual(expected, actual);
        }

        private void AssertBins(params double[] expecteds)
        {
            double[] actuals = _sut.GetBins();
            Assert.AreEqual(expecteds.Length, actuals.Length);
            for (var i = 0; i < expecteds.Length; i++)
            {
                Assert.AreEqual(expecteds[i], actuals[i], 0.001);
            }
        }
    }
}