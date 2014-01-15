namespace KaVE.Model.Groum
{
    public class TryCatchGroum : IGroum
    {
        public TryCatchGroum(IGroum tryGroum, IGroum finallyGroum, params IGroum[] catchGroums)
        {
            TryGroum = tryGroum;
            CatchGroums = new ParallelGroum(catchGroums);
            FinallyGroum = finallyGroum;
        }

        public IGroum TryGroum { get; private set; }
        public ParallelGroum CatchGroums { get; private set; }
        public IGroum FinallyGroum { get; private set; }
    }
}