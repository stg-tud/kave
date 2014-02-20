using System;

namespace CodeExamples.CompletionProposals
{
    /// <summary>
    /// Examples of static-member completion.
    /// </summary>
    public class StaticMemberProposals
    {
        public const string Constant = "constant";

        public class Nested { }

        public static string _field;

        public static bool StaticMethod()
        {
            return false;
        }

        public void TriggerCompletionHerein()
        {
            StaticMemberProposals.{caret}
        }
    }
}
