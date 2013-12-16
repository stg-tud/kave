using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.DataFlow;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.UI.Controls;
using KaVE.JetBrains.Annotations;

namespace KaVE.VsFeedbackGenerator.CodeCompletion
{
    internal static class LookupWindowManagerExtensions
    {
        private static readonly FieldInfo LookupWindowField =
            typeof (LookupWindowManagerImpl).GetField(
                "myCachedLookupWindow",
                BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly FieldInfo LookupListBox =
            typeof (LookupWindow).GetField("myListBox", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly FieldInfo LookupLifetime =
            typeof (Lookup).GetField("myLifetime", BindingFlags.NonPublic | BindingFlags.Instance);

        public static IEnumerable<ILookupItem> GetDisplayedLookupItems([NotNull] this ILookupWindowManager manager)
        {
            var lookupWindow = manager.GetLookupWindow();
            if (lookupWindow == null)
            {
                return new List<ILookupItem>();
            }
            var listBox = lookupWindow.GetLookupListBox();
            return listBox.Items.Cast<LookupListItem>().Select(lli => lli.LookupItem);
        }

        [CanBeNull]
        private static ILookupWindow GetLookupWindow([NotNull] this ILookupWindowManager manager)
        {
            return (ILookupWindow) LookupWindowField.GetValue(manager);
        }

        [NotNull]
        public static CustomListBoxControl<LookupListItem> GetLookupListBox([NotNull] this ILookupWindow lookupWindow)
        {
            return (CustomListBoxControl<LookupListItem>) LookupListBox.GetValue(lookupWindow);
        }

        [NotNull]
        public static Lifetime GetLifetime([NotNull] this ILookup lookup)
        {
            return (Lifetime) LookupLifetime.GetValue(lookup);
        }
    }
}