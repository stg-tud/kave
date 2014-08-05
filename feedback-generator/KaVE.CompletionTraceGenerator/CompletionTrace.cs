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
using System.Linq;
using KaVE.CompletionTraceGenerator.Model;
using KaVE.Utils;

namespace KaVE.CompletionTraceGenerator
{
    public class CompletionTrace
    {
        public CompletionTrace()
        {
            Actions = new List<CompletionAction>();
        }

        public long DurationInMillis { get; set; }
        public IList<CompletionAction> Actions { get; private set; }

        public void AppendAction(CompletionAction action)
        {
            Actions.Add(action);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        protected bool Equals(CompletionTrace other)
        {
            return DurationInMillis == other.DurationInMillis && Actions.SequenceEqual(other.Actions);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (DurationInMillis.GetHashCode()*397) ^ (Actions != null ? Actions.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("[DurationInMillis: {0}, Actions: [{1}]]", DurationInMillis, string.Join(",", Actions));
        }
    }
}