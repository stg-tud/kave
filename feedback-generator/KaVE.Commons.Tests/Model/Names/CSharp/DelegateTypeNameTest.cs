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
 *    - Sven Amann
 */

using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Names.CSharp
{
    [TestFixture]
    class DelegateTypeNameTest
    {
        private static readonly IDelegateTypeName LegacyDelegateName = DelegateTypeName.Get("d:Some.DelegateType, A, 1.0.0.0");
        private static readonly IDelegateTypeName ParameterlessDelegateName = DelegateTypeName.Get("d:Some.DelegateType(), A, 1.0.0.0");
        private static readonly IDelegateTypeName ParameterizedDelegateName = DelegateTypeName.Get("d:Some.DelegateType([C, A, 1.2.3.4] p1, [D, A, 1.2.3.4] p2), A, 1.0.0.0");

        private static readonly IDelegateTypeName[] DelegateTypeNames =
        {
            LegacyDelegateName,
            ParameterlessDelegateName,
            ParameterizedDelegateName
        };

        [TestCaseSource("DelegateTypeNames")]
        public void IsDelegateType(IDelegateTypeName delegateType)
        {
            Assert.IsTrue(delegateType.IsDelegateType);
        }

        [TestCaseSource("DelegateTypeNames")]
        public void IsNoClassType(IDelegateTypeName delegateType)
        {
            Assert.IsFalse(delegateType.IsClassType);
        }

        [TestCaseSource("DelegateTypeNames")]
        public void IsReferenceType(IDelegateTypeName delegateType)
        {
            Assert.IsTrue(delegateType.IsReferenceType);
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
            CollectionAssert.AreEqual(new[] { ParameterName.Get("[C, A, 1.2.3.4] p1"), ParameterName.Get("[D, A, 1.2.3.4] p2") }, ParameterizedDelegateName.Parameters);
        }

        [Test]
        public void OtherTypeNameIsNoDelegateType()
        {
            var uut = TypeName.Get("My.NonDelegate.Type, ND, 6.6.6.6");

            Assert.IsFalse(uut.IsDelegateType);
        }
    }
}
