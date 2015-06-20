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

using NUnit.Framework;

namespace KaVE.ReSharper.Commons.Tests_Unit.Utils
{
    internal class InverseBoolConverterTest
    {
        [TestCase(true),
         TestCase(false)]
        public void ShouldInvert(bool value)
        {
            var converter = new InverseBoolConverter();
            var result = converter.Convert(value, null, null, null);
            Assert.AreEqual(!value, result);
        }

        [TestCase(true),
         TestCase(false)]
        public void ShouldInvertBack(bool value)
        {
            var converter = new InverseBoolConverter();
            var result = converter.ConvertBack(value, null, null, null);
            Assert.AreEqual(!value, result);
        }

        [Test, ExpectedException(typeof (AssertException))]
        public void ShouldFailToConvertNonBool()
        {
            var converter = new InverseBoolConverter();
            converter.Convert(new object(), null, null, null);
        }

        [Test, ExpectedException(typeof (AssertException))]
        public void ShouldFailToConvertBackNonBool()
        {
            var converter = new InverseBoolConverter();
            converter.ConvertBack(new object(), null, null, null);
        }
    }
}