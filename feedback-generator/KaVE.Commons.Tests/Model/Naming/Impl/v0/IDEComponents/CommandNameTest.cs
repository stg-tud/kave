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

using KaVE.Commons.Model.Naming.Impl.v0.IDEComponents;
using KaVE.Commons.Utils.Exceptions;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Naming.Impl.v0.IDEComponents
{
    internal class CommandNameTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new CommandName();
            Assert.AreEqual("???", sut.Guid);
            Assert.AreEqual(-1, sut.Id);
            Assert.AreEqual("???", sut.Name);
            Assert.True(sut.IsUnknown);
        }

        [Test]
        public void ShouldImplementIsUnknown()
        {
            Assert.True(new CommandName().IsUnknown);
            Assert.True(new CommandName("???").IsUnknown);
            Assert.False(new CommandName("a:1:abc").IsUnknown);
        }

        [Test, ExpectedException(typeof(ValidationException))]
        public void ShouldAvoidNullParameters()
        {
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            new CommandName(null);
        }

        [Test]
        public void ShouldParseCommandsWithId()
        {
            var sut = new CommandName("a:1:abc");
            Assert.AreEqual("a", sut.Guid);
            Assert.AreEqual(1, sut.Id);
            Assert.AreEqual("abc", sut.Name);
        }

        [Test]
        public void ShouldIncludeAdditionalColonsInName()
        {
            var sut = new CommandName("a:1:funny :)");
            Assert.AreEqual("a", sut.Guid);
            Assert.AreEqual(1, sut.Id);
            Assert.AreEqual("funny :)", sut.Name);
        }

        [Test]
        public void ShouldParseSimpleButtonClicks()
        {
            var sut = new CommandName("funny :)");
            Assert.AreEqual("???", sut.Guid);
            Assert.AreEqual(-1, sut.Id);
            Assert.AreEqual("funny :)", sut.Name);
        }
    }
}