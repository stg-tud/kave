using System;
using JetBrains.Application;
using KaVE.Utils;

namespace KaVE.VsFeedbackGenerator.Utils
{
    public interface ICallbackManager
    {
        void RegisterCallback(Action callBack, int delay);
        void RegisterCallback(Action executeOnceAWeek, DateTime nextNotificationTime);
    }


    [ShellComponent]
    public class CallbackManager : ICallbackManager
    {
        public void RegisterCallback(Action callback, int delay)
        {
            Invoke.Later(callback, delay);
        }

        public void RegisterCallback(Action callback, DateTime datetime)
        {
            Invoke.Later(callback, datetime);
        }
    }
}