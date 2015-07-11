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

namespace KaVE.Commons.Utils.Concurrent
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
}