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

using System;
using System.ComponentModel;

namespace KaVE.VsFeedbackGenerator.Utils
{
    public class BackgroundWorker<TResult>
    {
        private readonly BackgroundWorker _backgroundWorker;

        public delegate TResult DoWorkEventHandler(BackgroundWorker worker);

        public event DoWorkEventHandler DoWork = delegate { return default(TResult); };

        public delegate void WorkCompletedEventHandler(TResult result);

        public event WorkCompletedEventHandler WorkCompleted = delegate { }; 

        public delegate void WorkFailedEventHandler(Exception exception);

        public event WorkFailedEventHandler WorkFailed = delegate { };

        public delegate void ProgressChangedEventHandler(int percentageProgressed);

        public event ProgressChangedEventHandler ProgressChanged = delegate { }; 

        public BackgroundWorker()
        {
            _backgroundWorker = new BackgroundWorker{WorkerSupportsCancellation = false};
            _backgroundWorker.DoWork += DoWorkHandler;
            _backgroundWorker.RunWorkerCompleted += WorkCompletedHandler;
            _backgroundWorker.ProgressChanged += ProgressChangedHandler;
        }

        public bool WorkerReportsProgress
        {
            get { return _backgroundWorker.WorkerReportsProgress; }
            set { _backgroundWorker.WorkerReportsProgress = value; }
        }

        public bool IsBusy
        {
            get { return _backgroundWorker.IsBusy; }
        }

        private void DoWorkHandler(object sender, DoWorkEventArgs e)
        {
            e.Result = DoWork(_backgroundWorker);
        }

        private void WorkCompletedHandler(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                WorkFailed(e.Error);
            }
            else
            {
                WorkCompleted((TResult)e.Result);
            }
        }

        private void ProgressChangedHandler(object sender, ProgressChangedEventArgs e)
        {
            ProgressChanged(e.ProgressPercentage);
        }

        public void RunWorkerAsync()
        {
            _backgroundWorker.RunWorkerAsync();
        }
    }
}
