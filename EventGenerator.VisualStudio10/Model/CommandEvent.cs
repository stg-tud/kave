using CodeCompletion.Model;
using CodeCompletion.Model.Names.VisualStudio;

namespace KAVE.EventGenerator_VisualStudio10.Model
{
    public class CommandEvent : IDEEvent
    {
        public const string EventKind = "Command";

        public CommandEvent()
            : base(EventKind)
        {
        }

        public CommandName Command { get; internal set; }
        public CommandBarControlName Source { get; internal set; }
    }
}
