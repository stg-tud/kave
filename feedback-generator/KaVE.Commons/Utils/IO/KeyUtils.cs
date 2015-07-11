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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Input;
using KaVE.Commons.Utils.Assertion;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Utils.IO
{
    public static class KeyUtils
    {
        private static readonly IDictionary<string, string> KeyNameAliases = new Dictionary<string, string>
        {
            // alternative key names
            {"bkspce", "Back"},
            {"up arrow", "Up"},
            {"down arrow", "Down"},
            {"left arrow", "Left"},
            {"right arrow", "Right"},
            // German localizations
            {"strg", "Ctrl"},
            {"umschalt", "Shift"},
            {"eingabe", "Enter"},
            {"bild-auf", "PgUp"},
            {"bild-ab", "PgDn"},
            {"rücktaste", "Back"},
            {"ende", "End"},
            {"pos1", "Home"},
            {"einfügen", "Ins"},
            {"nach-rechts-taste", "Right"},
            {"nach-links-taste", "Left"},
            {"nach-oben-taste", "Up"},
            {"nach-unten-taste", "Down"},
            {"entf", "Del"},
        };

        /// <summary>
        /// <see cref="ParseBinding"/>
        /// </summary>
        [NotNull]
        public static IEnumerable<KeyBinding> ParseBindings([NotNull] IEnumerable<string> bindingsIdentifiers)
        {
            return bindingsIdentifiers.Select(ParseBinding);
        }

        /// <summary>
        /// Bindings have the format "<code>&lt;scope&gt;::&lt;modifiers&gt;+&lt;key&gt;</code>" or
        /// "<code>&lt;scope&gt;::&lt;modifiers&gt;+&lt;key&gt;, &lt;modifiers&gt;+&lt;key&gt;</code>"
        /// where <code>modifiers</code> is one or a combination of <code>Ctrl</code>, <code>Alt</code>,
        /// and <code>Shift</code>. Examples of  bindings are
        /// <list type="bullet">
        ///     <item><description><code>Global::Ctrl+O</code></description></item>
        ///     <item><description><code>Text Editor::Ctrl+R, Ctrl+R</code></description></item>
        ///     <item><description><code>Text Editor::Ctrl+Shift+R</code></description></item>
        /// </list>
        /// </summary>
        [NotNull]
        public static KeyBinding ParseBinding([NotNull] string bindingIdentifier)
        {
            var startOfKeyCombinations = bindingIdentifier.IndexOf(':') + 2;
            var keyCombisStrings = bindingIdentifier.Substring(startOfKeyCombinations).Split(',').Select(s => s.Trim());
            var keyCombis = keyCombisStrings.Select(s => s.Split('+').Select(ResolveKey).ToArray()).ToArray();
            Asserts.That(keyCombis.Any(), "binding contains no key combination: " + bindingIdentifier);
            Asserts.That(keyCombis.Count() <= 2, "binding has more than two key combinations: " + bindingIdentifier);
            return new KeyBinding(keyCombis);
        }

        public static Key ResolveKey([NotNull] string key)
        {
            var resolvedKey = Key.None;
            try
            {
                key = UnifyKeyName(key);
                resolvedKey = (Key)(new KeyConverter().ConvertFromString(key) ?? Key.None);
            }
            catch (ArgumentException)
            {
                // ignore failure and proceed with fallback stategy
            }

            if (resolvedKey == Key.None)
            {
                resolvedKey = KeyInterop.KeyFromVirtualKey(VkKeyScan(key[0]));
            }
            return resolvedKey;
        }

        private static string UnifyKeyName(string keyName)
        {
            string unifiedName;
            KeyNameAliases.TryGetValue(keyName.ToLower(), out unifiedName);
            return unifiedName ?? keyName;
        }

        [DllImport("user32.dll")]
        private static extern short VkKeyScan(char ch);

        public static bool IsPressed(this Key key)
        {
            return Invoke.OnSTA(() => Keyboard.IsKeyDown(key));
        }
    }
}
