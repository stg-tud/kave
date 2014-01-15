namespace KaVE.Model.Groum
{
    public class DoWhileGroum : IGroum
    {
        public DoWhileGroum(IGroum bodyGroum, IGroum conditionGroum)
        {
            BodyGroum = bodyGroum;
            ConditionGroum = conditionGroum;
        }

        public IGroum BodyGroum { get; private set; }
        public IGroum ConditionGroum { get; private set; }
    }
}