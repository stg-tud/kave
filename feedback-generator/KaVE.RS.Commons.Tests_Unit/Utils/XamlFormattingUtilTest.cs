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

using KaVE.RS.Commons.Utils;
using NUnit.Framework;

namespace KaVE.RS.Commons.Tests_Unit.Utils
{
    internal class XamlFormattingUtilTest
    {
        private const string Content = "This is GOOD content";
        private const string Color = "Awesome";

        [Test]
        public void ShouldMakeTextBold()
        {
            const string expected = "<Bold>" + Content + "</Bold>";

            var actual = XamlFormattingUtil.Bold(Content);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldMakeTextBoldAndColored()
        {
            const string expected = "<Bold Foreground=\"" + Color + "\">" + Content + "</Bold>";

            var actual = XamlFormattingUtil.Bold(Content, Color);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldMakeTextColored()
        {
            const string expected = "<Span Foreground=\"" + Color + "\">" + Content + "</Span>";

            var actual = XamlFormattingUtil.Colored(Content, Color);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldMakeTextItalic()
        {
            const string expected = "<Italic>" + Content + "</Italic>";

            var actual = XamlFormattingUtil.Italic(Content);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldMakeTextItalicAndColored()
        {
            const string expected = "<Italic Foreground=\"" + Color + "\">" + Content + "</Italic>";

            var actual = XamlFormattingUtil.Italic(Content, Color);
            Assert.AreEqual(expected, actual);
        }
    }
}