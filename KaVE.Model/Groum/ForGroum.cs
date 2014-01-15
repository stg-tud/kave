namespace KaVE.Model.Groum
{
    public class ForGroum : IGroum
    {
        public ForGroum(IGroum initializerGroum, IGroum conditionGroum, IGroum bodyGroum, IGroum updaterGroum)
        {
            InitializerGroum = initializerGroum;
            ConditionGroum = conditionGroum;
            BodyGroum = bodyGroum;
            UpdaterGroum = updaterGroum;
        }

        public IGroum InitializerGroum { get; private set; }
        public IGroum ConditionGroum { get; private set; }
        public IGroum BodyGroum { get; private set; }
        public IGroum UpdaterGroum { get; private set; }
    }
}