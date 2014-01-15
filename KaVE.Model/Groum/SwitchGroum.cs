namespace KaVE.Model.Groum
{
    public class SwitchGroum : IGroum
    {
        public SwitchGroum(IGroum conditionGroum, params IGroum[] caseGroums)
        {
            ConditionGroum = conditionGroum;
            CaseGroums = new ParallelGroum(caseGroums);
        }

        public IGroum ConditionGroum { get; private set; }
        public ParallelGroum CaseGroums { get; private set; }
    }
}