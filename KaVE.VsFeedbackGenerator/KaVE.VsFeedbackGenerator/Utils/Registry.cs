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