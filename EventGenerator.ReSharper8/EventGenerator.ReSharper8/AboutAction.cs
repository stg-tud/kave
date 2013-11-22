using System.Windows.Forms;
using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;

namespace SessionManager
{
    [ActionHandler("EventGenerator.ReSharper8.About")]
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
              "SessionManager\nT0a\n\n",
              "About SessionManager",
              MessageBoxButtons.OK,
              MessageBoxIcon.Information);
        }
    }
}
