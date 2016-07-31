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
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Exceptions;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Naming.Impl.v0.IDEComponents
{
    internal class CommandBarControlNameTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new CommandBarControlName();
            Assert.AreEqual("???", sut.Name);
            Assert.AreEqual(null, sut.Parent);
            Assert.True(sut.IsUnknown);
        }

        [Test]
        public void ShouldImplementIsUnknown()
        {
            Assert.True(new CommandBarControlName().IsUnknown);
            Assert.True(new CommandBarControlName("???").IsUnknown);
            Assert.False(new CommandBarControlName("...").IsUnknown);
        }

        [Test, ExpectedException(typeof(ValidationException))]
        public void ShouldAvoidNullParameters()
        {
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            new CommandBarControlName(null);
        }

        [Test]
        public void ShouldParseSimpleExample()
        {
            var sut = new CommandBarControlName("a");
            Assert.AreEqual("a", sut.Name);
            Assert.Null(sut.Parent);
        }

        [Test]
        public void ShouldParseParent()
        {
            var sut = new CommandBarControlName("a|b|c");
            Assert.AreEqual("c", sut.Name);
            Assert.AreEqual(new CommandBarControlName("a|b"), sut.Parent);
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void ShouldNotAcceptDoubleSeparator()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new CommandBarControlName("a||b");
        }
    }
}