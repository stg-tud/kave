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
 *    - 
 */

using KaVE.Model.Names.CSharp;
using KaVE.Model.SSTs.Declarations;
using KaVE.Model.SSTs.Visitor;
using Moq;
using NUnit.Framework;

namespace KaVE.Model.Tests.SSTs.Declarations
{
    internal class FieldDeclarationTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new FieldDeclaration();
            Assert.Null(sut.Name);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new FieldDeclaration {Name = FieldName.UnknownName};
            Assert.AreEqual(FieldName.UnknownName, sut.Name);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new FieldDeclaration();
            var b = new FieldDeclaration();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyEquals()
        {
            var a = new FieldDeclaration {Name = FieldName.UnknownName};
            var b = new FieldDeclaration {Name = FieldName.UnknownName};
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentType()
        {
            var a = new FieldDeclaration {Name = FieldName.UnknownName};
            var b = new FieldDeclaration();

            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void VisitorImplementation()
        {
            var sut = new FieldDeclaration();
            var @visitor = new Mock<ISSTNodeVisitor<object>>();
            var context = new object();

            sut.Accept(@visitor.Object, context);

            @visitor.Verify(v => v.Visit(sut, context));
        }
    }
}