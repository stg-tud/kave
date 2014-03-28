namespace KaVE.Model.Groum
{
    public class ParallelGroum : GroumBase
    {
        public ParallelGroum(params GroumBase[] subGroums)
        {
            SubGroums = subGroums;
        }

        public GroumBase[] SubGroums { get; private set; }
    }
}