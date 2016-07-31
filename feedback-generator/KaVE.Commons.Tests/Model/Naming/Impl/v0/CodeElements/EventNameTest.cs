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
    internal class EventNameTest : MemberTestBase
    {
        protected override IMemberName GetMemberNameForBaseTests(string basicMemberSignature)
        {
            return new EventName(basicMemberSignature);
        }

        [Test]
        public void DefaultValues()
        {
            var sut = new EventName();
            Assert.AreEqual(new TypeName(), sut.DeclaringType);
            Assert.AreEqual(new TypeName(), sut.ValueType);
            Assert.False(sut.IsStatic);
            Assert.AreEqual("[?] [?].???", sut.Identifier);
            Assert.AreEqual("???", sut.Name);
            Assert.True(sut.IsUnknown);
            Assert.False(sut.IsHashed);
        }

        [Test]
        public void ShouldImplementIsUnknown()
        {
            Assert.True(new EventName().IsUnknown);
            Assert.True(new EventName("[?] [?].???").IsUnknown);
            Assert.False(new EventName("[T1,P] [T2,P].e").IsUnknown);
        }

        [Test, ExpectedException(typeof(ValidationException))]
        public void ShouldAvoidNullParameters()
        {
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            new EventName(null);
        }
    }
}