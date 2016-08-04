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
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Exceptions;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Naming.Impl.v0.CodeElements
{
    internal class ParameterNameTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new ParameterName();
            Assert.AreEqual("???", sut.Name);
            Assert.AreEqual(new TypeName(), sut.ValueType);
            Assert.True(sut.IsUnknown);
            Assert.False(sut.IsOutput);
            Assert.False(sut.IsPassedByReference);
            Assert.False(sut.IsExtensionMethodParameter);
            Assert.False(sut.IsOptional);
            Assert.False(sut.IsParameterArray);
        }

        [Test]
        public void ShouldRecognizeUnknownName()
        {
            Assert.True(new ParameterName().IsUnknown);
            Assert.True(new ParameterName("[?] ???").IsUnknown);
            Assert.False(new ParameterName("[T,P] p").IsUnknown);
        }

        [Test, ExpectedException(typeof(ValidationException))]
        public void ShouldAvoidNullParameters()
        {
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            new ParameterName(null);
        }

        [TestCaseSource(typeof(TestUtils), "TypeSource")]
        public void ShouldParseBasicInformation(string typeId)
        {
            var id = string.Format("[{0}] p", typeId);
            var sut = new ParameterName(id);

            Assert.AreEqual(typeId, sut.ValueType.Identifier);
            Assert.AreEqual("p", sut.Name);
            Assert.IsFalse(sut.IsOptional);
            Assert.IsFalse(sut.IsOutput);
            Assert.IsFalse(sut.IsParameterArray);
            Assert.IsFalse(sut.IsExtensionMethodParameter);
        }

        [Test]
        public void IsPassedByReferenceDepends()
        {
            Assert.False(new ParameterName("[?] p").IsPassedByReference);
            Assert.False(new ParameterName("[p:int] p").IsPassedByReference);
            Assert.True(new ParameterName("ref [p:int] p").IsPassedByReference);
            Assert.True(new ParameterName("ref [T,P] p").IsPassedByReference);
        }

        [Test]
        public void ShouldBeOutputParameter()
        {
            Assert.False(new ParameterName("[T,P] p").IsOutput);
            Assert.True(new ParameterName("out [T,P] p").IsOutput);
        }

        [Test]
        public void ShouldBeParameterArray()
        {
            Assert.False(new ParameterName("[T, P] p").IsParameterArray);
            Assert.False(new ParameterName("[T, P] p").IsParameterArray);
            Assert.False(new ParameterName("[T[], P] p").IsParameterArray);
            Assert.True(new ParameterName("params [T[], P] p").IsParameterArray);
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void ShouldRejectParamsWithoutArrayType()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new ParameterName("params [T, P] p");
        }

        [Test]
        public void ShouldHaveDefaultValue()
        {
            Assert.False(new ParameterName("[T,P] p").IsOptional);
            Assert.True(new ParameterName("opt [T,P] p").IsOptional);
        }

        [Test]
        public void ShouldBeExtensionMethodParameter()
        {
            Assert.False(new ParameterName("[T,P] p").IsExtensionMethodParameter);
            Assert.True(new ParameterName("this [T,P] p").IsExtensionMethodParameter);
        }
    }
}