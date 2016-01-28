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

namespace KaVE.Commons.Utils
{
    public class ValueTypeWrapper<T>
    {
        public T Value { get; set; }

        public ValueTypeWrapper(T initial)
        {
            Value = initial;
        }

        public static implicit operator T(ValueTypeWrapper<T> wrapper)
        {
            return wrapper.Value;
        }
    }
}