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

using KaVE.Commons.Model.Naming.Impl.v0.Types.Organization;
using KaVE.Commons.Utils.Exceptions;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Naming.Impl.v0.Types.Organization
{
    internal class AssemblyVersionTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new AssemblyVersion();
            Assert.AreEqual(-1, sut.Major);
            Assert.AreEqual(-1, sut.Minor);
            Assert.AreEqual(-1, sut.Build);
            Assert.AreEqual(-1, sut.Revision);
            Assert.IsTrue(sut.IsUnknown);
        }

        [Test]
        public void ShouldParseNumbers()
        {
            var sut = new AssemblyVersion("1.2.3.4");
            Assert.AreEqual(1, sut.Major);
            Assert.AreEqual(2, sut.Minor);
            Assert.AreEqual(3, sut.Build);
            Assert.AreEqual(4, sut.Revision);
            Assert.IsFalse(sut.IsUnknown);
        }

        [Test]
        public void ShouldParseNumbers_Zeros()
        {
            var sut = new AssemblyVersion("0.0.0.0");
            Assert.AreEqual(0, sut.Major);
            Assert.AreEqual(0, sut.Minor);
            Assert.AreEqual(0, sut.Build);
            Assert.AreEqual(0, sut.Revision);
            Assert.IsFalse(sut.IsUnknown);
        }

        [Test]
        public void ShouldParseNumbersMultiDigit()
        {
            var sut = new AssemblyVersion("11.22.33.44");
            Assert.AreEqual(11, sut.Major);
            Assert.AreEqual(22, sut.Minor);
            Assert.AreEqual(33, sut.Build);
            Assert.AreEqual(44, sut.Revision);
            Assert.IsFalse(sut.IsUnknown);
        }

        [Test]
        public void ShouldRecognizeUnknownName()
        {
            Assert.True(new AssemblyVersion().IsUnknown);
            Assert.True(new AssemblyVersion("???").IsUnknown);
            Assert.False(new AssemblyVersion("1.2.3.4").IsUnknown);
        }

        [Test, ExpectedException(typeof(ValidationException))]
        public void ShouldAvoidNullParameters()
        {
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            new AssemblyVersion(null);
        }

        [Test, ExpectedException(typeof(ValidationException))]
        public void ShouldRejectIdentifiersWithInsufficientNumberOfDots()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new AssemblyVersion("1.2.3");
        }

        [Test, ExpectedException(typeof(ValidationException))]
        public void ShouldRejectIdentifiers_WithNonNumbers()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new AssemblyVersion("1.2.3.a");
        }

        [Test]
        public void ShouldBeGreaterThenPreviousVersions()
        {
            Assert.IsTrue(V("0.2.3.4") < V("1.2.3.4"));
            Assert.IsTrue(V("1.1.3.4") < V("1.2.3.4"));
            Assert.IsTrue(V("1.2.2.4") < V("1.2.3.4"));
            Assert.IsTrue(V("1.2.3.3") < V("1.2.3.4"));
        }

        [Test]
        public void ShouldEqualsSameVersion()
        {
            var v1 = V("1.2.3.4");
            var v2 = V("1.2.3.4");

            Assert.AreEqual(v1, v2);
            Assert.IsTrue(v1 <= v2);
            Assert.IsTrue(v1 == v2);
            Assert.IsTrue(v1 >= v2);
        }

        [Test]
        public void ShouldBeSmallerThenLaterVersions()
        {
            Assert.IsTrue(V("2.2.3.4") > V("1.2.3.4"));
            Assert.IsTrue(V("1.3.3.4") > V("1.2.3.4"));
            Assert.IsTrue(V("1.2.4.4") > V("1.2.3.4"));
            Assert.IsTrue(V("1.2.3.5") > V("1.2.3.4"));
        }

        private static AssemblyVersion V(string id)
        {
            return new AssemblyVersion(id);
        }
    }
}