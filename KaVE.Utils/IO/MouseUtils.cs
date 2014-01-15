using System.Windows.Input;

namespace KaVE.Utils.IO
{
    public class MouseUtils
    {
        public static bool IsLeftMouseButtonPressed()
        {
            return Invoke.OnSTA(() => Mouse.LeftButton == MouseButtonState.Pressed);
        }
    }
}
