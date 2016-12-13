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
using KaVE.Commons.Model.Naming.Impl.v0.CodeElements;
using KaVE.Commons.Model.Naming.Impl.v0.Types;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Model.TypeShapes;
using KaVE.Commons.TestUtils;
using KaVE.Commons.Utils.Collections;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.TypeShapes
{
    internal class TypeShapeTest
    {
        private static TypeHierarchy SomeTypeHierarchy
        {
            get { return new TypeHierarchy {Element = Names.Type("T,P")}; }
        }

        [Test]
        public void DefaultValues()
        {
            var sut = new TypeShape();
            Assert.AreEqual(new TypeHierarchy(), sut.TypeHierarchy);
            Assert.AreEqual(Sets.NewHashSet<ITypeHierarchy>(), sut.NestedTypes);
            Assert.AreEqual(Sets.NewHashSet<IDelegateTypeName>(), sut.Delegates);
            Assert.AreEqual(Sets.NewHashSet<IFieldName>(), sut.Fields);
            Assert.AreEqual(Sets.NewHashSet<IMemberHierarchy<IEventName>>(), sut.EventHierarchies);
            Assert.AreEqual(Sets.NewHashSet<IMemberHierarchy<IMethodName>>(), sut.MethodHierarchies);
            Assert.AreEqual(Sets.NewHashSet<IMemberHierarchy<IPropertyName>>(), sut.PropertyHierarchies);
        }

        [Test]
        public void SettingValues()
        {
            var sut = new TypeShape
            {
                TypeHierarchy = SomeTypeHierarchy,
                NestedTypes = {Names.Type("T2,P")},
                Delegates = {new DelegateTypeName()},
                EventHierarchies = {new EventHierarchy()},
                Fields = {new FieldName()},
                MethodHierarchies = {new MethodHierarchy()},
                PropertyHierarchies = {new PropertyHierarchy()}
            };
            Assert.AreEqual(SomeTypeHierarchy, sut.TypeHierarchy);
            Assert.AreEqual(Sets.NewHashSet(new MethodHierarchy()), sut.MethodHierarchies);
            Assert.AreEqual(Sets.NewHashSet(Names.Type("T2,P")), sut.NestedTypes);
            Assert.AreEqual(Sets.NewHashSet<IDelegateTypeName>(new DelegateTypeName()), sut.Delegates);
            Assert.AreEqual(Sets.NewHashSet<IFieldName>(new FieldName()), sut.Fields);
            Assert.AreEqual(Sets.NewHashSet<IMemberHierarchy<IEventName>>(new EventHierarchy()), sut.EventHierarchies);
            Assert.AreEqual(
                Sets.NewHashSet<IMemberHierarchy<IPropertyName>>(new PropertyHierarchy()),
                sut.PropertyHierarchies);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new TypeHierarchy();
            var b = new TypeHierarchy();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new TypeShape
            {
                TypeHierarchy = SomeTypeHierarchy,
                NestedTypes = {Names.Type("T2,P")},
                Delegates = {new DelegateTypeName()},
                EventHierarchies = {new EventHierarchy()},
                Fields = {new FieldName()},
                MethodHierarchies = {new MethodHierarchy()},
                PropertyHierarchies = {new PropertyHierarchy()}
            };
            var b = new TypeShape
            {
                TypeHierarchy = SomeTypeHierarchy,
                NestedTypes = {Names.Type("T2,P")},
                Delegates = {new DelegateTypeName()},
                EventHierarchies = {new EventHierarchy()},
                Fields = {new FieldName()},
                MethodHierarchies = {new MethodHierarchy()},
                PropertyHierarchies = {new PropertyHierarchy()}
            };
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentType()
        {
            var a = new TypeShape
            {
                TypeHierarchy = SomeTypeHierarchy
            };
            var b = new TypeShape();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentNestedTypes()
        {
            var a = new TypeShape
            {
                NestedTypes = {Names.Type("T2,P")}
            };
            var b = new TypeShape();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentDelegates()
        {
            var a = new TypeShape
            {
                Delegates = {new DelegateTypeName()}
            };
            var b = new TypeShape();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentEvents()
        {
            var a = new TypeShape
            {
                EventHierarchies = {new EventHierarchy()}
            };
            var b = new TypeShape();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentFields()
        {
            var a = new TypeShape
            {
                Fields = {new FieldName()}
            };
            var b = new TypeShape();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentMethods()
        {
            var a = new TypeShape
            {
                MethodHierarchies = {new MethodHierarchy()}
            };
            var b = new TypeShape();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentProperties()
        {
            var a = new TypeShape
            {
                PropertyHierarchies = {new PropertyHierarchy()}
            };
            var b = new TypeShape();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(new TypeShape());
        }
    }
}