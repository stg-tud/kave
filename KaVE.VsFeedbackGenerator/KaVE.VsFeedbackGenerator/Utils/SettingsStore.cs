using JetBrains.Application;
using JetBrains.Application.DataContext;
using JetBrains.Application.Settings;
using RSISettingsStore = JetBrains.Application.Settings.ISettingsStore;

namespace KaVE.VsFeedbackGenerator.Utils
{
    public interface ISettingsStore
    {
        TSettingsInstance GetSettings<TSettingsInstance>();
        void SetSettings<TSettingsType>(TSettingsType settingsInstance) where TSettingsType : class;
    }

    [ShellComponent]
    public class SettingsStore : ISettingsStore
    {
        private readonly RSISettingsStore _settingsStore;
        private readonly DataContexts _dataContexts;

        public SettingsStore(RSISettingsStore settingsStore, DataContexts dataContexts)
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