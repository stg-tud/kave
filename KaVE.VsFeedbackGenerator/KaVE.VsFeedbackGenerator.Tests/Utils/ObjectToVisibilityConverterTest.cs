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
using KaVE.VsFeedbackGenerator.Utils;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils
{
    [TestFixture]
    class ObjectToVisibilityConverterTest
    {
        [Test]
        public void ShouldConvertNullToHidden()
        {
            AssertConverts(null, Visibility.Collapsed);
        }
        [Test]
        public void ShouldConvertObjectToVisible()
        {
            AssertConverts(new object(), Visibility.Visible);
        }
        [Test]
        public void ShouldConvertNullableNullToHidden()
        {
            // ReSharper disable once RedundantCast
            AssertConverts((int?) null, Visibility.Collapsed);
        }
        [Test]
        public void ShouldConvertNullableValueToVisible()
        {
            AssertConverts((int?) 1, Visibility.Visible);
        }

        private static void AssertConverts(object value, Visibility exptectedResult)
        {
            var converter = new ObjectToVisibilityConverter();
            var result = converter.Convert(value, null, null, null);
            Assert.AreEqual(exptectedResult, result);
        }
    }
}
