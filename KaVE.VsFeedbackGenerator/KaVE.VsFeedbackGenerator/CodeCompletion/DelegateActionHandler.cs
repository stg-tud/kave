using System;
using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;

namespace KaVE.VsFeedbackGenerator.CodeCompletion
{
    internal class DelegateActionHandler : IActionHandler
    {
        private readonly Action _action;

        public DelegateActionHandler(Action action)
        {
            _action = action;
        }

        public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
        {
            return nextUpdate();
        }

        public void Execute(IDataContext context, DelegateExecute nextExecute)
        {
            nextExecute();
            _action();
        }
    }
}