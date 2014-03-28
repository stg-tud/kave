namespace KaVE.Model.Groum
{
    public class DoWhileGroum : GroumBase
    {
        public DoWhileGroum(GroumBase bodyGroum, GroumBase conditionGroum)
        {
            BodyGroum = bodyGroum;
            ConditionGroum = conditionGroum;
        }

        public GroumBase BodyGroum { get; private set; }
        public GroumBase ConditionGroum { get; private set; }
    }
}