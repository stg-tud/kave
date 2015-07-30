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
using JetBrains.ReSharper.Resources.Shell;

namespace KaVE.VS.Statistics.Utils
{
    public static class Registry
    {
        private static readonly IDictionary<object, object> ComponentDictionary = new Dictionary<object, object>();

        public static T GetComponent<T>() where T : class
        {
            if (ComponentDictionary.ContainsKey(typeof (T)))
            {
                return (T) ComponentDictionary[typeof (T)];
            }
            return Shell.Instance.GetComponent<T>();
        }

        public static IEnumerable<T> GetComponents<T>() where T : class
        {
            if (ComponentDictionary.ContainsKey(typeof (T)))
            {
                return (IEnumerable<T>) ComponentDictionary[typeof (T)];
            }
            return Shell.Instance.GetComponents<T>();
        }

        public static void RegisterComponent<T>(T instance) where T : class
        {
            ComponentDictionary.Add(typeof (T), instance);
        }

        public static void RegisterComponents<T>(IEnumerable<T> components) where T : class
        {
            ComponentDictionary.Add(typeof (T), components);
        }

        public static void Clear()
        {
            ComponentDictionary.Clear();
        }
    }
}