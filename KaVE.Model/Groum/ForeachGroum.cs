namespace KaVE.Model.Groum
{
    public class ForeachGroum : IGroum
    {
        public ForeachGroum(IGroum iterable, IGroum body)
        {
            IterableGroum = iterable;
            BodyGroum = body;
        }

        public IGroum IterableGroum { get; private set; }
        public IGroum BodyGroum { get; private set; }
    }
}