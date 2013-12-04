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