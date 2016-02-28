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
using System.Windows;
using KaVE.VS.FeedbackGenerator.UserControls.ValueConverter;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.UserControls.ValueConverter
{
    internal class StringToVisibilityConverterTest
    {
        private StringToVisibilityConverter _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new StringToVisibilityConverter();
        }

        [Test]
        public void Null()
        {
            Assert.AreEqual(Visibility.Collapsed, Convert(null));
        }

        [Test]
        public void Empty()
        {
            Assert.AreEqual(Visibility.Collapsed, Convert(""));
        }

        [Test]
        public void SomeString()
        {
            Assert.AreEqual(Visibility.Visible, Convert("x"));
        }

        [Test]
        public void SomethingElse()
        {
            Assert.AreEqual(Visibility.Collapsed, Convert(1));
        }

        [Test, ExpectedException(typeof (NotImplementedException))]
        public void ConvertBackThrows()
        {
            _sut.ConvertBack(Visibility.Collapsed, null, null, null);
        }

        private object Convert(object o)
        {
            return _sut.Convert(o, null, null, null);
        }
    }
}