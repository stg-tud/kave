namespace KaVE.Model.Groum
{
    public class TryCatchGroum : GroumBase
    {
        public TryCatchGroum(GroumBase tryGroum, GroumBase finallyGroum, params GroumBase[] catchGroums)
        {
            TryGroum = tryGroum;
            CatchGroums = new ParallelGroum(catchGroums);
            FinallyGroum = finallyGroum;
        }

        public GroumBase TryGroum { get; private set; }
        public ParallelGroum CatchGroums { get; private set; }
        public GroumBase FinallyGroum { get; private set; }
    }
}