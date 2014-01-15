namespace KaVE.Model.Groum
{
    public class ParallelGroum : IGroum
    {
        public ParallelGroum(params IGroum[] subGroums)
        {
            SubGroums = subGroums;
        }

        public IGroum[] SubGroums { get; private set; }
    }
}