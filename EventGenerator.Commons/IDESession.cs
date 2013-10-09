using System;
using System.Globalization;
using EnvDTE;

namespace EventGenerator.Commons
{
    public static class IDESession
    {
        private const string UUIDGlobal = "KAVE.EventGenerator.SessionUUID";
        private const string UUIDCreatedAtGlobal = "KAVE.EventGenerator.SessionUUID.CreatedAt";
        private const string PastDate = "1987-06-20";

        public static string GetUUID(DTE dte)
        {
            var globals = dte.Globals;
            var createdAt = DateTime.Parse(globals.GetValueOrDefault(UUIDCreatedAtGlobal, PastDate));
            if (createdAt < DateTime.Today)
            {
                globals.SetValue(UUIDCreatedAtGlobal, DateTime.Today.ToString(CultureInfo.InvariantCulture));
                globals.SetValue(UUIDGlobal, Guid.NewGuid().ToString());
            }
            return globals[UUIDGlobal];
        }

        private static string GetValueOrDefault(this Globals globals, string globalName, string defaultValue)
        {
            return globals.VariableExists[globalName] ? globals[globalName] : defaultValue;
        }

        private static void SetValue(this Globals globals, string globalName, string value)
        {
            globals[globalName] = value;
            globals.VariablePersists[globalName] = true;
        }
    }
}