namespace KaVE.Model.Groum
{
    public class UsingGroum : IGroum
    {
        public UsingGroum(IGroum initializerName, string identifier, IGroum bodyGroum)
        {
            InitializerGroum = initializerName;
            Identifier = identifier;
            BodyGroum = bodyGroum;
        }

        public IGroum InitializerGroum { get; private set; }
        public string Identifier { get; private set; }
        public IGroum BodyGroum { get; private set; }
    }
}