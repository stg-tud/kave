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
        private static readonly ITypeName LegacyDelegateName = TypeName.Get("d:Some.DelegateType, A, 1.0.0.0");
        private static readonly ITypeName ParameterlessDelegateName = TypeName.Get("d:Some.DelegateType(), A, 1.0.0.0");

        private static readonly ITypeName[] DelegateTypeNames =
        {
            LegacyDelegateName,
            ParameterlessDelegateName
        };

        [TestCaseSource("DelegateTypeNames")]
        public void IsDelegateType(ITypeName delegateType)
        {
            Assert.IsTrue(delegateType.IsDelegateType);
        }

        [TestCaseSource("DelegateTypeNames")]
        public void IsNoClassType(ITypeName delegateType)
        {
            Assert.IsFalse(delegateType.IsClassType);
        }

        [TestCaseSource("DelegateTypeNames")]
        public void IsReferenceType(ITypeName delegateType)
        {
            Assert.IsTrue(delegateType.IsReferenceType);
        }

        [TestCaseSource("DelegateTypeNames")]
        public void ParsesName(ITypeName delegateType)
        {
            Assert.AreEqual("DelegateType", delegateType.Name);
        }

        [Test]
        public void OtherTypeNameIsNoDelegateType()
        {
            var uut = TypeName.Get("My.NonDelegate.Type, ND, 6.6.6.6");

            Assert.IsFalse(uut.IsDelegateType);
        }
    }
}
