using EnvDTE;

namespace KaVE.VsFeedbackGenerator.VsIntegration
{
    public interface IIDESession
    {
        string UUID
        {
            get;
        }

        DTE DTE
        {
            get;
        }
    }
}