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
using KaVE.Commons.Utils.Exceptions;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Naming.Impl.v0.CodeElements
{
    internal class LocalVariableNameTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new LocalVariableName();
            Assert.AreEqual("???", sut.Name);
            Assert.AreEqual(new TypeName(), sut.ValueType);
            Assert.True(sut.IsUnknown);
        }

        [Test]
        public void ShouldRecognizeUnknownName()
        {
            Assert.True(new LocalVariableName().IsUnknown);
            Assert.True(new LocalVariableName("[?] ???").IsUnknown);
            Assert.False(new LocalVariableName("[T,P] o").IsUnknown);
        }

        [Test, ExpectedException(typeof(ValidationException))]
        public void ShouldAvoidNullParameters()
        {
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            new LocalVariableName(null);
        }

        [TestCaseSource(typeof(TestUtils), "TypeSource")]
        public void ShouldParseType(string typeId)
        {
            var id = string.Format("[{0}] id", typeId);
            var sut = new LocalVariableName(id);

            var actual = sut.ValueType;
            var expected = TypeUtils.CreateTypeName(typeId);
            Assert.AreEqual(expected, actual);
        }

        [TestCaseSource(typeof(TestUtils), "TypeSource")]
        public void ShouldParseVariableName(string typeId)
        {
            var id = string.Format("[{0}] id", typeId);
            var sut = new LocalVariableName(id);

            var actual = sut.Name;
            var expected = "id";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldNotStripNameIfNoWhitespaceIsUsed()
        {
            var sut = new LocalVariableName("[T]t");
            Assert.AreEqual(TypeUtils.CreateTypeName("T"), sut.ValueType);
            Assert.AreEqual("t", sut.Name);
        }
    }
}