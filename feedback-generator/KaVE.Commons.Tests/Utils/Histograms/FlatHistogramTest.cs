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
using KaVE.Commons.Utils.Assertion;
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
        public void A()
        {
            _sut.Add(1, 3);
            AssertBins(1.0, 0.0, 0.0);
        }

        [Test]
        public void B()
        {
            _sut.Add(2, 3);
            AssertBins(0.0, 1.0, 0.0);
        }

        [Test]
        public void C()
        {
            _sut.Add(1, 2);
            AssertBins(0.666, 0.333, 0.0);
        }

        [Test]
        public void C2()
        {
            _sut.Add(1, 2);
            _sut.Add(2, 3);
            AssertBins(0.666, 1.333, 0.0);
        }

        [Test]
        public void C3()
        {
            _sut.Add(1, 1);
            AssertBins(0.333, 0.333, 0.333);
        }

        [Test]
        public void C4()
        {
            _sut = new FlatHistogram(5);
            _sut.Add(1, 2);
            AssertBins(0.4, 0.4, 0.2, 0.0, 0.0);
        }

        [Test]
        public void D()
        {
            _sut.Add(1, 4);
            AssertBins(1.0, 0.0, 0.0);
        }

        [Test]
        public void E()
        {
            _sut.Add(2, 4);
            AssertBins(0.333, 0.666, 0.0);
        }

        [Test]
        public void F()
        {
            _sut.Add(4, 4);
            AssertBins(0.0, 0.0, 1.0);
        }

        [Test, ExpectedException(typeof (AssertException))]
        public void BinTooSmall()
        {
            _sut.Add(0, 4);
        }

        [Test, ExpectedException(typeof (AssertException))]
        public void BinTooLarge()
        {
            _sut.Add(5, 4);
        }

        [Test, ExpectedException(typeof (AssertException))]
        public void SizeTooSmall()
        {
            _sut.Add(5, 0);
        }

        [Test]
        public void GetSize()
        {
            Assert.AreEqual(0, _sut.GetSize());
            _sut.Add(1, 2);
            Assert.AreEqual(1, _sut.GetSize());
            _sut.Add(3, 5);
            Assert.AreEqual(2, _sut.GetSize());
            _sut.Add(4, 4);
            Assert.AreEqual(3, _sut.GetSize());
        }

        [Test]
        public void GetSizeLarge()
        {
            _sut = new FlatHistogram(10);
            var r = new Random();
            for (var i = 0; i < 1000; i++)
            {
                var methodSize = r.Next(30);
                var bin = r.Next(methodSize);
                // implementation is one-based
                _sut.Add(bin + 1, methodSize + 1);
            }
            Assert.AreEqual(1000, _sut.GetSize());
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