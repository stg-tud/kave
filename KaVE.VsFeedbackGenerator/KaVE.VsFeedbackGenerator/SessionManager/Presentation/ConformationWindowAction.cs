using System;
using System.Windows;
using System.Windows.Interactivity;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

namespace KaVE.VsFeedbackGenerator.SessionManager.Presentation
{
    public class ConfirmationDialogAction : TriggerAction<FrameworkElement>
    {
        protected override void Invoke(object parameter)
        {
            var args = parameter as InteractionRequestedEventArgs;
            if (args == null)
            {
                return;
            }
            var confirmation = args.Context as Confirmation;
            if (confirmation == null)
            {
                return;
            }
            AskForConfirmation(confirmation, args);
        }

        private static void AskForConfirmation(Confirmation confirmation, InteractionRequestedEventArgs args)
        {
            var window = new ConfirmationDialog {DataContext = confirmation};
            EventHandler closeHandler = null;
            closeHandler = (sender, e) =>
            {
                window.Closed -= closeHandler;
                args.Callback();
            };
            window.Closed += closeHandler;
            window.Show();
        }
    }
}