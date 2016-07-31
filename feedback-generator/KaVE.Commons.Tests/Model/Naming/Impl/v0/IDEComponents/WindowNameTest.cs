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
    internal class WindowNameTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new WindowName();
            Assert.AreEqual("???", sut.Type);
            Assert.AreEqual("???", sut.Caption);
            Assert.IsTrue(sut.IsUnknown);
        }

        [Test]
        public void ShouldImplementIsUnknown()
        {
            Assert.True(new WindowName().IsUnknown);
            Assert.True(new WindowName("???").IsUnknown);
            Assert.False(new WindowName("someType someCaption").IsUnknown);
        }

        [Test, ExpectedException(typeof(ValidationException))]
        public void ShouldAvoidNullParameters()
        {
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            new WindowName(null);
        }

        [Test]
        public void ShouldParseType()
        {
            Assert.AreEqual("someType", new WindowName("someType someCaption").Type);
        }

        [Test]
        public void ShouldParseCaption()
        {
            Assert.AreEqual("someCaption", new WindowName("someType someCaption").Caption);
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void ShouldRejectNameWithoutSpaces()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new WindowName("OnlyTypeOrCaption");
        }
    }
}