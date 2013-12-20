using System;
using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;
using JetBrains.DataFlow;
using JetBrains.Util.Logging;

namespace KaVE.VsFeedbackGenerator.Tests.Mocking.Actions
{
    internal class MockExecutableAction : IExecutableAction
    {
        private readonly HandlersList _handlers;

        public IDataContext DataContext { get; private set; }

        public bool ShowConflict { get; set; }

        public MockExecutableAction(string id)
        {
            Id = id;
            _handlers = new HandlersList(id);
            ShowConflict = true;
        }

        internal IActionHandler GetNextHandler(ref int i)
        {
            while (i >= 0)
            {
                var handler = _handlers.GetHandler(i);
                if (!_handlers.IsRemoved(handler))
                    return handler;
                --i;
            }
            return null;
        }

        public void Execute(IDataContext context)
        {
            using (new HandlersList.Cookie(_handlers))
            {
                DataContext = context;
                var i = _handlers.Count - 1;
                var mockNextActionHandler = new MockNextActionHandler(this, i);
                try
                {
                    GetNextHandler(ref i).Execute(context, mockNextActionHandler.CallExecute);
                }
                catch (Exception ex)
                {
                    Logger.LogException(string.Format("An exception has occurred during action '{0}' execution.", Id), ex);
                }
                finally
                {
                    DataContext = null;
                }
            }
        }

        public void AddHandler(Lifetime lifetime, IActionHandler handler)
        {
            _handlers.AddHandler(handler);
        }

        public void RemoveHandler(IActionHandler handler)
        {
            _handlers.RemoveHandler(handler);
        }

        public bool Update(IDataContext context)
        {
            throw new NotImplementedException();
        }

        public string OverridenAction { get; set; }
        public string Id { get; private set; }
        public ActionPresentation Presentation {
            get { return new ActionPresentation(); }
        }
    }
}