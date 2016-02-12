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

using System.Windows;
using KaVE.VS.FeedbackGenerator.UserControls.ValueConverter;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.UserControls.ValueConverter
{
    internal class BooleanToVisibilityConverterTest
    {
        private BooleanToVisibilityConverter _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new BooleanToVisibilityConverter();
        }

        [Test]
        public void ConversionOfTrue()
        {
            var actual = _sut.Convert("True", null, null, null);
            var expected = Visibility.Visible;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ConversionOfFalse()
        {
            var actual = _sut.Convert("False", null, null, null);
            var expected = Visibility.Collapsed;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ConversionOfUnparseable()
        {
            var actual = _sut.Convert("this is unparseable", null, null, null);
            var expected = Visibility.Visible;
            Assert.AreEqual(expected, actual);
        }
    }
}