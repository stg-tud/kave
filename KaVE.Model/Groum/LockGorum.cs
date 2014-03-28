namespace KaVE.Model.Groum
{
    public class LockGorum : GroumBase
    {
        public LockGorum(string identifier, GroumBase bodyGroum)
        {
            Identifier = identifier;
            BodyGroum = bodyGroum;
        }

        public string Identifier { get; private set; }
        public GroumBase BodyGroum { get; private set; }
    }
}