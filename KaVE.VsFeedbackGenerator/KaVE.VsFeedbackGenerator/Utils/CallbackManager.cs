using System;
using JetBrains.Application;
using KaVE.Utils;

namespace KaVE.VsFeedbackGenerator.Utils
{
    [ShellComponent]
    public class CallbackManager
    {     
        public void RegisterCallback(Action callback, int delayInMillisecond)
        {
            Invoke.Later(callback, delayInMillisecond);
        }

        public void RegisterCallback(Action callback, DateTime timeForCallbackInvocation)
        {
            RegisterCallback(callback, timeForCallbackInvocation, () => {});
        }

        public void RegisterCallback(Action callback, DateTime timeForCallbackInvocation, Action finishedAction)
        {
            Invoke.Later(callback, timeForCallbackInvocation, finishedAction);
        }
    }
}