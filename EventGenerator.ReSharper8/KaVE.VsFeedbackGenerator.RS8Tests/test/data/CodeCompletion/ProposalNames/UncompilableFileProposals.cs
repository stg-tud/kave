using System;
using System.IO;

namespace CodeExamples.CompletionProposals
{
    /// <summary>
    /// An example of completion in an uncompilable file.
    /// </summary>
    public class UncompilableFileProposals
    {
        public void Method()
        {
            this.{caret}
        }
    }
}
} // <-- this closing brace is too much
