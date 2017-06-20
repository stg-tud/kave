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
using System.Text.RegularExpressions;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils.Assertion;

namespace KaVE.FeedbackProcessor.Preprocessing.Filters
{
    public class VersionFilter : IFilter
    {
        private readonly int _firstVersionIncluded;
        private readonly Regex _regex = new Regex("^0\\.(\\d+)(?:-default)?$", RegexOptions.IgnoreCase);

        public string Name
        {
            get { return string.Format("VersionFilter(>={0})", _firstVersionIncluded); }
        }

        public Func<IDEEvent, bool> Func
        {
            get { return Func2; }
        }

        public VersionFilter(int firstVersionIncluded)
        {
            _firstVersionIncluded = firstVersionIncluded;
        }

        public bool Func2(IDEEvent e)
        {
            if (string.IsNullOrEmpty(e.KaVEVersion))
            {
                return false;
            }


            var ms = _regex.Matches(e.KaVEVersion);
            if (ms.Count != 1)
            {
                return false;
            }
            var gs = ms[0].Groups;
            Asserts.That(gs.Count == 2);

            try
            {
                var num = int.Parse(gs[1].Value);
                return num >= _firstVersionIncluded;
            }
            catch
            {
                return false;
            }
        }
    }
}