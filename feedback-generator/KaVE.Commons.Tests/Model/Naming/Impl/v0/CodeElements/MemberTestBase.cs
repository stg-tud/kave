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
using KaVE.Commons.Utils;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Naming.Impl.v0.CodeElements
{
    internal abstract class MemberTestBase
    {
        protected abstract IMemberName GetMemberNameForBaseTests(string basicMemberSignature);

        [TestCaseSource(typeof(TestUtils), "TypeSource")]
        public void ShouldParseDeclaringType(string typeId)
        {
            var id = string.Format("[T,P] [{0}].M", typeId);
            var sut = GetMemberNameForBaseTests(id);

            Assert.AreEqual(typeId, sut.DeclaringType.Identifier);
        }

        [TestCaseSource(typeof(TestUtils), "TypeSource")]
        public void ShouldParseValueType(string typeId)
        {
            var id = string.Format("[{0}] [T,P].M", typeId);
            var sut = GetMemberNameForBaseTests(id);

            Assert.AreEqual(typeId, sut.ValueType.Identifier);
        }

        [TestCaseSource(typeof(TestUtils), "TypeSource")]
        public void ShouldParseMemberName(string typeId)
        {
            var id = string.Format("[{0}] [{0}].M", typeId);
            var sut = GetMemberNameForBaseTests(id);

            Assert.AreEqual("M", sut.Name);
        }

        [TestCaseSource(typeof(TestUtils), "TypeSource")]
        public void ShouldParseFullMemberName(string typeId)
        {
            var id = string.Format("[{0}] [{0}].M", typeId);
            var sut = GetMemberNameForBaseTests(id);

            Assert.AreEqual("M".FormatEx(typeId), sut.FullName);
        }

        [TestCaseSource(typeof(TestUtils), "TypeSource")]
        public void ShouldParseFullMemberNameGenericUnbound(string typeId)
        {
            var id = string.Format("[{0}] [{0}].M`1[[G1]]", typeId);
            var sut = GetMemberNameForBaseTests(id);

            Assert.AreEqual("M`1[[G1]]".FormatEx(typeId), sut.FullName);
        }

        [TestCaseSource(typeof(TestUtils), "TypeSource")]
        public void ShouldParseFullMemberNameGenericBound(string typeId)
        {
            var id = string.Format("[{0}] [{0}].M`1[[G1 -> {0}]]", typeId);
            var sut = GetMemberNameForBaseTests(id);

            Assert.AreEqual("M`1[[G1 -> {0}]]".FormatEx(typeId), sut.FullName);
        }

        [TestCaseSource(typeof(TestUtils), "TypeSource")]
        public void ShouldRecognizeStaticFieldName(string typeId)
        {
            var staticId = string.Format("static [{0}] [{0}].M", typeId);
            var sut = GetMemberNameForBaseTests(staticId);
            Assert.IsTrue(sut.IsStatic);

            var nonStaticId = string.Format("[{0}] [{0}].M", typeId);
            var sut2 = GetMemberNameForBaseTests(nonStaticId);
            Assert.IsFalse(sut2.IsStatic);
        }
    }
}