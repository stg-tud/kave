using System.Windows.Forms;
using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;

namespace KaVE.VsFeedbackGenerator
{
  [ActionHandler("KaVE.VsFeedbackGenerator.About")]
  public class AboutAction : IActionHandler
  {
    public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
    {
      // return true or false to enable/disable this action
      return true;
    }

    public void Execute(IDataContext context, DelegateExecute nextExecute)
    {
      MessageBox.Show(
        "KaVE Feedback\nTU Darmstadt\n\n",
        "About KaVE Feedback",
        MessageBoxButtons.OK,
        MessageBoxIcon.Information);
    }
  }
}
