using System;
using System.Reflection;
using EnvDTE;
using EventGenerator.ReSharper8.Generators;
using JetBrains.ReSharper.Intentions.Extensibility;
using JetBrains.UI.BulbMenu;
using KAVE.KAVE_MessageBus.MessageBus;

namespace EventGenerator.ReSharper8
{
    internal static class BulbItemInstrumentationExtensions
    {
        private static readonly MethodInfo MyExecutableProxiBulbActionSetter =
            typeof (IntentionAction.MyExecutableProxi).GetProperty("BulbAction").GetSetMethod(true);

        private static readonly FieldInfo ExecutableItemActionField =
            typeof (ExecutableItem).GetField("myAction", BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// Wraps the <see cref="IBulbAction"/> contained in the passed proxy in a <see cref="EventGeneratingBulbActionProxy"/>.
        /// </summary>
        public static void WrapBulbAction(this IntentionAction.MyExecutableProxi proxy, DTE dte, SMessageBus messageBus)
        {
            var originalBulbAction = proxy.BulbAction;
            var bulbActionWrapper = new EventGeneratingBulbActionProxy(originalBulbAction, dte, messageBus);
            MyExecutableProxiBulbActionSetter.Invoke(proxy, new object[] {bulbActionWrapper});
        }

        public static void WrapBulbAction(this ExecutableItem executable, DTE dte, SMessageBus messageBus)
        {
            var originalAction = (Action) ExecutableItemActionField.GetValue(executable);
            var wrapper = new EventGeneratingActionWrapper(originalAction, dte, messageBus);
            var newAction = (Action) (wrapper.Execute);
            ExecutableItemActionField.SetValue(executable, newAction);
        }
    }
}