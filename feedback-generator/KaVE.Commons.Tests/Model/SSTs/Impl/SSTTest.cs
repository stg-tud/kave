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
using KaVE.Commons.Model.SSTs.Declarations;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.TestUtils;
using KaVE.Commons.Utils.Collections;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.SSTs.Impl
{
    internal class SSTTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new SST();

            Assert.AreEqual(Names.UnknownType, sut.EnclosingType);
            Assert.Null(sut.PartialClassIdentifier);
            Assert.False(sut.IsPartialClass);
            Assert.AreEqual(Lists.NewList<IDelegateDeclaration>(), sut.Delegates);
            Assert.AreEqual(Lists.NewList<IEventDeclaration>(), sut.Events);
            Assert.AreEqual(Lists.NewList<IFieldDeclaration>(), sut.Fields);
            Assert.AreEqual(Lists.NewList<IMethodDeclaration>(), sut.Methods);
            Assert.AreEqual(Lists.NewList<IPropertyDeclaration>(), sut.Properties);
            Assert.AreEqual(Lists.NewList<IMethodDeclaration>(), sut.EntryPoints);
            Assert.AreEqual(Lists.NewList<IMethodDeclaration>(), sut.NonEntryPoints);

            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void PartialClassIdentifierCannotBeSetToEmpty()
        {
            var sut = new SST();

            Assert.Null(sut.PartialClassIdentifier);
            sut.PartialClassIdentifier = "";
            Assert.Null(sut.PartialClassIdentifier);
            sut.PartialClassIdentifier = "a";
            Assert.NotNull(sut.PartialClassIdentifier);
        }

        [Test]
        public void DifferentStylesOfPartialClassIdentifierAreEqual()
        {
            var a = new SST {PartialClassIdentifier = null};
            var b = new SST {PartialClassIdentifier = ""};
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }


        [Test]
        public void SettingValues()
        {
            var sut = new SST
            {
                EnclosingType = Names.Type("T1, P1"),
                PartialClassIdentifier = "abc",
                Delegates = {new DelegateDeclaration()},
                Events = {new EventDeclaration()},
                Fields = {new FieldDeclaration()},
                Methods = {new MethodDeclaration()},
                Properties = {new PropertyDeclaration()}
            };

            Assert.AreEqual(Names.Type("T1, P1"), sut.EnclosingType);
            Assert.AreEqual("abc", sut.PartialClassIdentifier);
            Assert.True(sut.IsPartialClass);
            Assert.AreEqual(Lists.NewList(new DelegateDeclaration()), sut.Delegates);
            Assert.AreEqual(Lists.NewList(new EventDeclaration()), sut.Events);
            Assert.AreEqual(Lists.NewList(new FieldDeclaration()), sut.Fields);
            Assert.AreEqual(Lists.NewList(new MethodDeclaration()), sut.Methods);
            Assert.AreEqual(Lists.NewList(new PropertyDeclaration()), sut.Properties);
        }

        [Test]
        public void IsPartialCanHandleBothEmptyStringsAndNulls()
        {
            Assert.IsFalse(new SST {PartialClassIdentifier = null}.IsPartialClass);
            Assert.IsFalse(new SST {PartialClassIdentifier = ""}.IsPartialClass);
            Assert.IsTrue(new SST {PartialClassIdentifier = "p"}.IsPartialClass);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new SST();
            var b = new SST();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new SST
            {
                EnclosingType = Names.Type("T1, P1"),
                PartialClassIdentifier = "abc",
                Delegates = {new DelegateDeclaration()},
                Events = {new EventDeclaration()},
                Fields = {new FieldDeclaration()},
                Methods = {new MethodDeclaration()},
                Properties = {new PropertyDeclaration()}
            };
            var b = new SST
            {
                EnclosingType = Names.Type("T1, P1"),
                PartialClassIdentifier = "abc",
                Delegates = {new DelegateDeclaration()},
                Events = {new EventDeclaration()},
                Fields = {new FieldDeclaration()},
                Methods = {new MethodDeclaration()},
                Properties = {new PropertyDeclaration()}
            };

            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentType()
        {
            var a = new SST {EnclosingType = Names.Type("T1, P1")};
            var b = new SST();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentPartialClassId()
        {
            var a = new SST {PartialClassIdentifier = "abc"};
            var b = new SST();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentDelegates()
        {
            var a = new SST {Delegates = {new DelegateDeclaration()}};
            var b = new SST();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentEvents()
        {
            var a = new SST {Events = {new EventDeclaration()}};
            var b = new SST();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentFields()
        {
            var a = new SST {Fields = {new FieldDeclaration()}};
            var b = new SST();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentMethods()
        {
            var a = new SST {Methods = {new MethodDeclaration()}};
            var b = new SST();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentProperties()
        {
            var a = new SST {Properties = {new PropertyDeclaration()}};
            var b = new SST();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void VisitorIsImplemented()
        {
            var sut = new SST();
            sut.Accept(23).Verify(v => v.Visit(sut, 23));
        }

        [Test]
        public void VisitorWithReturnIsImplemented()
        {
            var sut = new SST();
            sut.Accept(23).VerifyWithReturn(v => v.Visit(sut, 23));
        }

        [Test]
        public void EntryPointFiltering()
        {
            var ep = new MethodDeclaration {IsEntryPoint = true};
            var nep = new MethodDeclaration {IsEntryPoint = false};

            var sut = new SST();
            sut.Methods.Add(ep);
            sut.Methods.Add(nep);

            Assert.AreEqual(Sets.NewHashSet(ep), sut.EntryPoints);
            var newHashSet = Sets.NewHashSet(nep);
            var nonEntryPoints = sut.NonEntryPoints;
            Assert.AreEqual(newHashSet, nonEntryPoints);
        }

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(new SST());
        }
    }
}