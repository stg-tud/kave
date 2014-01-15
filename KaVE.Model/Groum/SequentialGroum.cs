namespace KaVE.Model.Groum
{
    public class SequentialGroum : IGroum
    {
        public SequentialGroum(IGroum firstGroum, IGroum secondGroum)
        {
            FirstGroum = firstGroum;
            SecondGroum = secondGroum;
        }

        public IGroum FirstGroum { get; private set; }
        public IGroum SecondGroum { get; private set; }
    }
}