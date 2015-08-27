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

using System.Windows.Input;
using KaVE.Commons.Utils.IO;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.IO
{
    [TestFixture, Ignore("ignored locally")]
    class KeyUtilsTest
    {
        /// <summary>
        /// The concrete mapping of OEM keys is hardware dependend. We are,
        /// therefore, unable to specify the result. But we want to test that
        /// the symbols are resolved to some key symbol.
        /// </summary>
        [TestCase(".")]
        [TestCase(",")]
        [TestCase("`")]
        [TestCase("[")]
        [TestCase("]")]
        [TestCase("\\")]
        [TestCase("/")]
        [TestCase("-")]
        public void ShouldResolveOemSymbols(string symbol)
        {
            Assert.AreNotEqual(Key.None, KeyUtils.ResolveKey(symbol));
        }

        [TestCase("A", Key.A)]
        [TestCase("a", Key.A)]
        [TestCase("5", Key.D5)]
        [TestCase("Ctrl", Key.LeftCtrl)]
        [TestCase("Alt", Key.LeftAlt)]
        [TestCase("Shift", Key.LeftShift)]
        [TestCase("Enter", Key.Enter)]
        [TestCase("F12", Key.F12)]
        [TestCase("PgUp", Key.PageUp)] // Confuses ReSharper because Key.PageUp == Key.Prior
        [TestCase("PgDn", Key.PageDown)] // Confuses ReSharper because Key.PageDown == Key.Next
        [TestCase("Back", Key.Back)]
        [TestCase("End", Key.End)]
        [TestCase("Home", Key.Home)]
        [TestCase("Space", Key.Space)]
        [TestCase("Ins", Key.Insert)]
        [TestCase("Right", Key.Right)]
        [TestCase("Right Arrow", Key.Right)]
        [TestCase("Left", Key.Left)]
        [TestCase("Left Arrow", Key.Left)]
        [TestCase("Up", Key.Up)]
        [TestCase("Up Arrow", Key.Up)]
        [TestCase("Down", Key.Down)]
        [TestCase("Down Arrow", Key.Down)]
        [TestCase("Bkspce", Key.Back)]
        [TestCase("Esc", Key.Escape)]
        [TestCase("Escape", Key.Escape)]
        public void ShouldResolveCharacterKey(string keyString, Key key)
        {
            Assert.AreEqual(key, KeyUtils.ResolveKey(keyString));
        }

        [TestCase("Strg", Key.LeftCtrl)]
        [TestCase("Umschalt", Key.LeftShift)]
        [TestCase("Eingabe", Key.Enter)]
        [TestCase("Bild-Auf", Key.PageUp)] // Confuses ReSharper because Key.PageUp == Key.Prior
        [TestCase("Bild-Ab", Key.PageDown)] // Confuses ReSharper because Key.PageDown == Key.Next
        [TestCase("Rücktaste", Key.Back)]
        [TestCase("Ende", Key.End)]
        [TestCase("Pos1", Key.Home)]
        [TestCase("Einfügen", Key.Insert)]
        [TestCase("NACH-RECHTS-TASTE", Key.Right)]
        [TestCase("Nach-Links-Taste", Key.Left)]
        [TestCase("NACH-LINKS-TASTE", Key.Left)]
        [TestCase("NACH-OBEN-TASTE", Key.Up)]
        [TestCase("NACH-UNTEN-TASTE", Key.Down)]
        [TestCase("Entf", Key.Delete)]
        public void ShouldResolveGermanCharacterKey(string keyString, Key key)
        {
            Assert.AreEqual(key, KeyUtils.ResolveKey(keyString));
        }

        [TestCase("Global::Ctrl+A", new [] {Key.LeftCtrl, Key.A})]
        [TestCase("Text Editor::Ctrl+Shift+R", new [] {Key.LeftCtrl, Key.LeftShift, Key.R})]
        [TestCase("Global::Alt+F4", new [] {Key.LeftAlt, Key.F4})]
        public void ShouldResolveSingleCombinationBinding(string binding, Key[] shortcut)
        {
            var combinations = KeyUtils.ParseBinding(binding).Combinations;
            Assert.AreEqual(shortcut, combinations[0]);
        }

        [TestCase("Text Editor::Ctrl+R, Ctrl+R", new[] { Key.LeftCtrl, Key.R }, new[] { Key.LeftCtrl, Key.R })]
        [TestCase("Text Editor::Ctrl+E, D", new[] { Key.LeftCtrl, Key.E }, new[] { Key.D })]
        public void ShouldResolveScopedBinding(string binding, Key[] scopeKeys, Key[] shortcut)
        {
            var combinations = KeyUtils.ParseBinding(binding).Combinations;
            Assert.AreEqual(scopeKeys, combinations[0]);
            Assert.AreEqual(shortcut, combinations[1]);
        }
    }
}
