using System;
using JetBrains.Application;
using KaVE.Utils;

namespace KaVE.VsFeedbackGenerator.Utils
{
    [ShellComponent]
    public class CallbackManager
    {
        public void RegisterCallback(Action callBack, int delay)
        {
            Invoke.Later(callBack, delay);
        }
    }
}
