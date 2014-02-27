using JetBrains.ActionManagement;

namespace KaVE.VsFeedbackGenerator.Tests.TestFactories.Actions
{
    internal class MockNextActionHandler
    {
        private readonly MockExecutableAction _action;
        private int _nextIndex;

        private IActionHandler NextHandler
        {
            get
            {
                return _action.GetNextHandler(ref _nextIndex);
            }
        }

        private MockNextActionHandler MockNext
        {
            get
            {
                return new MockNextActionHandler(_action, _nextIndex);
            }
        }

        public MockNextActionHandler(MockExecutableAction action, int index)
        {
            _action = action;
            _nextIndex = index - 1;
        }

        public bool CallUpdate()
        {
            return NextHandler.Update(_action.DataContext, _action.Presentation, MockNext.CallUpdate);
        }

        public void CallExecute()
        {
            NextHandler.Execute(_action.DataContext, MockNext.CallExecute);
        }
    }
}