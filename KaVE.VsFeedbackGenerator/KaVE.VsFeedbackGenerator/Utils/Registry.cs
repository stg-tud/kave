using System.Collections.Generic;
using JetBrains.Application;

namespace KaVE.VsFeedbackGenerator.Utils
{
    // TODO document intended usage
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
            ComponentDictionary.Add(typeof(T), instance);
        }

        public static void Clear()
        {
            ComponentDictionary.Clear();
        }
    }
}