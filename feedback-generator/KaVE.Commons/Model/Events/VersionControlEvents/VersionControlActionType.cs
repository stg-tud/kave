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
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace KaVE.Commons.Model.Events.VersionControlEvents
{
    public enum VersionControlActionType
    {
        Unknown = 0,
        Branch,
        Checkout,
        Clone,
        Commit,
        CommitAmend,
        CommitInitial,
        Merge,
        Pull,
        Rebase,
        RebaseFinished,
        Reset
    }

    public static class VersionControlActionTypeExtensions
    {
        private static readonly Dictionary<Regex, VersionControlActionType> SpecialPatterns = new Dictionary
            <Regex, VersionControlActionType>
        {
            {new Regex(@"commit \(amend\)"), VersionControlActionType.CommitAmend},
            {new Regex(@"commit \(initial\)"), VersionControlActionType.CommitInitial},
            {new Regex("rebase finished"), VersionControlActionType.RebaseFinished},
            {new Regex("pull.*"), VersionControlActionType.Pull},
            {new Regex("merge.*"), VersionControlActionType.Merge}
        };

        public static VersionControlActionType ToVersionControlActionType(this string value)
        {
            try
            {
                return (VersionControlActionType) Enum.Parse(typeof (VersionControlActionType), value, true);
            }
            catch
            {
                return HandleSpecialCases(value);
            }
        }

        private static VersionControlActionType HandleSpecialCases(string value)
        {
            var match = SpecialPatterns.Keys.FirstOrDefault(regex => regex.IsMatch(value));
            return match != null ? SpecialPatterns[match] : VersionControlActionType.Unknown;
        }
    }
}