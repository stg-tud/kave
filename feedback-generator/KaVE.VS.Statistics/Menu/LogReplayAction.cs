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

using System.ComponentModel;
using System.Windows.Forms;
using JetBrains.ActionManagement;
using JetBrains.Application;
using JetBrains.Application.DataContext;
using JetBrains.UI.ActionsRevised;
using KaVE.VS.Statistics.LogCollector;
using KaVE.VS.Statistics.Properties;
using KaVE.VS.Statistics.Utils;

namespace KaVE.VS.Statistics.Menu
{
    /// <summary>
    ///     ActionHandler for collecting Logs
    /// </summary>
    [ShellComponent, Action(Id, "Log Replay", Id = 8645740)]
    public class LogReplayAction : IExecutableAction
    {
        public const string Id = "KaVE.BP.Statistics.LogReplay";

        private readonly BackgroundWorker _backgroundWorker = new BackgroundWorker();
        private readonly LogReplay _logReplay;

        private LogReplayWindow _logReplayWindow;

        public LogReplayAction()
        {
            _logReplay = Registry.GetComponent<LogReplay>();

            InitializeBackgroundWorker();
        }

        public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
        {
            // return true or false to enable/disable this action
            return true;
        }

        public void Execute(IDataContext context, DelegateExecute nextExecute)
        {
            var result = MessageBox.Show(
                UIText.LogCollectorAction_ReadDialog,
                UIText.LogCollectorAction_Title,
                MessageBoxButtons.YesNo);

            if (result != DialogResult.Yes)
            {
                return;
            }

            _logReplayWindow = new LogReplayWindow();

            _logReplayWindow.Show();

            if (!_backgroundWorker.IsBusy)
            {
                _backgroundWorker.RunWorkerAsync();
            }
            else
            {
                MessageBox.Show(UIText.LogCollectorAction_IsBusy);
            }
        }

        private void InitializeBackgroundWorker()
        {
            _backgroundWorker.WorkerReportsProgress = true;
            _backgroundWorker.DoWork += bw_DoWork;
            _backgroundWorker.RunWorkerCompleted += bw_RunWorkerCompleted;
            _backgroundWorker.ProgressChanged += bw_ProgressChanged;
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            _logReplay.CollectEventsFromLog(worker);
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _logReplayWindow.Progress.Value = 100;
            _logReplayWindow.Close();
            MessageBox.Show(UIText.LogCollectorAction_Finished);
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var progressPercentage = e.ProgressPercentage;

            _logReplayWindow.Progress.Value = progressPercentage;
            _logReplayWindow.Title = string.Format("{0} - {1} %", UIText.LogCollectorAction_Title, progressPercentage);
        }
    }
}