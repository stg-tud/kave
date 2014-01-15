namespace KaVE.Model.Groum
{
    public class LockGorum : IGroum
    {
        public LockGorum(string identifier, IGroum bodyGroum)
        {
            Identifier = identifier;
            BodyGroum = bodyGroum;
        }

        public string Identifier { get; private set; }
        public IGroum BodyGroum { get; private set; }
    }
}