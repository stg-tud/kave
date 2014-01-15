using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Input;
using KaVE.JetBrains.Annotations;
using KaVE.Utils.Assertion;

namespace KaVE.Utils.IO
{
    public static class KeyUtils
    {
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
            switch (keyName)
            {
                case "Bkspce":
                    return "Back";
                case "Up Arrow":
                    return "Up";
                case "Down Arrow":
                    return "Down";
                case "Left Arrow":
                    return "Left";
                case "Right Arrow":
                    return "Right";
                default:
                    return keyName;
            }
        }

        [DllImport("user32.dll")]
        private static extern short VkKeyScan(char ch);

        public static bool IsPressed(this Key key)
        {
            return Invoke.OnSTA(() => Keyboard.IsKeyDown(key));
        }
    }
}
