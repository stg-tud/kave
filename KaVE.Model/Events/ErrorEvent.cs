using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaVE.Model.Events
{
    public class ErrorEvent : IDEEvent
    {
        public string Content { get; set; }
        public string[] StackTrace { get; set; }
    }
}
