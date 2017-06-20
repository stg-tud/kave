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
using KaVE.FeedbackProcessor.Cleanup.Heuristics;

namespace KaVE.FeedbackProcessor.Utils
{
    internal class SortedCommandPair : Pair<string>
    {
        private SortedCommandPair(string first, string second) : base(first, second) {}

        public static SortedCommandPair NewSortedPair(string command1, string command2)
        {
            switch (CommandCompareHeuristic.CompareCommands(command1, command2))
            {
                case 1:
                    return new SortedCommandPair(command1, command2);
                case -1:
                    return new SortedCommandPair(command2, command1);
                default:
                    return String.Compare(command1, command2, StringComparison.InvariantCulture) > 0
                        ? new SortedCommandPair(command2, command1)
                        : new SortedCommandPair(command1, command2);
            }
        }
    }
}