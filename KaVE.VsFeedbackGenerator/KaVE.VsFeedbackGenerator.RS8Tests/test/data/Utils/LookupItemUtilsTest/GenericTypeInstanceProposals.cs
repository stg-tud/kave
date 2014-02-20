using System;
using System.Collections.Generic;

namespace CodeExamples.CompletionProposals
{
    /// <summary>
    /// Example of completion on an instance with generic type. All type parameters
    /// are bound here.
    /// </summary>
    class GenericTypeInstanceProposals
    {
        private void TriggerCompletionHerein(IDictionary<string, object> dict)
        {
            dict.{caret}
        }
    }
}
