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

using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Impl.v0.CodeElements;
using KaVE.Commons.Model.Naming.Impl.v0.Types;
using KaVE.Commons.Utils.Exceptions;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Naming.Impl.v0.CodeElements
{
    internal class PropertyNameTest : MemberTestBase
    {
        protected override IMemberName GetMemberNameForBaseTests(string basicMemberSignature)
        {
            // TODO NameUpdate: think again about this "()" at the end
            return new PropertyName(string.Format("get set {0}()", basicMemberSignature));
        }

        [Test]
        public void DefaultValues()
        {
            var sut = new PropertyName();
            Assert.AreEqual(new TypeName(), sut.DeclaringType);
            Assert.AreEqual(new TypeName(), sut.ValueType);
            Assert.False(sut.IsStatic);
            Assert.AreEqual("[?] [?].???", sut.Identifier);
            Assert.AreEqual("???", sut.Name);
            Assert.True(sut.IsUnknown);
            Assert.False(sut.IsHashed);
            Assert.False(sut.HasGetter);
            Assert.False(sut.HasSetter);
        }

        [Test]
        public void ShouldImplementIsUnknown()
        {
            Assert.True(new PropertyName().IsUnknown);
            Assert.True(new PropertyName("[?] [?].???").IsUnknown);
            Assert.False(new PropertyName("[T1,P] [T2,P].f").IsUnknown);
        }

        [Test, ExpectedException(typeof(ValidationException))]
        public void ShouldAvoidNullParameters()
        {
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            new PropertyName(null);
        }

        [Test]
        public void ShouldParseGetter()
        {
            Assert.IsFalse(new PropertyName("[T,P] [T,P].Prop").HasGetter);
            Assert.IsTrue(new PropertyName("get [T,P] [T,P].Prop").HasGetter);
        }

        [Test]
        public void ShouldParseSetter()
        {
            Assert.IsFalse(new PropertyName("[T,P] [T,P].Prop").HasSetter);
            Assert.IsTrue(new PropertyName("set [T,P] [T,P].Prop").HasSetter);
        }
    }
}