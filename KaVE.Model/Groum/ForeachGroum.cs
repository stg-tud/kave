namespace KaVE.Model.Groum
{
    public class ForeachGroum : GroumBase
    {
        public ForeachGroum(GroumBase iterable, GroumBase body)
        {
            IterableGroum = iterable;
            BodyGroum = body;
        }

        public GroumBase IterableGroum { get; private set; }
        public GroumBase BodyGroum { get; private set; }
    }
}