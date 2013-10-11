using System;
using System.Linq;
using System.Reflection;
using CodeCompletion.Utils.Assertion;
using EnvDTE;
using JetBrains.DataFlow;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Intentions.Bulbs;
using JetBrains.ReSharper.Intentions.Extensibility;
using JetBrains.ReSharper.Psi;
using JetBrains.TextControl;
using JetBrains.UI.BulbMenu;

namespace EventGenerator.ReSharper8.Generators
{
    [SolutionComponent]
    public class BulbItemInstrumentationComponent : IBulbItemsProvider
    {
        private readonly DTE _dte;

        public BulbItemInstrumentationComponent(DTE dte)
        {
            _dte = dte;
        }

        public int Priority
        {
            get
            {
                // to make sure we catch all bulb actions, we place this provider at the last possible
                // possition in the queue
                return int.MaxValue;
            }
        }

        public object PreExecute(ITextControl textControl)
        {
            return null;
        }

        public void CollectActions(IntentionsBulbItems intentionsBulbItems,
            BulbItems.BulbCache cacheData,
            ITextControl textControl,
            Lifetime caretPositionLifetime,
            IPsiSourceFile psiSourceFile,
            object precalculatedData)
        {
            var allBulbMenuItems = intentionsBulbItems.AllBulbMenuItems;
            foreach (var executableItem in allBulbMenuItems.Select(item => item.ExecutableItem))
            {
                var proxy = executableItem as IntentionAction.MyExecutableProxi;
                if (proxy != null)
                {
                    proxy.WrapBulbAction(_dte);
                    continue;
                }

                var executable = executableItem as ExecutableItem;
                if (executable != null)
                {
                    executable.WrapBulbAction(_dte);
                    continue;
                }

                Asserts.Fail("unexpected item type: {0}", executableItem.GetType().FullName);
            }
        }
    }

    internal static class BulbItemInstrumentationExtensions
    {
        private static readonly MethodInfo MyExecutableProxiBulbActionSetter =
            typeof (IntentionAction.MyExecutableProxi).GetProperty("BulbAction").GetSetMethod(true);

        private static readonly FieldInfo ExecutableItemActionField =
            typeof (ExecutableItem).GetField("myAction", BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// Wraps the <see cref="IBulbAction"/> contained in the passed proxy in a <see cref="EventGeneratingBulbActionProxy"/>.
        /// </summary>
        public static void WrapBulbAction(this IntentionAction.MyExecutableProxi proxy, DTE dte)
        {
            var originalBulbAction = proxy.BulbAction;
            var bulbActionWrapper = new EventGeneratingBulbActionProxy(originalBulbAction, dte);
            MyExecutableProxiBulbActionSetter.Invoke(proxy, new object[] {bulbActionWrapper});
        }

        public static void WrapBulbAction(this ExecutableItem executable, DTE dte)
        {
            var originalAction = (Action) ExecutableItemActionField.GetValue(executable);
            var wrapper = new EventGeneratingActionWrapper(originalAction, dte);
            var newAction = (Action) (wrapper.Execute);
            ExecutableItemActionField.SetValue(executable, newAction);
        }
    }
}