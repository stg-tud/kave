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

using System.Linq;
using System.Windows.Input;

namespace KaVE.Commons.Utils.IO
{
    public class KeyBinding
    {
        private readonly Key[][] _keyCombinations;

        public KeyBinding(Key[][] keyCombinations)
        {
            _keyCombinations = keyCombinations;
        }

        public Key[][] Combinations
        {
            get
            {
                return _keyCombinations;
            }
        }

        public bool IsPressed()
        {
            return _keyCombinations.Last().All(IsKeyDown);
        }

        private static bool IsKeyDown(Key key)
        {
            return key != Key.None && Keyboard.IsKeyDown(key);
        }
    }
}
