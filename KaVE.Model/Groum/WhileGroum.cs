namespace KaVE.Model.Groum
{
    public class WhileGroum : GroumBase
    {
        public WhileGroum(GroumBase conditionGroum, GroumBase bodyGroum)
        {
            ConditionGroum = conditionGroum;
            BodyGroum = bodyGroum;
        }

        public GroumBase ConditionGroum { get; private set; }
        public GroumBase BodyGroum { get; private set; }
    }
}