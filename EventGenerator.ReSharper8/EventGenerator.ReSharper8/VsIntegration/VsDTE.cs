using EnvDTE;
using JetBrains.Application;
using JetBrains.Application.Components;

namespace KaVE.EventGenerator.ReSharper8.VsIntegration
{
    [ShellComponent(ProgramConfigurations.VS_ADDIN)]
    public class VsDTE : IVsDTE
    {
        private readonly DTE _dte;

        public VsDTE(DTE dte)
        {
            _dte = dte;
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
