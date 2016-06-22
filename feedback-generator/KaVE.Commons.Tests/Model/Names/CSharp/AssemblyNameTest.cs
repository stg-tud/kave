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

using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Names.CSharp
{
    internal class AssemblyNameTest
    {
        [Test]
        public void HappyPath()
        {
            var n = CsNameUtil.GetAssemblyName("A, 1.2.3.4");
            AssertName(n, "A");
            AssertVersion(n, "1.2.3.4");
        }

        [Test]
        public void NoVersion()
        {
            var n = CsNameUtil.GetAssemblyName("A");
            AssertName(n, "A");
            AssertVersion(n, "?");
        }

        [Test]
        public void KommasInName()
        {
            var n = CsNameUtil.GetAssemblyName("A (B, C)");
            AssertName(n, "A (B, C)");
            AssertVersion(n, AssemblyVersion.UnknownName.Identifier);
        }

        [Test]
        public void KommasInNameAndVersion()
        {
            var n = CsNameUtil.GetAssemblyName("A (B, C), 1.2.3.4");
            AssertName(n, "A (B, C)");
            AssertVersion(n, "1.2.3.4");
        }

        [Test]
        public void LotOfWhitespace()
        {
            var n = CsNameUtil.GetAssemblyName(" A , 1.2.3.4");
            AssertName(n, "A");
            AssertVersion(n, "1.2.3.4");
        }

        [Test]
        public void NoWhitespace()
        {
            var n = CsNameUtil.GetAssemblyName("A,1.2.3.4");
            AssertName(n, "A");
            AssertVersion(n, "1.2.3.4");
        }

        // Clustering (K-Means, MeanShift)


        private static void AssertName(IAssemblyName assemblyName, string expected)
        {
            var actual = assemblyName.Name;
            Assert.AreEqual(expected, actual);
        }

        private static void AssertVersion(IAssemblyName assemblyName, string version)
        {
            var actual = assemblyName.AssemblyVersion;
            Assert.AreEqual(version, actual.Identifier);
        }
    }
}