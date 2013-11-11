using System.Linq;
using EnvDTE;
using JetBrains.DataFlow;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Intentions.Bulbs;
using JetBrains.ReSharper.Intentions.Extensibility;
using JetBrains.ReSharper.Psi;
using JetBrains.TextControl;
using JetBrains.UI.BulbMenu;
using KaVE.MessageBus.MessageBus;
using KaVE.Utils.Assertion;

namespace KaVE.EventGenerator.ReSharper8
{
    [SolutionComponent]
    internal class BulbItemInstrumentationComponent : IBulbItemsProvider
    {
        private readonly DTE _dte;
        private readonly SMessageBus _messageBus;

        public BulbItemInstrumentationComponent(DTE dte, SMessageBus messageBus)
        {
            _dte = dte;
            _messageBus = messageBus;
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
                    proxy.WrapBulbAction(_dte, _messageBus);
                    continue;
                }

                var executable = executableItem as ExecutableItem;
                if (executable != null)
                {
                    executable.WrapBulbAction(_dte, _messageBus);
                    continue;
                }

                Asserts.Fail("unexpected item type: {0}", executableItem.GetType().FullName);
            }
        }
    }
}