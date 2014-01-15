namespace KaVE.Model.Groum
{
    public class WhileGroum : IGroum
    {
        public WhileGroum(IGroum conditionGroum, IGroum bodyGroum)
        {
            ConditionGroum = conditionGroum;
            BodyGroum = bodyGroum;
        }

        public IGroum ConditionGroum { get; private set; }
        public IGroum BodyGroum { get; private set; }
    }
}