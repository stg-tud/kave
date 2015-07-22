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
using System.Threading;
using System.Threading.Tasks;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Utils.Concurrency
{
    public class Task : System.Threading.Tasks.Task
    {
        public Task([NotNull] Action action) : base(action) {}
        public Task([NotNull] Action action, CancellationToken cancellationToken) : base(action, cancellationToken) {}
        public Task([NotNull] Action action, TaskCreationOptions creationOptions) : base(action, creationOptions) {}

        public Task([NotNull] Action action, CancellationToken cancellationToken, TaskCreationOptions creationOptions)
            : base(action, cancellationToken, creationOptions) {}

        public Task([NotNull] Action<object> action, object state) : base(action, state) {}

        public Task([NotNull] Action<object> action, object state, CancellationToken cancellationToken)
            : base(action, state, cancellationToken) {}

        public Task([NotNull] Action<object> action, object state, TaskCreationOptions creationOptions)
            : base(action, state, creationOptions) {}

        public Task([NotNull] Action<object> action,
            object state,
            CancellationToken cancellationToken,
            TaskCreationOptions creationOptions) : base(action, state, cancellationToken, creationOptions) {}

        public new static TaskFactory Factory
        {
            get { return System.Threading.Tasks.Task.Factory; }
        }

        public static void StartNewLongRunning(Action action)
        {
            Factory.StartNew(action, TaskCreationOptions.LongRunning);
        }
    }

    public class Task<T> : System.Threading.Tasks.Task<T>
    {
        public Task(Func<T> function) : base(function) {}
        public Task(Func<T> function, CancellationToken cancellationToken) : base(function, cancellationToken) {}
        public Task(Func<T> function, TaskCreationOptions creationOptions) : base(function, creationOptions) {}
        public Task(Func<T> function, CancellationToken cancellationToken, TaskCreationOptions creationOptions) : base(function, cancellationToken, creationOptions) {}
        public Task(Func<object, T> function, object state) : base(function, state) {}
        public Task(Func<object, T> function, object state, CancellationToken cancellationToken) : base(function, state, cancellationToken) {}
        public Task(Func<object, T> function, object state, TaskCreationOptions creationOptions) : base(function, state, creationOptions) {}
        public Task(Func<object, T> function, object state, CancellationToken cancellationToken, TaskCreationOptions creationOptions) : base(function, state, cancellationToken, creationOptions) {}
    }
}