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
using KaVE.Commons.Model.Naming.Impl.v0.Types.Organization;
using KaVE.Commons.Model.Naming.Types;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Naming.Impl.v0.Types
{
    class DelegateTypeNameTest
    {
        private static readonly IDelegateTypeName ParameterlessDelegateName =
            new DelegateTypeName("d:[R, A, 1.0.0.0] [Some.DelegateType, A, 1.0.0.0].()");

        private static readonly IDelegateTypeName ParameterizedDelegateName =
            new DelegateTypeName(
                "d:[R, A, 1.0.0.0] [Some.DelegateType, A, 1.0.0.0].([C, A, 1.2.3.4] p1, [D, A, 1.2.3.4] p2)");

        private static readonly IDelegateTypeName[] DelegateTypeNames =
        {
            ParameterlessDelegateName,
            ParameterizedDelegateName
        };

        [TestCaseSource("DelegateTypeNames")]
        public void TypeClassification(IDelegateTypeName delegateType)
        {
            Assert.IsTrue(delegateType.IsDelegateType);
            Assert.IsTrue(delegateType.IsReferenceType);

            Assert.IsFalse(delegateType.IsArray);
            Assert.IsFalse(delegateType.IsClassType);
            Assert.IsFalse(delegateType.IsEnumType);
            Assert.IsFalse(delegateType.IsInterfaceType);
            Assert.IsFalse(delegateType.IsNullableType);
            Assert.IsFalse(delegateType.IsSimpleType);
            Assert.IsFalse(delegateType.IsStructType);
            Assert.IsFalse(delegateType.IsTypeParameter);
            Assert.IsFalse(delegateType.IsUnknown);
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
            Assert.AreEqual(new NamespaceName("Some"), delegateType.Namespace);
        }

        [Test]
        public void IsNested()
        {
            Assert.IsTrue(new DelegateTypeName("d:[R, P] [O+D, P].()").IsNestedType);
        }

        [Test]
        public void IsNotNested()
        {
            Assert.IsFalse(new DelegateTypeName("d:[R, P] [D, P].()").IsNestedType);
        }

        [Test]
        public void HasNoParameters()
        {
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
            CollectionAssert.IsEmpty(ParameterlessDelegateName.Parameters);
            CollectionAssert.AreEqual(
                new[] {new ParameterName("[C, A, 1.2.3.4] p1"), new ParameterName("[D, A, 1.2.3.4] p2")},
                ParameterizedDelegateName.Parameters);
        }

        [Test]
        public void OtherTypeNameIsNoDelegateType()
        {
            var uut = new TypeName("My.NonDelegate.Type, ND, 6.6.6.6");

            Assert.IsFalse(uut.IsDelegateType);
        }

        [Test]
        public void ParsesDelegateTypeOfMethodParameter()
        {
            var methodName = new MethodName("[R, A] [D, A].M([d:[DR, A] [DD, A].()] p)");
            var delegateParameter = methodName.Parameters[0];
            Assert.AreEqual(new ParameterName("[d:[DR, A] [DD, A].()] p"), delegateParameter);
        }

        [Test]
        public void ParsesDelegateTypeOfLambdaParameter()
        {
            var lambdaName = new LambdaName("[R, P] ([d:[DR, A] [DD, A].()] p)");
            var delegateParameter = lambdaName.Parameters[0];
            Assert.AreEqual(new ParameterName("[d:[DR, A] [DD, A].()] p"), delegateParameter);
        }

        [Test]
        public void ParsesDelegateTypeOfMemberValueType()
        {
            var delegateTypeId =
                "d:[System.Void, mscorlib, 4.0.0.0] [C+Delegate, TestProject].([System.Object, mscorlib, 4.0.0.0] obj)";
            var eventName = new EventName(string.Format("[{0}] [C, TestProject].Event", delegateTypeId));
            Assert.AreEqual(new DelegateTypeName(delegateTypeId), eventName.HandlerType);
        }

        [Test]
        public void IsDelegateTypeName()
        {
            Assert.True(
                TypeUtils.IsDelegateTypeIdentifier(
                    "d:[System.Void, mscorlib, 4.0.0.0] [System.AppDomainInitializer, mscorlib, 4.0.0.0].([System.String[], mscorlib, 4.0.0.0] args)"));
        }

        [Test]
        public void ParsesTypeParameters()
        {
            var typeName = new DelegateTypeName("d:[T] [DT`1[[T -> String, mscorlib]]].([T] p)");

            Assert.IsTrue(typeName.HasTypeParameters);
            CollectionAssert.AreEqual(new[] {new TypeParameterName("T -> String, mscorlib")}, typeName.TypeParameters);
        }

        [Test]
        public void TypeParameterParsingIsCached()
        {
            var sut = new DelegateTypeName("d:[?] [n.C+D`1[[T]]].()");
            var a = sut.TypeParameters;
            var b = sut.TypeParameters;
            Assert.AreSame(a, b);
        }
    }
}