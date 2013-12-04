using System;
using System.Globalization;
using EnvDTE;
using JetBrains.Application;
using JetBrains.Application.Components;
using KaVE.JetBrains.Annotations;
using KaVE.VsFeedbackGenerator.Utils;

namespace KaVE.VsFeedbackGenerator.VsIntegration
{
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
                // TODO test whether this is working properly in a non-experimental instance
                // in the experimental instance updates of the globals are not persisted reliably...
                var globals = _dte.Globals;
                var createdAt = DateTime.Parse(globals.GetValueOrDefault(UUIDCreatedAtGlobal, PastDate));
                if (createdAt < DateTime.Today)
                {
                    globals.SetValue(UUIDCreatedAtGlobal, DateTime.Today.ToString(CultureInfo.InvariantCulture));
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