using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.CommandBars;

namespace KAVE.EventGenerator_VisualStudio10.Generators
{
    // TODO find a way for this to work (fast!)
    //[Export(typeof(VisualStudioEventGenerator))]
    internal class CommandBarEventGenerator : VisualStudioEventGenerator
    {
        private IEnumerable<CommandBarEvents> _commandBarsControlsEvents;
        private IEnumerable<CommandBar> _commandBars;
        private IEnumerable<CommandBarControl> _commandBarsControls;

        protected override void Initialize()
        {
            _commandBars = CommandBars.Cast<CommandBar>().ToList();
            _commandBarsControls = _commandBars.SelectMany(bar => CommandBarsLeafControls(bar.Controls)).ToList();
            _commandBarsControlsEvents = _commandBarsControls.Select(CommandBarEvents).ToList();
            foreach (var commandBarControlEvents in _commandBarsControlsEvents)
            {
                commandBarControlEvents.Click += CommandBarEvents_Click;
            }
        }

        private IEnumerable<CommandBarControl> CommandBarsLeafControls(CommandBarControls controls)
        {
            ISet<CommandBarControl> leafs = new HashSet<CommandBarControl>();
            foreach (CommandBarControl commandBarControl in controls)
            {
                var popup = commandBarControl as CommandBarPopup;
                if (popup != null)
                {
                    leafs = new HashSet<CommandBarControl>(leafs.Union(CommandBarsLeafControls(popup.Controls)));
                }
                else
                {
                    leafs.Add(commandBarControl);
                }
            }
            return leafs;
        }

        private CommandBars CommandBars { get { return (CommandBars)DTE.CommandBars; } }

        private CommandBarEvents CommandBarEvents(object control)
        {
            return (CommandBarEvents)DTEEvents.CommandBarEvents[control];
        }

        void CommandBarEvents_Click(object commandBarControl, ref bool handled, ref bool cancelDefault)
        {
            var barControl = (CommandBarControl)commandBarControl;
            var commandBar = barControl.Parent;
            Debug.WriteLine("COMMAND BAR CONTROL " + barControl.Caption + " FROM BAR " + commandBar.Name + " WAS CLICKED #########################################################");
        }
    }
}