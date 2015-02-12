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
 * 
 * Contributors:
 *    - Sven Amann
 */

using System.Linq;
using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;
using JetBrains.Util;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.Utils.Logging;
using NuGet;
using ILogger = KaVE.Utils.Exceptions.ILogger;

namespace KaVE.VsFeedbackGenerator.Export
{
    [ActionHandler(ActionId)]
    public class UploadWizardActionHandler : IActionHandler
    {
        internal const string ActionId = "KaVE.VsFeedbackGenerator.UploadWizard";

        private readonly ISettingsStore _settingsStore;
        private readonly IExporter _exporter;
        private readonly ILogManager _logManager;
        private readonly ILogger _logger;
        private readonly IDateUtils _dateUtils;
        private readonly IActionManager _actionManager;

        public static void Execute(IActionManager actionManager)
        {
            actionManager.ExecuteActionGuarded(ActionId, "KaVE.Feedback.Export");
        }

        public UploadWizardActionHandler(IActionManager actionManager, ISettingsStore settingsStore, IExporter exporter, ILogManager logManager, ILogger logger, IDateUtils dateUtils)
        {
            _actionManager = actionManager;
            _settingsStore = settingsStore;
            _exporter = exporter;
            _logManager = logManager;
            _logger = logger;
            _dateUtils = dateUtils;
        }

        public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
        {
            return true;
        }

        public void Execute(IDataContext context, DelegateExecute nextExecute)
        {
            if (HasContentToExport())
            {
                var viewModel = new UploadWizardViewModel(_exporter, _logManager, _settingsStore, _dateUtils, _logger);
                new UploadWizard(_actionManager, viewModel).ShowDialog();
            }
            else
            {
                MessageBox.ShowInfo(Properties.UploadWizard.NothingToExport, Properties.UploadWizard.window_title);
            }
        }

        private bool HasContentToExport()
        {
            var logs = _logManager.Logs.ToList();
            var noLogs = EnumerableExtensions.IsEmpty(logs);
            var singleEmptyLog = logs.Count == 1 && logs[0].IsEmpty();
            return !noLogs && !singleEmptyLog;
        }
    }
}