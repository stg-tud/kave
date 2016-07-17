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

using KaVE.Commons.Model.Naming.Impl.v0.Types;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Naming.Impl.v0.Types
{
    internal class AssemblyVersionTest
    {
        [Test]
        public void ShouldImplementIsUnknown()
        {
            Assert.That(new AssemblyVersion().IsUnknown);
        }

        [Test]
        public void ShouldEqualsSameVersion()
        {
            var v1 = new AssemblyVersion("1.2.3.4");
            var v2 = new AssemblyVersion("1.2.3.4");

            Assert.AreEqual(v1, v2);
            Assert.IsTrue(v1 == v2);
        }

        [Test]
        public void ShouldParseVersion()
        {
            var uut = new AssemblyVersion("4.6.3.5");

            Assert.AreEqual(4, uut.Major);
            Assert.AreEqual(6, uut.Minor);
            Assert.AreEqual(3, uut.Build);
            Assert.AreEqual(5, uut.Revision);
        }

        [Test]
        public void ShouldBeGreaterThenPreviousVersions()
        {
            var assemblyVersion = new AssemblyVersion("4.3.2.1");

            Assert.IsTrue(new AssemblyVersion("3.4.2.1") < assemblyVersion);
            Assert.IsTrue(new AssemblyVersion("4.2.99.10") < assemblyVersion);
            Assert.IsTrue(new AssemblyVersion("4.3.1.23") < assemblyVersion);
            Assert.IsTrue(new AssemblyVersion("4.3.2.0") < assemblyVersion);
        }

        [Test]
        public void ShouldBeSmallerThenLaterVersions()
        {
            var assemblyVersion = new AssemblyVersion("4.3.2.1");

            Assert.IsTrue(new AssemblyVersion("5.3.2.1") > assemblyVersion);
            Assert.IsTrue(new AssemblyVersion("4.13.0.0") > assemblyVersion);
            Assert.IsTrue(new AssemblyVersion("4.3.1337.69") > assemblyVersion);
            Assert.IsTrue(new AssemblyVersion("4.3.2.666") > assemblyVersion);
        }

        [Test]
        public void ShouldBeUnknown()
        {
            var uut = new AssemblyVersion();

            Assert.AreEqual(-1, uut.Major);
            Assert.AreEqual(-1, uut.Minor);
            Assert.AreEqual(-1, uut.Build);
            Assert.AreEqual(-1, uut.Revision);
        }
    }
}