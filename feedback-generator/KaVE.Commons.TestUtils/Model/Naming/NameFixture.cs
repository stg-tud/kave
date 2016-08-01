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
using KaVE.Commons.Utils;

namespace KaVE.Commons.TestUtils.Model.Naming
{
    public class NameFixture
    {
        public const string Void = "p:void";
        public const string Bool = "p:bool";
        public const string Int = "p:int";
        public const string String = "p:string";
        public const string Object = "p:object";

        public static string ObjectArr(int rank)
        {
            return Arr(Object, rank);
        }

        public static string StringArr(int rank)
        {
            return Arr(String, rank);
        }

        public static string IntArr(int rank)
        {
            return Arr(Int, rank);
        }

        private static string Arr(string type, int rank)
        {
            var marker = new StringBuilder();
            marker.Append('[').Append(',', rank - 1).Append(']');
            return "{0}{1}".FormatEx(type, marker);
        }
    }
}