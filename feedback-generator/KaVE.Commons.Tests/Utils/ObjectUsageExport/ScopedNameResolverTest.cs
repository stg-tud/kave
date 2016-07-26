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
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.ObjectUsageExport;
using Moq;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.ObjectUsageExport
{
    internal class ScopedNameResolverTest
    {
        private CoReTypeName TypeUnknown
        {
            get { return Names.UnknownType.ToCoReName(); }
        }

        [Test]
        public void DefaultValues()
        {
            var sut = new ScopedNameResolver();
            Assert.False(sut.IsExisting(TypeUnknown));
            Assert.NotNull(sut.BoundNames);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var parentMock = new ScopedNameResolver();

            var sut = new ScopedNameResolver(parentMock);
            Assert.AreEqual(parentMock, sut.ParentResolver);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void IsExisting_Type()
        {
            var parent = new ScopedNameResolver();
            var sut = new ScopedNameResolver(parent);

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
            var parent = new ScopedNameResolver();
            var sut = new ScopedNameResolver(parent);

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
        public void GetStaticType()
        {
            var sut = new ScopedNameResolver();
            sut.Register(
                "a",
                new Query
                {
                    type = Type("T")
                });
            var actual = sut.GetStaticType("a");
            var expected = Type("T");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetStaticType_DefinedInParentResolver()
        {
            var parent = new ScopedNameResolver();
            parent.Register(
                "a",
                new Query
                {
                    type = Type("T")
                });
            var sut = new ScopedNameResolver(parent);

            var actual = sut.GetStaticType("a");
            var expected = Type("T");
            Assert.AreEqual(expected, actual);
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void GetStaticType_Undefined()
        {
            var sut = new ScopedNameResolver();
            sut.GetStaticType("a");
        }

        [Test]
        public void RegisterType()
        {
            var q = SomeQuery();

            var sut = new ScopedNameResolver();
            sut.Register(TypeUnknown, q);

            Assert.True(sut.IsExisting(TypeUnknown));
            Assert.AreEqual(q, sut.Find(TypeUnknown));
        }

        [Test]
        public void RegisterId()
        {
            var q = SomeQuery();

            var sut = new ScopedNameResolver();
            sut.Register("x", q);

            Assert.True(sut.IsExisting("x"));
            Assert.AreEqual(q, sut.Find("x"));
        }

        [Test]
        public void LookupWorksInParentScope_Type()
        {
            var q = SomeQuery();

            var parent = new ScopedNameResolver();
            parent.Register(TypeUnknown, q);
            var sut = new ScopedNameResolver(parent);

            Assert.True(sut.IsExisting(TypeUnknown));
            Assert.AreEqual(q, sut.Find(TypeUnknown));
        }

        [Test]
        public void LookupWorksInParentScope_Id()
        {
            var q = SomeQuery();

            var parent = new ScopedNameResolver();
            parent.Register("x", q);
            var sut = new ScopedNameResolver(parent);

            Assert.True(sut.IsExisting("x"));
            Assert.AreEqual(q, sut.Find("x"));
        }

        [Test]
        public void ReregisterDoesNotReplaceParentRegistration_Type()
        {
            var q1 = SomeQuery();
            var q2 = SomeQuery();

            var parent = new ScopedNameResolver();
            parent.Register(TypeUnknown, q1);
            var sut = new ScopedNameResolver(parent);
            sut.Register(TypeUnknown, q2);

            Assert.AreEqual(q1, parent.Find(TypeUnknown));
            Assert.AreEqual(q2, sut.Find(TypeUnknown));
        }

        [Test]
        public void ReregisterDoesNotReplaceParentRegistration_Id()
        {
            var q1 = SomeQuery();
            var q2 = SomeQuery();

            var parent = new ScopedNameResolver();
            parent.Register("x", q1);
            var sut = new ScopedNameResolver(parent);
            sut.Register("x", q2);

            Assert.AreEqual(q1, parent.Find("x"));
            Assert.AreEqual(q2, sut.Find("x"));
        }

        [Test]
        public void Equality_Default()
        {
            var a = new ScopedNameResolver();
            var b = new ScopedNameResolver();

            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var parent = new ScopedNameResolver();

            var a = new ScopedNameResolver(parent);
            a.Register(Type("T1"), new Query());
            a.Register("t2", new Query());

            var b = new ScopedNameResolver(parent);
            b.Register(Type("T1"), new Query());
            b.Register("t2", new Query());

            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentParent()
        {
            var a = new ScopedNameResolver(new ScopedNameResolver());
            var b = new ScopedNameResolver();

            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentRegistration_Type()
        {
            var a = new ScopedNameResolver();
            a.Register(TypeUnknown, new Query());
            var b = new ScopedNameResolver();

            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentRegistration_Identifier()
        {
            var a = new ScopedNameResolver();
            a.Register("x", new Query());
            var b = new ScopedNameResolver();

            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test,
         ExpectedException(typeof(AssertException),
             ExpectedMessage = "type 'LX' is already bound in current scope")]
        public void CannotReregisterInTheSameScope_Type()
        {
            var sut = new ScopedNameResolver();
            sut.Register(Type("X"), new Query());
            sut.Register(Type("X"), new Query());
        }

        [Test,
         ExpectedException(typeof(AssertException), ExpectedMessage = "id 'x' is already bound in current scope")]
        public void CannotReregisterInTheSameScope_Id()
        {
            var sut = new ScopedNameResolver();
            sut.Register("x", new Query());
            sut.Register("x", new Query());
        }

        [Test]
        public void RegisteredVariablesAreAccessibleInBoundNames()
        {
            var sut = new ScopedNameResolver();
            sut.Register("x", new Query());

            var actual = Lists.NewListFrom(sut.BoundNames);
            var expected = Lists.NewList("x");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void RegisteredVariablesOfParentScopesAreAccessibleInBoundNames()
        {
            var parent = new ScopedNameResolver();
            parent.Register("x", new Query());

            var sut = new ScopedNameResolver(parent);
            sut.Register("y", new Query());

            var actual = Lists.NewListFrom(sut.BoundNames);
            var expected = Lists.NewList("x", "y");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void BoundNamesHaveSetSemantic()
        {
            var parent = new ScopedNameResolver();
            parent.Register("x", new Query());

            var sut = new ScopedNameResolver(parent);
            sut.Register("x", new Query());

            var actual = Lists.NewListFrom(sut.BoundNames);
            var expected = Lists.NewList("x");
            Assert.AreEqual(expected, actual);
        }

        private static Query SomeQuery()
        {
            return Mock.Of<Query>();
        }

        private static CoReTypeName Type(string name)
        {
            return Names.Type(name + ", P").ToCoReName();
        }
    }
}