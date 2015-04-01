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

using KaVE.Model.SSTs.Impl;
using KaVE.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Model.SSTs.Impl.References;
using KaVE.Model.SSTs.Impl.Statements;
using NUnit.Framework;

namespace KaVE.Model.Tests.SSTs.Impl.Statements
{
    public class AssignmentTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new Assignment();
            Assert.AreEqual(new UnknownReference(), sut.Reference);
            Assert.AreEqual(new UnknownExpression(), sut.Expression);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new Assignment
            {
                Reference = new VariableReference {Identifier = "x"},
                Expression = new ConstantValueExpression()
            };
            Assert.AreEqual(new VariableReference {Identifier = "x"}, sut.Reference);
            Assert.AreEqual(new ConstantValueExpression(), sut.Expression);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new Assignment();
            var b = new Assignment();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = SSTUtil.AssignmentToLocal("a", new ConstantValueExpression());
            var b = SSTUtil.AssignmentToLocal("a", new ConstantValueExpression());
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentReference()
        {
            var a = SSTUtil.AssignmentToLocal("a", new ConstantValueExpression());
            var b = SSTUtil.AssignmentToLocal("b", new ConstantValueExpression());
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentExpression()
        {
            var a = SSTUtil.AssignmentToLocal("a", new ConstantValueExpression());
            var b = SSTUtil.AssignmentToLocal("a", new ComposedExpression());
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void VisitorIsImplemented()
        {
            var sut = new Assignment();
            sut.Accept(23).Verify(v => v.Visit(sut, 23));
        }

        [Test]
        public void VisitorWithReturnIsImplemented()
        {
            var sut = new Assignment();
            sut.Accept(23).VerifyWithReturn(v => v.Visit(sut, 23));
        }
    }
}