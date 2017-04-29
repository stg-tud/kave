/*
 * Copyright 2017 Sebastian Proksch
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
using KaVE.Commons.Model.Naming.Types.Organization;
using KaVE.Commons.Utils.Collections;

namespace KaVE.FeedbackProcessor.StatisticsUltimate
{
    public class AssemblyCounter
    {
        private readonly IDictionary<IAssemblyName, int> _counts = new Dictionary<IAssemblyName, int>();

        public void Count(IKaVESet<IAssemblyName> asms)
        {
            foreach (var asm in asms)
            {
                if (_counts.ContainsKey(asm))
                {
                    _counts[asm]++;
                }
                else
                {
                    _counts[asm] = 1;
                }
            }
        }

        public IDictionary<IAssemblyName, int> Counts
        {
            get { return _counts; }
        }
    }
}