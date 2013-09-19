using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using EnvDTE;
using KAVE.EventGenerator_VisualStudio10.Model;

namespace KAVE.EventGenerator_VisualStudio10.Generators
{
    [Export(typeof (VisualStudioEventGenerator))]
    internal class TextEditorEventGenerator : VisualStudioEventGenerator
    {
        private TextEditorEvents _textEditorEvents;

        protected override void Initialize()
        {
            _textEditorEvents = DTEEvents.TextEditorEvents;
            _textEditorEvents.LineChanged += TextEditorEvents_LineChanged;
        }

        void TextEditorEvents_LineChanged(TextPoint startPoint, TextPoint endPoint, int hint)
        {
            // TODO match up with commands that cause edits?
            var editEvent = Create<EditEvent>();
            editEvent.Line = startPoint.Line;
            editEvent.ChangeSize = endPoint.LineCharOffset - startPoint.LineCharOffset;
            Fire(editEvent);
        }
    }
}