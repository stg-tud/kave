namespace KaVE.Model.Groum
{
    public class ReferenceGroum : IGroum
    {
        public ReferenceGroum(string identifier)
        {
            Identifier = identifier;
        }

        public string Identifier { get; private set; }
    }
}