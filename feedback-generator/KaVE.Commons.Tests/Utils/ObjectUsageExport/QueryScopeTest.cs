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

using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.ObjectUsageExport;
using Moq;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.ObjectUsageExport
{
    internal class QueryScopeTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new QueryScope();
            Assert.False(sut.IsExisting(TypeName.UnknownName));
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var parentMock = new QueryScope();

            var sut = new QueryScope(parentMock);
            Assert.AreEqual(parentMock, sut.ParentScope);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void IsExisting_Type()
        {
            var parent = new QueryScope();
            var sut = new QueryScope(parent);

            parent.Register(Type("A"), new Query());
            sut.Register(Type("B"), new Query());

            Assert.True(sut.IsExisting(Type("A")));
            Assert.False(sut.IsExistingInCurrentScope(Type("A")));
            Assert.True(sut.IsExisting(Type("B")));
            Assert.True(sut.IsExistingInCurrentScope(Type("B")));

            Assert.True(parent.IsExisting(Type("A")));
            Assert.True(parent.IsExistingInCurrentScope(Type("A")));
        }

        [Test]
        public void IsExisting_Id()
        {
            var parent = new QueryScope();
            var sut = new QueryScope(parent);

            parent.Register("a", new Query());
            sut.Register("b", new Query());

            Assert.True(sut.IsExisting("a"));
            Assert.False(sut.IsExistingInCurrentScope("a"));
            Assert.True(sut.IsExisting("b"));
            Assert.True(sut.IsExistingInCurrentScope("b"));

            Assert.True(parent.IsExisting("a"));
            Assert.True(parent.IsExistingInCurrentScope("a"));
        }

        [Test]
        public void RegisterType()
        {
            var q = SomeQuery();

            var sut = new QueryScope();
            sut.Register(TypeName.UnknownName, q);

            Assert.True(sut.IsExisting(TypeName.UnknownName));
            Assert.AreEqual(q, sut.Find(TypeName.UnknownName));
        }

        [Test]
        public void RegisterId()
        {
            var q = SomeQuery();

            var sut = new QueryScope();
            sut.Register("x", q);

            Assert.True(sut.IsExisting("x"));
            Assert.AreEqual(q, sut.Find("x"));
        }

        [Test]
        public void LookupWorksInParentScope_Type()
        {
            var q = SomeQuery();

            var parent = new QueryScope();
            parent.Register(TypeName.UnknownName, q);
            var sut = new QueryScope(parent);

            Assert.True(sut.IsExisting(TypeName.UnknownName));
            Assert.AreEqual(q, sut.Find(TypeName.UnknownName));
        }

        [Test]
        public void LookupWorksInParentScope_Id()
        {
            var q = SomeQuery();

            var parent = new QueryScope();
            parent.Register("x", q);
            var sut = new QueryScope(parent);

            Assert.True(sut.IsExisting("x"));
            Assert.AreEqual(q, sut.Find("x"));
        }

        [Test]
        public void ReregisterDoesNotReplaceParentRegistration_Type()
        {
            var q1 = SomeQuery();
            var q2 = SomeQuery();

            var parent = new QueryScope();
            parent.Register(TypeName.UnknownName, q1);
            var sut = new QueryScope(parent);
            sut.Register(TypeName.UnknownName, q2);

            Assert.AreEqual(q1, parent.Find(TypeName.UnknownName));
            Assert.AreEqual(q2, sut.Find(TypeName.UnknownName));
        }

        [Test]
        public void ReregisterDoesNotReplaceParentRegistration_Id()
        {
            var q1 = SomeQuery();
            var q2 = SomeQuery();

            var parent = new QueryScope();
            parent.Register("x", q1);
            var sut = new QueryScope(parent);
            sut.Register("x", q2);

            Assert.AreEqual(q1, parent.Find("x"));
            Assert.AreEqual(q2, sut.Find("x"));
        }

        [Test]
        public void Equality_Default()
        {
            var a = new QueryScope();
            var b = new QueryScope();

            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var parent = new QueryScope();

            var a = new QueryScope(parent);
            a.Register(Type("T1"), new Query());
            a.Register("t2", new Query());

            var b = new QueryScope(parent);
            b.Register(Type("T1"), new Query());
            b.Register("t2", new Query());

            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentParent()
        {
            var a = new QueryScope(new QueryScope());
            var b = new QueryScope();

            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentRegistration_Type()
        {
            var a = new QueryScope();
            a.Register(TypeName.UnknownName, new Query());
            var b = new QueryScope();

            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentRegistration_Identifier()
        {
            var a = new QueryScope();
            a.Register("x", new Query());
            var b = new QueryScope();

            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test,
         ExpectedException(typeof (AssertException),
             ExpectedMessage = "type 'X, P' is already bound in current scope")]
        public void CannotReregisterInTheSameScope_Type()
        {
            var sut = new QueryScope();
            sut.Register(Type("X"), new Query());
            sut.Register(Type("X"), new Query());
        }

        [Test,
         ExpectedException(typeof (AssertException), ExpectedMessage = "id 'x' is already bound in current scope")]
        public void CannotReregisterInTheSameScope_Id()
        {
            var sut = new QueryScope();
            sut.Register("x", new Query());
            sut.Register("x", new Query());
        }

        private static Query SomeQuery()
        {
            return Mock.Of<Query>();
        }

        private static ITypeName Type(string name)
        {
            return TypeName.Get(name + ", P");
        }
    }
}