namespace KaVE.Model.Groum
{
    public class ForGroum : GroumBase
    {
        public ForGroum(GroumBase initializerGroum, GroumBase conditionGroum, GroumBase bodyGroum, GroumBase updaterGroum)
        {
            InitializerGroum = initializerGroum;
            ConditionGroum = conditionGroum;
            BodyGroum = bodyGroum;
            UpdaterGroum = updaterGroum;
        }

        public GroumBase InitializerGroum { get; private set; }
        public GroumBase ConditionGroum { get; private set; }
        public GroumBase BodyGroum { get; private set; }
        public GroumBase UpdaterGroum { get; private set; }
    }
}