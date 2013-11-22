using System;

namespace CodeExamples.CompletionProposals
{
    /// <summary>
    /// Examples of namespace completion.
    /// </summary>
    public class NamespaceProposals
    {
        public void TriggerCompletionHerein()
        {
            CodeExamples.CompletionProposals.{caret}
        }
    }

    namespace SubNamespace
    {
        public class Foo {}
    }
}
