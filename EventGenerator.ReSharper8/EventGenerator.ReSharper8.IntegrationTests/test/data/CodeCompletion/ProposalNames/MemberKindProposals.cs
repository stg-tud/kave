namespace CodeExamples.CompletionProposals
{
    /// <summary>
    /// Examples of different kinds of members.
    /// </summary>
    public class MemberKindProposals
    {
        // not covered by this test, since static
        // see StaticMemberProposals.cs
        public const string Constant = "constant";

        public MemberKindProposals(string arg) {}

        // not covered by this test, since it is never explicitly invoked
        public ~MemberKindProposals() {}

        public object this[int i]
        {
            get { return this; }
        }

        // not covered by this test, since "static"
        // see StaticMemberProposals.cs
        public class Nested {}

        public delegate void Delegate(object obj);

        public event Delegate Event;

        public int Property { get; set; }

        public string _field;

        public void Method(object param) {}

        // not covered by this test, since it doesn't seem to appear in code completion
        public static object operator +(MemberKindProposals mkps, object obj)
        {
            return null;
        }

        public void TriggerCompletionHerein()
        {
            this.{caret}
        }
    }
}
