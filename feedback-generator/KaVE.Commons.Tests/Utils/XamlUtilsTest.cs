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

using System.Windows.Markup;
using KaVE.Commons.Utils;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils
{
    class XamlUtilsTest
    {
        [Test]
        [ExpectedException(typeof(XamlParseException))]
        public void ShouldThrowOnInvalidXaml()
        {
            const string invalidXaml = "<Span>Some text</span>";
            XamlUtils.CreateDataTemplateFromXaml(invalidXaml);
        }

        [Test]
        public void ShouldStripTagsCorrectly()
        {
            const string xaml = "<Span Foreground=\"#123456\">Some <Bold>text</Bold></Span>";
            var actual = xaml.StripTags();
            Assert.AreEqual("Some text", actual);
        }

        [Test]
        public void ShouldEncodeSpecialCharsCorrectly()
        {
            // Quotes are not encoded, because they cause no problems in Xaml.
            const string xaml = "\"<>&";
            Assert.AreEqual("\"&lt;&gt;&amp;", xaml.EncodeSpecialChars());
        }
    }
}
