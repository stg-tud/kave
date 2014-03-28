using System;
using JetBrains.Application;
using KaVE.Utils;

namespace KaVE.VsFeedbackGenerator.Utils
{
    public interface ICallbackManager
    {
        void RegisterCallback(Action callback, int delayInMillisecond);
        void RegisterCallback(Action callback, DateTime timeForCallbackInvocation, Action finishedAction);
    }

    [ShellComponent]
    public class CallbackManager : ICallbackManager
    {
        public void RegisterCallback(Action callback, int delayInMillisecond)
        {
            Invoke.Later(callback, delayInMillisecond);
        }

        public void RegisterCallback(Action callback, DateTime timeForCallbackInvocation, Action finishedAction)
        {
            Invoke.Later(callback, timeForCallbackInvocation, finishedAction);
        }
    }
}