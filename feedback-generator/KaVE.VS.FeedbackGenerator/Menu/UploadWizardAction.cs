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

using System.IO;
using System.Linq;
using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;
using JetBrains.UI.ActionsRevised;
using JetBrains.Util;
using KaVE.Commons.Utils;
using KaVE.RS.Commons.Settings;
using KaVE.RS.Commons.Utils;
using KaVE.VS.FeedbackGenerator.UserControls.UploadWizard;
using KaVE.VS.FeedbackGenerator.Utils.Export;
using KaVE.VS.FeedbackGenerator.Utils.Logging;
using NuGet;
using ILogger = KaVE.Commons.Utils.Exceptions.ILogger;

namespace KaVE.VS.FeedbackGenerator.Menu
{
    [Action(Id, "Export Feedback...", Id = 12193486)]
    public class UploadWizardAction : IExecutableAction
    {
        internal const string Id = "KaVE.VS.FeedbackGenerator.UploadWizard";

        private readonly ISettingsStore _settingsStore;
        private readonly IExporter _exporter;
        private readonly ILogManager _logManager;
        private readonly ILogger _logger;
        private readonly IDateUtils _dateUtils;

        public UploadWizardAction()
        {
            _settingsStore = Registry.GetComponent<ISettingsStore>();
            _exporter = Registry.GetComponent<IExporter>();
            _logManager = Registry.GetComponent<ILogManager>();
            _logger = Registry.GetComponent<ILogger>();
            _dateUtils = Registry.GetComponent<IDateUtils>();
        }

        public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
        {
            return true;
        }

        public void Execute(IDataContext context, DelegateExecute nextExecute)
        {
            if (HasContentToExport())
            {
                var viewModel = new UploadWizardContext(
                    _exporter,
                    _logManager,
                    _settingsStore,
                    _dateUtils,
                    _logger,
                    new RandomizationUtils());
                new UploadWizardControl(viewModel, _settingsStore).ShowDialog();
            }
            else
            {
                MessageBox.ShowInfo(Properties.UploadWizard.NothingToExport, UploadWizardMessages.Title);
            }
        }

        private bool HasContentToExport()
        {
            try
            {
                var logs = _logManager.Logs.ToList();
                var noLogs = EnumerableExtensions.IsEmpty(logs);
                var singleEmptyLog = logs.Count == 1 && logs[0].IsEmpty();
                return !noLogs && !singleEmptyLog;
            }
            catch (DirectoryNotFoundException)
            {
                // directly after the reset, before any event is generated
                return false;
            }
        }
    }
}