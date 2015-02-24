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

using KaVE.Model.Names;
using KaVE.Model.Names.CSharp;
using KaVE.Model.SSTs.Expressions;
using KaVE.Model.SSTs.Expressions.Basic;
using KaVE.Model.SSTs.Statements.Wrapped;
using NUnit.Framework;

namespace KaVE.Model.Tests.SSTs.Statements.Wrapped
{
    public class InvocationStatementTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new InvocationStatement();
            Assert.Null(sut.Target);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues_Helper1()
        {
            var sut = InvocationStatement.Create("a", GetMethod("A"), Ref("b"));
            Assert.AreEqual(InvocationExpression.Create("a", GetMethod("A"), Ref("b")), sut.Target);
        }

        [Test]
        public void SettingValues_Helper2()
        {
            var sut = InvocationStatement.Create(GetStaticMethod("A"), Ref("b"));
            Assert.AreEqual(InvocationExpression.Create(GetStaticMethod("A"), Ref("b")), sut.Target);
        }

        [Test]
        public void TargetDoesNotChange()
        {
            var sut = InvocationStatement.Create(GetStaticMethod("A"), Ref("b"));
            Assert.AreSame(sut.Invocation, sut.Target);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new InvocationStatement();
            var b = new InvocationStatement();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = InvocationStatement.Create(GetStaticMethod("A"), Ref("b"));
            var b = InvocationStatement.Create(GetStaticMethod("A"), Ref("b"));
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentInvocation()
        {
            var a = InvocationStatement.Create(GetStaticMethod("A"), Ref("b"));
            var b = InvocationStatement.Create(GetStaticMethod("B"), Ref("b"));
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        private static IMethodName GetMethod(string simpleName)
        {
            var methodName = string.Format(
                "[System.String, mscore, 4.0.0.0] [System.String, mscore, 4.0.0.0].{0}()",
                simpleName);
            return MethodName.Get(methodName);
        }

        private static IMethodName GetStaticMethod(string simpleName)
        {
            var methodName = string.Format(
                "static [System.String, mscore, 4.0.0.0] [System.String, mscore, 4.0.0.0].{0}()",
                simpleName);
            return MethodName.Get(methodName);
        }

        private static BasicExpression Ref(string id)
        {
            return new ReferenceExpression {Identifier = id};
        }
    }
}