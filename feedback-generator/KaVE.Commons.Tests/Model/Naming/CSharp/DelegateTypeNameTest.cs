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

using KaVE.Commons.Model.Naming.Impl.v0.CodeElements;
using KaVE.Commons.Model.Naming.Impl.v0.Types;
using KaVE.Commons.Model.Naming.Types;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Naming.CSharp
{
    class DelegateTypeNameTest
    {
        private static readonly IDelegateTypeName LegacyDelegateName =
            DelegateTypeName.Get("d:Some.DelegateType, A, 1.0.0.0");

        private static readonly IDelegateTypeName ParameterlessDelegateName =
            DelegateTypeName.Get("d:[R, A, 1.0.0.0] [Some.DelegateType, A, 1.0.0.0].()");

        private static readonly IDelegateTypeName ParameterizedDelegateName =
            DelegateTypeName.Get(
                "d:[R, A, 1.0.0.0] [Some.DelegateType, A, 1.0.0.0].([C, A, 1.2.3.4] p1, [D, A, 1.2.3.4] p2)");

        private static readonly IDelegateTypeName[] DelegateTypeNames =
        {
            LegacyDelegateName,
            ParameterlessDelegateName,
            ParameterizedDelegateName
        };

        [TestCaseSource("DelegateTypeNames")]
        public void TypeClassification(IDelegateTypeName delegateType)
        {
            Assert.IsTrue(delegateType.IsDelegateType);
            Assert.IsTrue(delegateType.IsReferenceType);

            Assert.IsFalse(delegateType.IsArrayType);
            Assert.IsFalse(delegateType.IsClassType);
            Assert.IsFalse(delegateType.IsEnumType);
            Assert.IsFalse(delegateType.IsInterfaceType);
            Assert.IsFalse(delegateType.IsNullableType);
            Assert.IsFalse(delegateType.IsSimpleType);
            Assert.IsFalse(delegateType.IsStructType);
            Assert.IsFalse(delegateType.IsTypeParameter);
            Assert.IsFalse(delegateType.IsUnknownType);
            Assert.IsFalse(delegateType.IsValueType);
            Assert.IsFalse(delegateType.IsVoidType);
        }

        [TestCaseSource("DelegateTypeNames")]
        public void ParsesFullName(IDelegateTypeName delegateType)
        {
            Assert.AreEqual("Some.DelegateType", delegateType.FullName);
        }

        [TestCaseSource("DelegateTypeNames")]
        public void ParsesName(IDelegateTypeName delegateType)
        {
            Assert.AreEqual("DelegateType", delegateType.Name);
        }

        [TestCaseSource("DelegateTypeNames")]
        public void ParsesNamespace(IDelegateTypeName delegateType)
        {
            Assert.AreEqual(NamespaceName.Get("Some"), delegateType.Namespace);
        }

        [Test]
        public void IsNested()
        {
            Assert.IsTrue(DelegateTypeName.Get("d:[R, P] [O+D, P].()").IsNestedType);
        }

        [Test]
        public void IsNotNested()
        {
            Assert.IsFalse(DelegateTypeName.Get("d:[R, P] [D, P].()").IsNestedType);
        }

        [Test]
        public void ParsesSignature()
        {
            Assert.AreEqual("DelegateType()", LegacyDelegateName.Signature);
            Assert.AreEqual("DelegateType()", ParameterlessDelegateName.Signature);
            Assert.AreEqual("DelegateType([C, A, 1.2.3.4] p1, [D, A, 1.2.3.4] p2)", ParameterizedDelegateName.Signature);
        }

        [Test]
        public void HasNoParameters()
        {
            Assert.IsFalse(LegacyDelegateName.HasParameters);
            Assert.IsFalse(ParameterlessDelegateName.HasParameters);
        }

        [Test]
        public void HasParameters()
        {
            Assert.IsTrue(ParameterizedDelegateName.HasParameters);
        }

        [Test]
        public void ParsesParameters()
        {
            CollectionAssert.IsEmpty(LegacyDelegateName.Parameters);
            CollectionAssert.IsEmpty(ParameterlessDelegateName.Parameters);
            CollectionAssert.AreEqual(
                new[] {ParameterName.Get("[C, A, 1.2.3.4] p1"), ParameterName.Get("[D, A, 1.2.3.4] p2")},
                ParameterizedDelegateName.Parameters);
        }

        [Test]
        public void OtherTypeNameIsNoDelegateType()
        {
            var uut = TypeName.Get("My.NonDelegate.Type, ND, 6.6.6.6");

            Assert.IsFalse(uut.IsDelegateType);
        }

        [Test]
        public void ParsesDelegateTypeOfMethodParameter()
        {
            var methodName = MethodName.Get("[R, A] [D, A].M([d:[DR, A] [DD, A].()] p)");
            var delegateParameter = methodName.Parameters[0];
            Assert.AreEqual(ParameterName.Get("[d:[DR, A] [DD, A].()] p"), delegateParameter);
        }

        [Test]
        public void ParsesDelegateTypeOfLambdaParameter()
        {
            var lambdaName = LambdaName.Get("[R, P] ([d:[DR, A] [DD, A].()] p)");
            var delegateParameter = lambdaName.Parameters[0];
            Assert.AreEqual(ParameterName.Get("[d:[DR, A] [DD, A].()] p"), delegateParameter);
        }

        [Test]
        public void ParsesDelegateTypeOfMemberValueType()
        {
            var eventName =
                EventName.Get(
                    "[d:[System.Void, mscorlib, 4.0.0.0] [C+Delegate, TestProject].([System.Object, mscorlib, 4.0.0.0] obj)] [C, TestProject].Event");
            Assert.AreSame(
                TypeName.Get(
                    "d:[System.Void, mscorlib, 4.0.0.0] [C+Delegate, TestProject].([System.Object, mscorlib, 4.0.0.0] obj)"),
                eventName.HandlerType);
        }

        [Test]
        public void IsDelegateTypeName()
        {
            var typeName =
                TypeName.Get(
                    "d:[System.Void, mscorlib, 4.0.0.0] [System.AppDomainInitializer, mscorlib, 4.0.0.0].([System.String[], mscorlib, 4.0.0.0] args)");
            Assert.IsInstanceOf<DelegateTypeName>(typeName);
        }

        [Test]
        public void ParsesTypeParameters()
        {
            var typeName = TypeName.Get("d:[T] [DT`1[[T -> String, mscorlib]]].([T] p)");

            Assert.IsTrue(typeName.HasTypeParameters);
            CollectionAssert.AreEqual(new[] {TypeParameterName.Get("T -> String, mscorlib")}, typeName.TypeParameters);
        }

        [Test]
        public void FixesLegacyDelegateTypeNameFormat()
        {
            Assert.AreEqual("d:[?] [Some.DelegateType, A, 1.0.0.0].()", LegacyDelegateName.Identifier);
        }
    }
}