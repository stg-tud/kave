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

using System;
using System.ComponentModel;

namespace KaVE.ReSharper.Commons.Utils
{
    public class BackgroundWorker<TResult> : BackgroundWorkerBase<object, TResult>
    {
        public delegate TResult DoWorkEventHandler(BackgroundWorker worker);

        public event DoWorkEventHandler DoWork = delegate { return default(TResult); };

        public void RunWorkerAsync()
        {
            InternalWorker.RunWorkerAsync();
        }

        protected override TResult DoWorkHandler(BackgroundWorker worker, object argument)
        {
            return DoWork(worker);
        }
    }

    public class BackgroundWorker<TArgument, TResult> : BackgroundWorkerBase<TArgument, TResult>
    {
        public delegate TResult DoWorkEventHandler(BackgroundWorker worker, TArgument argument);

        public event DoWorkEventHandler DoWork = delegate { return default(TResult); };

        protected override TResult DoWorkHandler(BackgroundWorker worker, TArgument argument)
        {
            return DoWork(worker, argument);
        }
    }

    public abstract class BackgroundWorkerBase<TArgument, TResult>
    {
        protected readonly BackgroundWorker InternalWorker;

        public delegate void WorkCompletedEventHandler(TResult result);

        public event WorkCompletedEventHandler WorkCompleted = delegate { };

        public delegate void WorkFailedEventHandler(Exception exception);

        public event WorkFailedEventHandler WorkFailed = delegate { };

        public delegate void ProgressChangedEventHandler(int percentageProgressed);

        public event ProgressChangedEventHandler ProgressChanged = delegate { };

        protected BackgroundWorkerBase()
        {
            InternalWorker = new BackgroundWorker {WorkerSupportsCancellation = false};
            InternalWorker.DoWork += DoWorkHandler;
            InternalWorker.RunWorkerCompleted += WorkCompletedHandler;
            InternalWorker.ProgressChanged += ProgressChangedHandler;
        }

        public bool WorkerReportsProgress
        {
            get { return InternalWorker.WorkerReportsProgress; }
            set { InternalWorker.WorkerReportsProgress = value; }
        }

        public bool IsBusy
        {
            get { return InternalWorker.IsBusy; }
        }

        private void DoWorkHandler(object sender, DoWorkEventArgs e)
        {
            e.Result = DoWorkHandler(InternalWorker, (TArgument) e.Argument);
        }

        protected abstract TResult DoWorkHandler(BackgroundWorker worker, TArgument argument);

        private void WorkCompletedHandler(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                WorkFailed(e.Error);
            }
            else
            {
                WorkCompleted((TResult) e.Result);
            }
        }

        private void ProgressChangedHandler(object sender, ProgressChangedEventArgs e)
        {
            ProgressChanged(e.ProgressPercentage);
        }

        public void RunWorkerAsync(TArgument argument)
        {
            InternalWorker.RunWorkerAsync(argument);
        }
    }
}