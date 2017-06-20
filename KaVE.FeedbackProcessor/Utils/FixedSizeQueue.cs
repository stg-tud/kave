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

using System.Collections.Generic;

namespace KaVE.FeedbackProcessor.Utils
{
    internal class FixedSizeQueue<T> : Queue<T>
    {
        public int Limit { get; set; }

        public FixedSizeQueue(int limit)
        {
            Limit = limit;
        }

        public new void Enqueue(T obj)
        {
            base.Enqueue(obj);

            if (Count > Limit)
            {
                Dequeue();
            }
        }
    }
}