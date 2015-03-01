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
using KaVE.Model.SSTs.Impl;
using KaVE.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Model.SSTs.Impl.References;
using KaVE.Model.SSTs.Impl.Visitor;
using NUnit.Framework;

namespace KaVE.Model.Tests.SSTs
{
    public class InvocationTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new Invocation();
            Assert.IsNull(sut.Reference);
            Assert.IsNull(sut.MethodName);
            Assert.IsNotNull(sut.Parameters);
            Assert.AreEqual(0, sut.Parameters.Count);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void Equality_Default()
        {
            var a = new Invocation();
            var b = new Invocation();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            Assert.AreEqual(GetMethod("a"), GetMethod("a"));

            var a = SSTUtil.InvocationExpression("o", GetMethod("A"), RefExprs("a", "b", "c"));
            var b = SSTUtil.InvocationExpression("o", GetMethod("A"), RefExprs("a", "b", "c"));
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentIdentifier()
        {
            var a = new Invocation {Reference = new VariableReference {Identifier = "a"}};
            var b = new Invocation {Reference = new VariableReference {Identifier = "b"}};
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentMethod()
        {
            var a = new Invocation {MethodName = GetMethod("A")};
            var b = new Invocation {MethodName = GetMethod("B")};
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentParameters()
        {
            var a = new Invocation {Parameters = RefExprs("a")};
            var b = new Invocation {Parameters = RefExprs("b")};
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

        private static ISimpleExpression[] RefExprs(params string[] ids)
        {
            return
                ids.Select<string, ISimpleExpression>(
                    id => new ReferenceExpression {Reference = new VariableReference {Identifier = id}}).ToArray();
        }

        [Test]
        public void VisitorIsImplemented()
        {
            var sut = new Invocation();
            var visitor = new TestVisitor();
            sut.Accept(visitor, 4);

            Assert.AreEqual(sut, visitor.Expr);
            Assert.AreEqual(4, visitor.Context);
        }

        internal class TestVisitor : AbstractNodeVisitor<int>
        {
            public IInvocation Expr { get; private set; }
            public int Context { get; private set; }

            public override void Visit(IInvocation expr, int context)
            {
                Expr = expr;
                Context = context;
            }
        }
    }
}