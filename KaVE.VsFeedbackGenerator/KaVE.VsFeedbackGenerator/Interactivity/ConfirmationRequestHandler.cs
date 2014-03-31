using System.Windows;

namespace KaVE.VsFeedbackGenerator.Interactivity
{
    public class ConfirmationRequestHandler
    {
        private readonly Window _window;

        public ConfirmationRequestHandler(DependencyObject parentControl)
        {
            _window = Window.GetWindow(parentControl);
        }

        public void Handle(object sender, InteractionRequestedEventArgs<Confirmation> args)
        {
            var confirmation = args.Notification;
            var answer = _window == null
                ? MessageBox.Show(confirmation.Message, confirmation.Caption, MessageBoxButton.YesNo)
                : MessageBox.Show(_window, confirmation.Message, confirmation.Caption, MessageBoxButton.YesNo);
            confirmation.Confirmed = answer == MessageBoxResult.Yes;
            args.Callback();
        }
    }
}