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

using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.Impl.v0.Types.Organization;
using KaVE.Commons.Model.Naming.Types.Organization;
using KaVE.Commons.Utils.Exceptions;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Naming.Impl.v0.Types.Organization
{
    internal class AssemblyNameTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new AssemblyName();
            Assert.IsFalse(sut.IsLocalProject);
            Assert.AreEqual("???", sut.Name);
            Assert.AreEqual(new AssemblyVersion(), sut.Version);
            Assert.IsTrue(sut.IsUnknown);
        }

        [Test]
        public void HappyPath_Assembly()
        {
            var sut = new AssemblyName("A, 1.2.3.4");
            Assert.IsFalse(sut.IsLocalProject);
            Assert.AreEqual("A", sut.Name);
            Assert.AreEqual(new AssemblyVersion("1.2.3.4"), sut.Version);
            Assert.IsFalse(sut.IsUnknown);
        }

        [Test]
        public void HappyPath_LocalProject()
        {
            var sut = new AssemblyName("P");
            Assert.IsTrue(sut.IsLocalProject);
            Assert.AreEqual("P", sut.Name);
            Assert.AreEqual(new AssemblyVersion(), sut.Version);
            Assert.IsFalse(sut.IsUnknown);
        }

        [Test]
        public void AnUnknownVersionDoesNotMakeALocalProject()
        {
            Assert.IsFalse(new AssemblyName("P, -1.-1.-1.-1").IsLocalProject);
        }

        [ExpectedException(typeof(ValidationException)), //
         TestCase("("), TestCase(")"), //
         TestCase("["), TestCase("]"), //
         TestCase("{"), TestCase("}"), //
         TestCase(","), TestCase(";"), TestCase(":"), TestCase(" ")]
        public void SpecialCharsInNameAreNotAllowed(string specialChar)
        {
            // ReSharper disable once ObjectCreationAsStatement
            new AssemblyName("a" + specialChar + "b, 1.2.3.4");
        }

        [Test, ExpectedException(typeof(ValidationException))]
        public void VersionNeedsToBeParseable()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new AssemblyName("P, 1.2.3.4a");
        }

        [Test]
        public void LotOfWhitespace()
        {
            var n = new AssemblyName(" A , 1.2.3.4");
            AssertName(n, "A");
            AssertVersion(n, "1.2.3.4");
        }

        [Test]
        public void NoWhitespace()
        {
            var n = new AssemblyName("A,1.2.3.4");
            AssertName(n, "A");
            AssertVersion(n, "1.2.3.4");
        }

        [Test]
        public void ShouldImplementIsUnknown()
        {
            Assert.That(new AssemblyName().IsUnknown);
        }

        [Test]
        public void ShouldBeMSCorLibAssembly()
        {
            const string identifier = "mscorlib, 4.0.0.0";
            var mscoreAssembly = new AssemblyName(identifier);

            Assert.AreEqual("mscorlib", mscoreAssembly.Name);
            Assert.AreEqual("4.0.0.0", mscoreAssembly.Version.Identifier);
            Assert.AreEqual(identifier, mscoreAssembly.Identifier);
        }

        [Test]
        public void MscorlibIsNotLocal()
        {
            Assert.False(Names.Type("p:int").Assembly.IsLocalProject);
        }

        private static void AssertName(IAssemblyName assemblyName, string expected)
        {
            var actual = assemblyName.Name;
            Assert.AreEqual(expected, actual);
        }

        private static void AssertVersion(IAssemblyName assemblyName, string expectedVersion)
        {
            var actual = assemblyName.Version;
            Assert.AreEqual(new AssemblyVersion(expectedVersion), actual);
        }
    }
}