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
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.Exceptions;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Naming.Impl.v0.CodeElements
{
    internal class PropertyNameTest : MemberTestBase
    {
        protected override IMemberName GetMemberNameForBaseTests(string basicMemberSignature)
        {
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

            Assert.False(sut.IsIndexer);
            Assert.False(sut.HasParameters);
            Assert.AreEqual(Lists.NewList<IParameterName>(), sut.Parameters);
        }

        [Test]
        public void ShouldImplementIsUnknown()
        {
            Assert.True(new PropertyName().IsUnknown);
            Assert.True(new PropertyName("[?] [?].???").IsUnknown);
            Assert.False(new PropertyName("get [T1,P] [T2,P].f()").IsUnknown);
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
            Assert.IsFalse(new PropertyName("set [T,P] [T,P].Prop()").HasGetter);
            Assert.IsTrue(new PropertyName("get [T,P] [T,P].Prop()").HasGetter);
        }

        [Test]
        public void ShouldParseSetter()
        {
            Assert.IsFalse(new PropertyName("get [T,P] [T,P].Prop()").HasSetter);
            Assert.IsTrue(new PropertyName("set [T,P] [T,P].Prop()").HasSetter);
        }

        [ExpectedException(typeof(ValidationException)), //
         TestCase("get set [?] [?].P"), // no paranthesis
         TestCase("[?] [?].P()") // neither get nor set
        ]
        public void ShouldRejectInvalidProperties(string invalidId)
        {
            // ReSharper disable once ObjectCreationAsStatement
            new PropertyName(invalidId);
        }

        [Test]
        public void ShouldParseParameters1()
        {
            var sut = new PropertyName("get [?] [?].P([?] p)");
            Assert.IsTrue(sut.IsIndexer);
            Assert.IsTrue(sut.HasParameters);
            Assert.AreEqual(Lists.NewList(new ParameterName("[?] p")), sut.Parameters);
        }

        [Test]
        public void ShouldParseParameters2()
        {
            var sut = new PropertyName("get [?] [?].P([?] p1, [?] p2)");
            Assert.IsTrue(sut.IsIndexer);
            Assert.IsTrue(sut.HasParameters);
            Assert.AreEqual(Lists.NewList(new ParameterName("[?] p1"), new ParameterName("[?] p2")), sut.Parameters);
        }
    }
}