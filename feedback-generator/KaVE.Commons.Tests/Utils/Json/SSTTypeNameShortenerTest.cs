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

using KaVE.Commons.Utils.Json;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.Json
{
    internal class SSTTypeNameShortenerTest
    {
        private const string Long =
            "...$type\": \"KaVE.Commons.Model.SSTs.Impl.Declarations.PropertyDeclaration, KaVE.Commons\",...";

        private const string Short = "...$type\": \"[SST:Declarations.PropertyDeclaration]\",...";
        private const string NoMatch = "abc";

        private SSTTypeNameShortener _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new SSTTypeNameShortener();
        }

        [Test]
        public void RemoveDetails()
        {
            var actual = _sut.RemoveDetails(Long);
            const string expected = Short;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void RemoveDetails_NoMatch()
        {
            var actual = _sut.RemoveDetails(NoMatch);
            const string expected = NoMatch;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void RemoveDetails_Twice()
        {
            var actual = _sut.RemoveDetails(Long + Long);
            const string expected = Short + Short;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AddDetails()
        {
            var actual = _sut.AddDetails(Short);
            const string expected = Long;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AddDetails_NoMatch()
        {
            var actual = _sut.AddDetails(NoMatch);
            const string expected = NoMatch;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AddDetails_Twice()
        {
            var actual = _sut.AddDetails(Short + Short);
            const string expected = Long + Long;
            Assert.AreEqual(expected, actual);
        }
    }
}