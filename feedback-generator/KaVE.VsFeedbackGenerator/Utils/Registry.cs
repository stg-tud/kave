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
 * 
 * Contributors:
 *    - Sven Amann
 */

using System.Collections.Generic;
using JetBrains.Application;

namespace KaVE.VsFeedbackGenerator.Utils
{
    /// <summary>
    ///     The registry should be used to access R# components, if injection is not possible or appropriate.
    /// </summary>
    /// <remarks>
    ///     The method <see cref="GetComponent{T}" /> works much like <see cref="M:Shell.Instance.GetComponent{T}" />, except
    ///     that the registry can be set up for testing purpose.
    ///     When testing, mock components can be registered using <see cref="RegisterComponent{T}" />. Registered instances
    ///     take precedence over Shell instances. In the test-teardown phase, the the registry should be cleared using
    ///     <see cref="Clear" />.
    /// </remarks>
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

        public static void RegisterComponent<T>(T instance) where T : class
        {
            ComponentDictionary.Add(typeof (T), instance);
        }

        public static void Clear()
        {
            ComponentDictionary.Clear();
        }
    }
}