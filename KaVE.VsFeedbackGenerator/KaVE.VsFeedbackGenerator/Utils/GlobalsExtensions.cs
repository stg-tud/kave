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
using EnvDTE;

namespace KaVE.VsFeedbackGenerator.Utils
{
    internal static class GlobalsExtensions
    {
        public static string GetValueOrDefault(this Globals globals, string globalName, string defaultValue)
        {
            return globals.VariableExists[globalName] ? globals.Get(globalName) : defaultValue;
        }

        public static string Get(this Globals globals, string globalName)
        {
            return globals[globalName] as string;
        }

        public static void SetValue(this Globals globals, string globalName, string value)
        {
            globals[globalName] = value;
            globals.VariablePersists[globalName] = true;
        }
    }
}