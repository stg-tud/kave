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
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Utils.ObjectUsageExport;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.ObjectUsageExport
{
    internal class ScopedEnclosingsTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new ScopedEnclosings();
            Assert.Null(sut.Parent);
            Assert.AreEqual(Names.UnknownType, sut.Type);
            Assert.AreEqual(Names.UnknownMethod, sut.Method);
        }

        [Test]
        public void HierarchyCanBeCreated()
        {
            var parent = new ScopedEnclosings();
            var sut = new ScopedEnclosings(parent);
            Assert.AreEqual(parent, sut.Parent);
        }

        [Test]
        public void EnclosingsCanBeSet()
        {
            var sut = new ScopedEnclosings
            {
                Type = Type("T"),
                Method = Method("M")
            };
            Assert.AreEqual(Type("T"), sut.Type);
            Assert.AreEqual(Method("M"), sut.Method);
        }

        [Test]
        public void EnclosingsAreInferedFromHierarchy()
        {
            var parent = new ScopedEnclosings
            {
                Type = Type("T"),
                Method = Method("M")
            };
            var sut = new ScopedEnclosings(parent);

            Assert.AreEqual(Type("T"), sut.Type);
            Assert.AreEqual(Method("M"), sut.Method);
        }

        [Test]
        public void SettingEnclosingsDoesNotAffectHierarchy()
        {
            var parent = new ScopedEnclosings
            {
                Type = Type("T"),
                Method = Method("M")
            };
            var sut = new ScopedEnclosings(parent)
            {
                Type = Type("T2"),
                Method = Method("M2")
            };

            Assert.AreEqual(Type("T"), parent.Type);
            Assert.AreEqual(Method("M"), parent.Method);

            Assert.AreEqual(Type("T2"), sut.Type);
            Assert.AreEqual(Method("M2"), sut.Method);
        }

        private static IMethodName Method(string m)
        {
            var methodName = string.Format("[{0}] [{1}].{2}()", Type("R"), Type("D"), m);
            return Names.Method(methodName);
        }

        private static ITypeName Type(string typeName)
        {
            return Names.Type(typeName + ",P");
        }
    }
}