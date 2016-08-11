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

using KaVE.Commons.Model.Naming.Impl.v0.Types.Organization;
using KaVE.Commons.Utils.Exceptions;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Naming.Impl.v0.Types.Organization
{
    public class NamespaceNameTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new NamespaceName();
            Assert.IsFalse(sut.IsGlobalNamespace);
            Assert.IsTrue(sut.IsUnknown);
            Assert.AreEqual("???", sut.Name);
            Assert.AreEqual(new NamespaceName(), sut.ParentNamespace);
        }

        [Test]
        public void ShouldRecognizeUnknownName()
        {
            Assert.True(new NamespaceName().IsUnknown);
            Assert.True(new NamespaceName("???").IsUnknown);
            Assert.False(new NamespaceName("a.b.c").IsUnknown);
        }

        [Test, ExpectedException(typeof(ValidationException))]
        public void ShouldAvoidNullParameters()
        {
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            new NamespaceName(null);
        }

        [Test]
        public void GlobalNamespace()
        {
            var sut = new NamespaceName("");
            Assert.IsEmpty(sut.Name);
            Assert.IsTrue(sut.IsGlobalNamespace);
            Assert.IsEmpty(sut.Identifier);
            Assert.IsNull(sut.ParentNamespace);
        }

        [TestCase("", ""), TestCase("a", "a"), TestCase("a.b", "b"), TestCase("a.b.c", "c")]
        public void ShoulParseName(string id, string name)
        {
            Assert.AreEqual(name, new NamespaceName(id).Name);
        }

        [TestCase("", null), TestCase("a", ""), TestCase("a.b", "a"), TestCase("a.b.c", "a.b")]
        public void ShoulParseParent(string id, string parentId)
        {
            var parentNamespace = new NamespaceName(id).ParentNamespace;
            if (parentId == null)
            {
                Assert.Null(parentNamespace);
            }
            else
            {
                Assert.AreEqual(new NamespaceName(parentId), parentNamespace);
            }
        }
    }
}