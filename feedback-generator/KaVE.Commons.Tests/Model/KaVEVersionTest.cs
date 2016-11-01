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
using KaVE.Commons.Model;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model
{
    public class KaVEVersionTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new KaVEVersion();
            Assert.AreEqual(new Version(0, 0), sut.Version);
            Assert.AreEqual(Variant.Unknown, sut.Variant);
            Assert.AreEqual(0, sut.KaVEVersionNumber);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new KaVEVersion
            {
                KaVEVersionNumber = 123,
                Variant = Variant.Development
            };
            Assert.AreEqual(new Version(0, 123), sut.Version);
            Assert.AreEqual(Variant.Development, sut.Variant);
            Assert.AreEqual(123, sut.KaVEVersionNumber);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new KaVEVersion();
            var b = new KaVEVersion();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new KaVEVersion
            {
                KaVEVersionNumber = 123,
                Variant = Variant.Development
            };
            var b = new KaVEVersion
            {
                KaVEVersionNumber = 123,
                Variant = Variant.Development
            };
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentVersionNumber()
        {
            var a = new KaVEVersion
            {
                KaVEVersionNumber = 123
            };
            var b = new KaVEVersion();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentVariant()
        {
            var a = new KaVEVersion
            {
                Variant = Variant.Development
            };
            var b = new KaVEVersion();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void ToStringIsImplemented()
        {
            var sut = new KaVEVersion
            {
                Variant = Variant.Development,
                KaVEVersionNumber = 1234
            };
            Assert.AreEqual("0.1234-Development", sut.ToString());
        }
    }
}