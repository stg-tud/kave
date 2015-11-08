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

namespace KaVE.Commons.Utils
{
    // ReSharper disable once InconsistentNaming
    public interface IKaVEBackgroundWorker
    {
        bool CancellationPending { get; }
        void CancelAsync();
        event DoWorkEventHandler DoWork;
        bool IsBusy { get; }
        event ProgressChangedEventHandler ProgressChanged;
        void ReportProgress(int percentProgress);
        void ReportProgress(int percentProgess, object userState);
        void RunWorkerAsync();
        void RunWorkerAsync(object argument);
        event RunWorkerCompletedEventHandler RunWorkerCompleted;
        bool WorkerReportsProgress { get; set; }
        bool WorkerSupportsCancellation { get; set; }
    }

    public class KaVEBackgroundWorker : BackgroundWorker, IKaVEBackgroundWorker {}
}