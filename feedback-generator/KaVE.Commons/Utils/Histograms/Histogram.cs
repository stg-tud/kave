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
using KaVE.Commons.Utils.Assertion;

namespace KaVE.Commons.Utils.Histograms
{
    public class Histogram
    {
        private readonly int _numSlots;
        private readonly IDictionary<int, int> _slots;

        public Histogram(int numSlots)
        {
            _numSlots = numSlots;
            NumValues = 0;
            _slots = CreateNewDictionary(0);
        }

        private IDictionary<int, T> CreateNewDictionary<T>(T defaultValue)
        {
            var dict = new Dictionary<int, T>();
            for (var i = 1; i <= _numSlots; i++)
            {
                dict[i] = defaultValue;
            }
            return dict;
        }

        public bool IsEmpty
        {
            get { return NumValues == 0; }
        }

        public int NumValues { get; private set; }

        public void Add(int slot)
        {
            Asserts.That(slot > 0);
            Asserts.That(slot <= _numSlots);
            NumValues++;
            _slots[slot]++;
        }

        public IDictionary<int, int> Values
        {
            get { return _slots; }
        }

        public IDictionary<int, double> ValuesRelative
        {
            get
            {
                var dict = CreateNewDictionary(0d);
                if (!IsEmpty)
                {
                    for (var i = 1; i <= _numSlots; i++)
                    {
                        dict[i] = _slots[i]/(1.0*NumValues);
                    }
                }
                return dict;
            }
        }
    }
}