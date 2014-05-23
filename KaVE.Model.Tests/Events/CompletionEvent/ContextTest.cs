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
 *    - Sebastian Proksch
 */

using System.Collections.Generic;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names;
using KaVE.Model.Names.CSharp;
using NUnit.Framework;

namespace KaVE.Model.Tests.Events.CompletionEvent
{
    [TestFixture]
    internal class ContextTest
    {
        [Test]
        public void HashCodeAndEquals_NewContextObjects()
        {
            var a = new Context();
            var b = new Context();
            Assert.AreEqual(a, b);
            Assert.IsTrue(a.GetHashCode() == b.GetHashCode());
        }

        [Test]
        public void HashCodeAndEquals_EnclosingMethod()
        {
            var a = new Context {EnclosingMethod = M("A")};
            var b = new Context {EnclosingMethod = M("A")};
            Assert.AreEqual(a, b);
            Assert.IsTrue(a.GetHashCode() == b.GetHashCode());
        }

        [Test]
        public void HashCodeAndEquals_EnclosingMethodDiff()
        {
            var a = new Context {EnclosingMethod = M("A")};
            var b = new Context {EnclosingMethod = M("B")};
            Assert.AreNotEqual(a, b);
            Assert.IsFalse(a.GetHashCode() == b.GetHashCode());
        }

        [Test]
        public void HashCodeAndEquals_TypeShape()
        {
            var a = new Context {TypeShape = TS("A")};
            var b = new Context {TypeShape = TS("A")};
            Assert.AreEqual(a, b);
            Assert.IsTrue(a.GetHashCode() == b.GetHashCode());
        }

        [Test]
        public void HashCodeAndEquals_TypeShapeDiff()
        {
            var a = new Context {TypeShape = TS("A")};
            var b = new Context {TypeShape = TS("B")};
            Assert.AreNotEqual(a, b);
            Assert.IsFalse(a.GetHashCode() == b.GetHashCode());
        }

        [Test]
        public void HashCodeAndEquals_TriggerTarget()
        {
            var a = new Context {TriggerTarget = Name.Get("A")};
            var b = new Context {TriggerTarget = Name.Get("A")};
            Assert.AreEqual(a, b);
            Assert.IsTrue(a.GetHashCode() == b.GetHashCode());
        }

        [Test]
        public void HashCodeAndEquals_TriggerTargetDiff()
        {
            var a = new Context {TriggerTarget = Name.Get("A")};
            var b = new Context {TriggerTarget = Name.Get("B")};
            Assert.AreNotEqual(a, b);
            Assert.IsFalse(a.GetHashCode() == b.GetHashCode());
        }

        [Test]
        public void HashCodeAndEquals_CalledMethods()
        {
            var a = new Context();
            a.EntryPointToCalledMethods.Add(M("A"), new HashSet<IMethodName> {M("C")});
            var b = new Context();
            b.EntryPointToCalledMethods.Add(M("A"), new HashSet<IMethodName> {M("C")});
            Assert.AreEqual(a, b);
            Assert.IsTrue(a.GetHashCode() == b.GetHashCode());
        }

        [Test]
        public void HashCodeAndEquals_CalledMethodsDiff()
        {
            var a = new Context();
            a.EntryPointToCalledMethods.Add(M("A"), new HashSet<IMethodName> {M("C")});
            var b = new Context();
            b.EntryPointToCalledMethods.Add(M("B"), new HashSet<IMethodName> {M("C")});
            Assert.AreNotEqual(a, b);
            Assert.IsFalse(a.GetHashCode() == b.GetHashCode());
        }

        [Test]
        public void HashCodeAndEquals_CalledMethodsDiff2()
        {
            var a = new Context();
            a.EntryPointToCalledMethods.Add(M("A"), new HashSet<IMethodName> {M("C")});
            var b = new Context();
            b.EntryPointToCalledMethods.Add(M("A"), new HashSet<IMethodName> {M("D")});
            Assert.AreNotEqual(a, b);
            Assert.IsFalse(a.GetHashCode() == b.GetHashCode());
        }

        [Test]
        public void HashCodeAndEquals_Groums()
        {
            var a = new Context();
            a.EntryPointToGroum.Add(M("A"), G(1));
            var b = new Context();
            b.EntryPointToGroum.Add(M("A"), G(1));
            Assert.AreEqual(a, b);
            Assert.IsTrue(a.GetHashCode() == b.GetHashCode());
        }

        [Test]
        public void HashCodeAndEquals_GroumsDiff()
        {
            var a = new Context();
            a.EntryPointToGroum.Add(M("A"), G(1));
            var b = new Context();
            b.EntryPointToGroum.Add(M("B"), G(1));
            Assert.AreNotEqual(a, b);
            Assert.IsFalse(a.GetHashCode() == b.GetHashCode());
        }

        [Test]
        public void HashCodeAndEquals_GroumsDiff2()
        {
            var a = new Context();
            a.EntryPointToGroum.Add(M("A"), G(1));
            var b = new Context();
            b.EntryPointToGroum.Add(M("A"), G(2));
            Assert.AreNotEqual(a, b);
            Assert.IsFalse(a.GetHashCode() == b.GetHashCode());
        }

        public static IMethodName M(string methodName)
        {
            return
                MethodName.Get(
                    "[System.String, mscore, 4.0.0.0] [MyType, MyAssembly, 1.0.0.0]." + methodName +
                    "(opt [System.Int32, mscore, 4.0.0.0] length)");
        }

        private static Model.Groums.Groum G(int s)
        {
            return new Model.Groums.Groum {Name = s};
        }

        // ReSharper disable once InconsistentNaming
        private static TypeShape TS(string type)
        {
            return new TypeShape {TypeHierarchy = new TypeHierarchy(type)};
        }
    }
}