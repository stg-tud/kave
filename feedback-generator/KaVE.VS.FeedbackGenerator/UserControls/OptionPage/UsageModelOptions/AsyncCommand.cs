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
using System.Windows.Input;
using JetBrains.UI.Extensions.Commands;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Assertion;
using KaVE.JetBrains.Annotations;

namespace KaVE.VS.FeedbackGenerator.UserControls.OptionPage.UsageModelOptions
{
    public class AsyncCommand<T> : ICommand<T>
    {
        [NotNull]
        private readonly IKaVEBackgroundWorker _backgroundWorker;

        [NotNull]
        private readonly Action<T> _onExecute;

        [NotNull]
        private readonly Func<T, bool> _canExecute;

        public AsyncCommand([NotNull] Action<T> onExecute,
            [NotNull] Func<T, bool> canExecute,
            [CanBeNull] IKaVEBackgroundWorker worker = null)
        {
            _onExecute = onExecute;
            _canExecute = canExecute;
            _backgroundWorker = worker ?? new KaVEBackgroundWorker();
        }

        public AsyncCommand([NotNull] Action<T> onExecute, [CanBeNull] IKaVEBackgroundWorker worker = null)
            : this(onExecute, parameter => true, worker) {}

        public void Execute([CanBeNull] T parameter)
        {
            DoWorkEventHandler executeAction = delegate { _onExecute(parameter); };
            _backgroundWorker.DoWork += executeAction;
            _backgroundWorker.RunWorkerCompleted += delegate { _backgroundWorker.DoWork -= executeAction; };
            _backgroundWorker.RunWorkerAsync(parameter);
        }

        public bool CanExecute([CanBeNull] T parameter)
        {
            return !_backgroundWorker.IsBusy && _canExecute(parameter);
        }

        public void Execute([CanBeNull] object parameter)
        {
            Asserts.That(parameter == null || parameter is T, "Wrong parameter type!");
            Execute((T) parameter);
        }

        public bool CanExecute([CanBeNull] object parameter)
        {
            Asserts.That(parameter == null || parameter is T, "Wrong parameter type!");
            return CanExecute((T) parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public override string ToString()
        {
            return this.ToStringReflection();
        }
    }

    public class AsyncCommand : AsyncCommand<object>
    {
        public AsyncCommand([NotNull] Action onExecute,
            [NotNull] Func<bool> canExecute,
            [CanBeNull] IKaVEBackgroundWorker worker = null)
            : base(o => onExecute(), o => canExecute(), worker) {}

        public AsyncCommand([NotNull] Action onExecute, [CanBeNull] IKaVEBackgroundWorker worker = null)
            : this(onExecute, () => true, worker) {}
    }
}