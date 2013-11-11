using EnvDTE;

namespace KaVE.EventGenerator.ReSharper8.VsIntegration
{
    public interface IVsDTE
    {
        DTE DTE { get; }
    }
}