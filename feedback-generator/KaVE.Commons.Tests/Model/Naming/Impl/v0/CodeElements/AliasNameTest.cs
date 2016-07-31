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
using KaVE.Commons.Utils.Exceptions;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Naming.Impl.v0.CodeElements
{
    internal class AliasNameTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new AliasName();
            Assert.True(sut.IsUnknown);
            Assert.False(sut.IsHashed);
        }

        [Test]
        public void ShouldRecognizeUnknownName()
        {
            Assert.True(new AliasName().IsUnknown);
            Assert.True(new AliasName("???").IsUnknown);
        }

        [Test, ExpectedException(typeof(ValidationException))]
        public void ShouldAvoidNullParameters()
        {
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            new AliasName(null);
        }

        [Test, Ignore]
        public void AddValidExampleOfAnAliasName()
        {
            Assert.Fail();
        }
    }
}