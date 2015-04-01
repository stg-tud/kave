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
using System.Text;

namespace KaVE.Commons.Utils
{
    public static class StringBuilderUtils
    {
        public static void AppendIf(this StringBuilder identifier, bool condition, string stringToAppend)
        {
            if (condition)
            {
                identifier.Append(stringToAppend);
            }
        }

        public static StringBuilder Append(this StringBuilder builder, params string[] values)
        {
            foreach (var value in values)
            {
                builder.Append(value);
            }
            return builder;
        }
    }
}
