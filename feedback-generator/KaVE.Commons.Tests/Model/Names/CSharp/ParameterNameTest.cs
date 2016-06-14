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

using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Names.CSharp
{
    internal class ParameterNameTest
    {
        [Test]
        public void ShouldImplementIsUnknown()
        {
            Assert.That(ParameterName.UnknownName.IsUnknown);
        }

        [Test]
        public void ShouldBeSimpleParameter()
        {
            var parameterName = CsNameUtil.ParseParameterName("[ValueType, Assembly, 1.2.3.4] ParameterName");

            Assert.AreEqual("ValueType, Assembly, 1.2.3.4", parameterName.ValueType.Identifier);
            Assert.AreEqual("ParameterName", parameterName.Name);
            Assert.IsFalse(parameterName.IsOptional);
            Assert.IsFalse(parameterName.IsOutput);
            Assert.IsFalse(parameterName.IsParameterArray);
            Assert.IsFalse(parameterName.IsExtensionMethodParameter);
            Assert.IsTrue(parameterName.IsPassedByReference);
        }

        [Test]
        public void ShouldBeOutputParameter()
        {
            var parameterName = CsNameUtil.ParseParameterName("out [VT, A, 1.0.0.0] PName");

            Assert.AreEqual("VT, A, 1.0.0.0", parameterName.ValueType.Identifier);
            Assert.AreEqual("PName", parameterName.Name);
            Assert.IsTrue(parameterName.IsOutput);
        }

        [Test]
        public void ShouldBeValueParameter()
        {
            var parameterName = CsNameUtil.ParseParameterName("[System.Single, mscore, 4.0.0.0] i");

            Assert.IsFalse(parameterName.IsPassedByReference);
        }

        [Test]
        public void ShouldBeReferenceParameter()
        {
            var parameterName = CsNameUtil.ParseParameterName("ref [System.Single, mscore, 4.0.0.0] i");

            Assert.IsTrue(parameterName.IsPassedByReference);
        }

        [Test]
        public void ShouldBeParameterArray()
        {
            var parameterName = CsNameUtil.ParseParameterName("params [T, P, 1.3.2.4] name");

            Assert.IsTrue(parameterName.IsParameterArray);
        }

        [Test]
        public void ShouldNoBeParameterArray()
        {
            var parameterName = CsNameUtil.ParseParameterName("[arr(1):T, P, 1.3.2.4] name");

            Assert.IsTrue(parameterName.ValueType.IsArrayType);
            Assert.IsFalse(parameterName.IsParameterArray);
        }

        [Test]
        public void ShouldHaveDefaultValue()
        {
            var parameterName = CsNameUtil.ParseParameterName("opt [T, A, 4.3.2.1] p");

            Assert.IsTrue(parameterName.IsOptional);
        }

        [Test]
        public void ShouldBeExtensionMethodParameter()
        {
            var parameterName = CsNameUtil.ParseParameterName("this [T, A, 4.3.2.1] p");

            Assert.IsTrue(parameterName.IsExtensionMethodParameter);
        }

        [Test]
        public void ShouldBeOptionalReferenceParameter()
        {
            var parameterName = CsNameUtil.ParseParameterName("opt ref [System.Double, mscore, 4.0.0.0] param");

            Assert.IsTrue(parameterName.IsOptional);
            Assert.IsTrue(parameterName.IsPassedByReference);
            Assert.IsFalse(parameterName.IsOutput);
            Assert.IsFalse(parameterName.IsParameterArray);
        }

        [Test]
        public void ShouldBeUnknownParameter()
        {
            Assert.AreSame(TypeName.UnknownName, ParameterName.UnknownName.ValueType);
            Assert.AreEqual("???", ParameterName.UnknownName.Name);
        }
    }
}