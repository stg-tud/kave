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
 *    - Sven Amann
 */

using KaVE.Model.Names.CSharp;
using NUnit.Framework;

namespace KaVE.Model.Tests.Names.CSharp.Modularization
{
    [TestFixture]
    internal class AssemblyVersionTest
    {
        [Test]
        public void ShouldImplementIsUnknown()
        {
            Assert.That(AssemblyVersion.UnknownName.IsUnknown);
        }

        [Test]
        public void ShouldEqualsSameVersion()
        {
            var v1 = AssemblyVersion.Get("1.2.3.4");
            var v2 = AssemblyVersion.Get("1.2.3.4");

            Assert.AreEqual(v1, v2);
            Assert.IsTrue(v1 == v2);
        }

        [Test]
        public void ShouldParseVersion()
        {
            var uut = AssemblyVersion.Get("4.6.3.5");

            Assert.AreEqual(4, uut.Major);
            Assert.AreEqual(6, uut.Minor);
            Assert.AreEqual(3, uut.Build);
            Assert.AreEqual(5, uut.Revision);
        }

        [Test]
        public void ShouldBeGreaterThenPreviousVersions()
        {
            var assemblyVersion = AssemblyVersion.Get("4.3.2.1");

            Assert.IsTrue(AssemblyVersion.Get("3.4.2.1") < assemblyVersion);
            Assert.IsTrue(AssemblyVersion.Get("4.2.99.10") < assemblyVersion);
            Assert.IsTrue(AssemblyVersion.Get("4.3.1.23") < assemblyVersion);
            Assert.IsTrue(AssemblyVersion.Get("4.3.2.0") < assemblyVersion);
        }

        [Test]
        public void ShouldBeSmallerThenLaterVersions()
        {
            var assemblyVersion = AssemblyVersion.Get("4.3.2.1");

            Assert.IsTrue(AssemblyVersion.Get("5.3.2.1") > assemblyVersion);
            Assert.IsTrue(AssemblyVersion.Get("4.13.0.0") > assemblyVersion);
            Assert.IsTrue(AssemblyVersion.Get("4.3.1337.69") > assemblyVersion);
            Assert.IsTrue(AssemblyVersion.Get("4.3.2.666") > assemblyVersion);
        }

        [Test]
        public void ShouldBeUnknown()
        {
            var uut = AssemblyVersion.UnknownName;

            Assert.AreEqual(-1, uut.Major);
            Assert.AreEqual(-1, uut.Minor);
            Assert.AreEqual(-1, uut.Build);
            Assert.AreEqual(-1, uut.Revision);
        }
    }
}