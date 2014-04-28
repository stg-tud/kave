using JetBrains.Application;

namespace KaVE.VsFeedbackGenerator.Utils
{
    [ShellComponent]
    public class DateUtils : IDateUtils
    {
        public System.DateTime Now
        {
            get { return System.DateTime.Now; }
        }
    }
}