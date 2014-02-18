using JetBrains.Application;
using JetBrains.Application.DataContext;
using JetBrains.Application.Settings;

namespace KaVE.VsFeedbackGenerator.Utils
{
    [ShellComponent]
    public class SettingsStore
    {
        private readonly ISettingsStore _settingsStore;
        private readonly DataContexts _dataContexts;

        public SettingsStore(ISettingsStore settingsStore, DataContexts dataContexts)
        {
            _settingsStore = settingsStore;
            _dataContexts = dataContexts;
        }

        private IContextBoundSettingsStore ContextStore
        {
            get
            {
                var contextRange = ContextRange.Smart((lt, _) => _dataContexts.Empty);
                return _settingsStore.BindToContextTransient(contextRange);
            }
        }

        public TSettingsInstance GetSettings<TSettingsInstance>()
        {
            return ContextStore.GetKey<TSettingsInstance>(SettingsOptimization.DoMeSlowly);
        }

        public void SetSettings<TSettingsType>(TSettingsType settingsInstance) where TSettingsType : class
        {
            ContextStore.SetKey(settingsInstance, SettingsOptimization.DoMeSlowly);
        }
    }
}