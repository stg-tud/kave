using System.Collections.Generic;
using System.ComponentModel.Composition;
using EnvDTE;
using KAVE.EventGenerator_VisualStudio10.Model;

namespace KAVE.EventGenerator_VisualStudio10.Generators
{
    [Export(typeof(VisualStudioEventGenerator))]
    internal class IDEModeEventGenerator : VisualStudioEventGenerator
    {
        protected override void Initialize()
        {
            DTEEvents.DTEEvents.ModeChanged += DTEEvents_ModeChanged;
        }

        void DTEEvents_ModeChanged(vsIDEMode lastMode)
        {
            var ideModeEvent = Create<IDEModeEvent>();
            ideModeEvent.SwitchTo = lastMode == vsIDEMode.vsIDEModeDebug ? IDEModeEvent.IDEMode.Design : IDEModeEvent.IDEMode.Debug;
            Fire(ideModeEvent);
        }
    }
}