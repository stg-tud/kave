namespace KaVE.Model.Groum
{
    public class SequentialGroum : GroumBase
    {
        public SequentialGroum(GroumBase firstGroum, GroumBase secondGroum)
        {
            FirstGroum = firstGroum;
            SecondGroum = secondGroum;
        }

        public GroumBase FirstGroum { get; private set; }
        public GroumBase SecondGroum { get; private set; }
    }
}