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
using System.Linq.Expressions;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Reflection;

namespace KaVE.VS.FeedbackGenerator.SessionManager
{
    public abstract class ViewModelBase<T> : INotifyPropertyChanged where T : ViewModelBase<T>
    {
        private readonly string[] _suffixes = {"   ", ".  ", ".. ", "..."};

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private bool _isBusy;
        private string _busyMessage;
        private int _suffixIndex;
        public  int AnimationRefreshDelayInMillis { get; set; }

        protected ViewModelBase()
        {
            AnimationRefreshDelayInMillis = 1000;
            this.OnPropertyChanged(
                self => self.IsBusy,
                amBusy => ScheduleBusyMessageAnimation());
        }

        private void ScheduleBusyMessageAnimation()
        {
            if (IsBusy)
            {
                Invoke.Later(
                    () =>
                    {
                        _suffixIndex = (_suffixIndex + 1) % _suffixes.Length;
                        RaisePropertyChanged(self => self.BusyMessageAnimated);

                    }, AnimationRefreshDelayInMillis, ScheduleBusyMessageAnimation);
            }
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void RaisePropertyChanged<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            RaisePropertyChanged(TypeExtensions<T>.GetPropertyName(expression));
        }

        protected void SetBusy(string reason)
        {
            BusyMessage = reason;
            IsBusy = true;
        }

        protected void SetIdle()
        {
            IsBusy = false;
        }

        /// <summary>
        ///     Indicates that the view is busy performing some background task. No calls should be issued on the model if this is
        ///     true.
        /// </summary>
        public bool IsBusy
        {
            get { return _isBusy; }
            private set
            {
                _isBusy = value;
                RaisePropertyChanged(vm => vm.IsBusy);
            }
        }

        public string BusyMessage
        {
            get { return _busyMessage; }
            set
            {
                _busyMessage = value;
                RaisePropertyChanged(self => self.BusyMessage);
                RaisePropertyChanged(self => self.BusyMessageAnimated);
            }
        }

        public string BusyMessageAnimated
        {
            get { return BusyMessage + _suffixes[_suffixIndex]; }
        }

        /// <summary>
        ///     When a registered sub view model becomes busy, so does its parent.
        ///     <b>Make sure there's only one busy view model at a time!</b>
        /// </summary>
        protected void RegisterSubViewModel<TM>(ViewModelBase<TM> subViewModel) where TM : ViewModelBase<TM>
        {
            subViewModel.OnPropertyChanged(svm => svm.IsBusy, busy => IsBusy = busy);
            subViewModel.OnPropertyChanged(svm => svm.BusyMessage, msg => BusyMessage = msg);
        }
    }
}