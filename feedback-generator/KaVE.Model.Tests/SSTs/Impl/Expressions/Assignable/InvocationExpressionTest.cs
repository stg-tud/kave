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

using System.Linq;
using KaVE.Model.Names;
using KaVE.Model.Names.CSharp;
using KaVE.Model.SSTs;
using KaVE.Model.SSTs.Expressions;
using KaVE.Model.SSTs.Expressions.Assignable;
using KaVE.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Model.SSTs.Impl.References;
using KaVE.Model.SSTs.Impl.Visitor;
using KaVE.Utils.Assertion;
using NUnit.Framework;

namespace KaVE.Model.Tests.SSTs.Impl.Expressions.Assignable
{
    public class InvocationExpressionTest
    {
        private TestVisitor _visitor;

        [SetUp]
        public void Setup()
        {
            _visitor = new TestVisitor();
        }

        [Test]
        public void DefaultValues()
        {
            var sut = new InvocationExpression();
            Assert.IsNull(sut.Reference);
            Assert.IsNull(sut.Name);
            Assert.IsNull(sut.Parameters);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var a = SSTUtil.InvocationExpression("a1", GetMethod("A2"), Ref("a3"));
            Assert.AreEqual(new VariableReference {Identifier = "a1"}, a.Reference);
            Assert.AreEqual(GetMethod("A2"), a.Name);
            Assert.AreEqual(new[] {Ref("a3")}, a.Parameters);
        }

        [Test]
        public void CustomConstructor_NonStatic()
        {
            var a = SSTUtil.InvocationExpression("a1", GetMethod("B1"), Ref("c1"));
            Assert.AreEqual("a1", a.Reference);
            Assert.AreEqual(GetMethod("B1"), a.Name);
            Assert.AreEqual(new[] {Ref("c1")}, a.Parameters);
        }

        [Test, ExpectedException(typeof (AssertException))]
        public void CustomConstructor_NonStaticAssert()
        {
            SSTUtil.InvocationExpression("a1", GetStaticMethod("B1"), Ref("c1"));
        }

        [Test]
        public void CustomConstructor_Static()
        {
            var a = SSTUtil.InvocationExpression(GetStaticMethod("B2"), Ref("c2"));
            Assert.AreEqual("", a.Reference);
            Assert.AreEqual(GetStaticMethod("B2"), a.Name);
            Assert.AreEqual(new[] {Ref("c2")}, a.Parameters);
        }

        [Test, ExpectedException(typeof (AssertException))]
        public void CustomConstructor_StaticAssert()
        {
            SSTUtil.InvocationExpression(GetMethod("B2"), Ref("c2"));
        }

        [Test]
        public void Equality_Default()
        {
            var a = new InvocationExpression();
            var b = new InvocationExpression();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            Assert.AreEqual(GetMethod("a"), GetMethod("a"));

            var a = SSTUtil.InvocationExpression("o", GetMethod("A"), Refs("a", "b", "c"));
            var b = SSTUtil.InvocationExpression("o", GetMethod("A"), Refs("a", "b", "c"));
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentIdentifier()
        {
            var a = new InvocationExpression {Reference = new VariableReference {Identifier = "a"}};
            var b = new InvocationExpression {Reference = new VariableReference {Identifier = "b"}};
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentMethod()
        {
            var a = new InvocationExpression {Name = GetMethod("A")};
            var b = new InvocationExpression {Name = GetMethod("B")};
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentParameters()
        {
            var a = new InvocationExpression {Parameters = new[] {Ref("a")}};
            var b = new InvocationExpression {Parameters = new[] {Ref("b")}};
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

        private static ISimpleExpression Ref(string id)
        {
            return new ReferenceExpression {Reference = new VariableReference {Identifier = id}};
        }

        private static ISimpleExpression[] Refs(params string[] ids)
        {
            return
                ids.Select<string, ISimpleExpression>(
                    id => new ReferenceExpression {Reference = new VariableReference {Identifier = id}}).ToArray();
        }

        [Test]
        public void VisitorIsImplemented()
        {
            var sut = new ComposedExpression();
            sut.Accept(_visitor, 13);
            Assert.AreEqual(sut, _visitor.Expr);
            Assert.AreEqual(13, _visitor.Context);
        }

        internal class TestVisitor : AbstractNodeVisitor<int>
        {
            public IExpressionCompletion Expr { get; private set; }
            public int Context { get; private set; }

            public override void Visit(IExpressionCompletion expr, int context)
            {
                Expr = expr;
                Context = context;
            }
        }
    }
}