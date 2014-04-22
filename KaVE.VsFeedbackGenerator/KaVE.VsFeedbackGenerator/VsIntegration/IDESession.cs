using System;
using System.Globalization;
using EnvDTE;
using JetBrains.Application;
using JetBrains.Application.Components;
using KaVE.JetBrains.Annotations;
using KaVE.VsFeedbackGenerator.Utils;

namespace KaVE.VsFeedbackGenerator.VsIntegration
{
    // TODO migrate class to R# settings
    [ShellComponent(ProgramConfigurations.VS_ADDIN)]
    public class IDESession : IIDESession
    {
        private const string UUIDGlobal = "KAVE_EventGenerator_SessionUUID";
        private const string UUIDCreatedAtGlobal = "KAVE_EventGenerator_SessionUUID_CreatedAt";
        private const string PastDate = "1987-06-20";

        private readonly DTE _dte;

        public IDESession([NotNull] DTE dte)
        {
            _dte = dte;
        }

        public string UUID
        {
            get
            {
                var globals = _dte.Globals;
                var storedDateString = globals.GetValueOrDefault(UUIDCreatedAtGlobal, PastDate);
                var createdAt = DateTime.Parse(storedDateString, CultureInfo.InvariantCulture);
                if (createdAt != DateTime.Today)
                {
                    var dateString = DateTime.Today.ToString(CultureInfo.InvariantCulture);
                    globals.SetValue(UUIDCreatedAtGlobal, dateString);
                    globals.SetValue(UUIDGlobal, Guid.NewGuid().ToString());
                }
                return globals.Get(UUIDGlobal);
            }
        }

        public DTE DTE
        {
            get
            {
                return _dte;
            }
        }
    }
}