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

using System;
using KaVE.VS.FeedbackGenerator.UserControls.ValueConverter;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.UserControls.ValueConverter
{
    internal class EnumLocalizationConverterTest
    {
        private EnumLocalizationConverter _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new EnumLocalizationConverter();
        }

        [Test]
        public void ExistingKeys()
        {
            var actual = _sut.Convert(TestEnum.ExistingKey, null, null, null);
            const string expected = "Localization";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void NonExistingKeys()
        {
            var actual = _sut.Convert(TestEnum.NonExistingKey, null, null, null);
            const string expected = "%TestEnum_NonExistingKey%";
            Assert.AreEqual(expected, actual);
        }

        [Test, ExpectedException(typeof (NotImplementedException))]
        public void ConvertBackIsNotImplemented()
        {
            _sut.ConvertBack(null, null, null, null);
        }

        private enum TestEnum
        {
            ExistingKey,
            NonExistingKey
        }
    }
}