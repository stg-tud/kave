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

using KaVE.Model.Names.CSharp;
using NUnit.Framework;

namespace KaVE.Model.Tests.Names.CSharp
{
    [TestFixture]
    class ParameterNameTest
    {
        [Test]
        public void ShouldBeSimpleParameter()
        {
            var parameterName = ParameterName.Get("[ValueType, Assembly, 1.2.3.4] ParameterName");

            Assert.AreEqual("ValueType, Assembly, 1.2.3.4", parameterName.ValueType.Identifier);
            Assert.AreEqual("ParameterName", parameterName.Name);
            Assert.IsFalse(parameterName.IsOptional);
            Assert.IsFalse(parameterName.IsOutput);
            Assert.IsFalse(parameterName.IsParameterArray);
            Assert.IsTrue(parameterName.IsPassedByReference);
        }

        [Test]
        public void ShouldBeOutputParameter()
        {
            var parameterName = ParameterName.Get("out [VT, A, 1.0.0.0] PName");

            Assert.AreEqual("VT, A, 1.0.0.0", parameterName.ValueType.Identifier);
            Assert.AreEqual("PName", parameterName.Name);
            Assert.IsTrue(parameterName.IsOutput);
        }

        [Test]
        public void ShouldBeValueParameter()
        {
            var parameterName = ParameterName.Get("[System.Single, mscore, 4.0.0.0] i");

            Assert.IsFalse(parameterName.IsPassedByReference);
        }

        [Test]
        public void ShouldBeReferenceParameter()
        {
            var parameterName = ParameterName.Get("ref [System.Single, mscore, 4.0.0.0] i");

            Assert.IsTrue(parameterName.IsPassedByReference);
        }

        [Test]
        public void ShouldBeParameterArray()
        {
            var parameterName = ParameterName.Get("params [T, P, 1.3.2.4] name");

            Assert.IsTrue(parameterName.IsParameterArray);
        }

        [Test]
        public void ShouldNoBeParameterArray()
        {
            var parameterName = ParameterName.Get("[T[], P, 1.3.2.4] name");

            Assert.IsTrue(parameterName.ValueType.IsArrayType);
            Assert.IsFalse(parameterName.IsParameterArray);
        }

        [Test]
        public void ShouldHaveDefaultValue()
        {
            var parameterName = ParameterName.Get("opt [T, A, 4.3.2.1] p");

            Assert.IsTrue(parameterName.IsOptional);
        }

        [Test]
        public void ShouldBeOptionalReferenceParameter()
        {
            var parameterName = ParameterName.Get("opt ref [System.Double, mscore, 4.0.0.0] param");

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
