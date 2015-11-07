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
using KaVE.Commons.Utils.Assertion;
using KaVE.JetBrains.Annotations;

namespace KaVE.VS.FeedbackGenerator.UserControls.OptionPage.UsageModelOptions
{
    public class AsyncCommand<TIn> : ICommand<TIn>, IDisposable
    {
        [NotNull]
        private readonly Func<TIn, bool> _canExecute;

        [NotNull]
        private readonly BackgroundWorker _backgroundWorker;

        public AsyncCommand([NotNull] Action<TIn> onExecute, [NotNull] Func<TIn, bool> canExecute)
        {
            _canExecute = canExecute;

            _backgroundWorker = new BackgroundWorker();

            _backgroundWorker.DoWork += (sender, args) => { onExecute((TIn) args.Argument); };

            _backgroundWorker.RunWorkerCompleted += delegate
            {
                if (ExecuteCompleted != null)
                {
                    ExecuteCompleted(this, EventArgs.Empty);
                }
            };
        }

        public AsyncCommand([NotNull] Action<TIn> onExecute) : this(onExecute, parameter => true) {}

        public void Execute([CanBeNull] TIn parameter)
        {
            _backgroundWorker.RunWorkerAsync(parameter);
        }

        public bool CanExecute([CanBeNull] TIn parameter)
        {
            return !_backgroundWorker.IsBusy && _canExecute(parameter);
        }

        public void Execute([CanBeNull] object parameter)
        {
            Asserts.That(parameter == null || parameter is TIn, "Wrong parameter type!");
            Execute((TIn) parameter);
        }

        public bool CanExecute([CanBeNull] object parameter)
        {
            Asserts.That(parameter == null || parameter is TIn, "Wrong parameter type!");
            return CanExecute((TIn) parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public event EventHandler ExecuteCompleted;

        public void Dispose()
        {
            _backgroundWorker.Dispose();
        }
    }

    public class AsyncCommand : AsyncCommand<object>
    {
        public AsyncCommand([NotNull] Action onExecute, [NotNull] Func<bool> canExecute)
            : base(o => onExecute(), o => canExecute()) {}

        public AsyncCommand([NotNull] Action onExecute) : this(onExecute, () => true) {}
    }
}