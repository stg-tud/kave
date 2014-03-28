namespace KaVE.Model.Groum
{
    public class UsingGroum : GroumBase
    {
        public UsingGroum(GroumBase initializerName, string identifier, GroumBase bodyGroum)
        {
            InitializerGroum = initializerName;
            Identifier = identifier;
            BodyGroum = bodyGroum;
        }

        public GroumBase InitializerGroum { get; private set; }
        public string Identifier { get; private set; }
        public GroumBase BodyGroum { get; private set; }
    }
}