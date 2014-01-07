using System;
using System.Windows;
using JetBrains.UI.Extensions.Commands;
using KaVE.Utils.Assertion;

namespace KaVE.VsFeedbackGenerator.Utils
{
    public static class ConfirmedCommand
    {
        public static DelegateCommand Create(Action execute,
            Func<string> confirmationTitle,
            Func<string> confirmationText,
            Func<bool> canExecute)
        {
            Asserts.NotNull(execute, "delegate action is null");
            var execute2 = (Action<object>) (param => execute());
            var canExecute2 = canExecute == null ? default(Predicate<object>) : param => canExecute();
            return Create(execute2, confirmationTitle, confirmationText, canExecute2);
        }

        public static DelegateCommand Create(Action<object> execute,
            Func<string> confirmationTitle,
            Func<string> confirmationText,
            Predicate<object> canExecute = null)
        {
            Asserts.NotNull(execute, "delegate action is null");
            Asserts.NotNull(confirmationTitle, "missing confirmation dialog title");
            Asserts.NotNull(confirmationText, "missing confirmation dialog text");
            return new DelegateCommand(
                parameter =>
                {
                    var confirmationResult = MessageBox.Show(
                        confirmationText(),
                        confirmationTitle(),
                        MessageBoxButton.YesNo);
                    if (confirmationResult == MessageBoxResult.Yes)
                    {
                        execute(parameter);
                    }
                },
                canExecute);
        }
    }
}