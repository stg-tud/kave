namespace KaVE.Model.Groum
{
    public class SwitchGroum : GroumBase
    {
        public SwitchGroum(GroumBase conditionGroum, params GroumBase[] caseGroums)
        {
            ConditionGroum = conditionGroum;
            CaseGroums = new ParallelGroum(caseGroums);
        }

        public GroumBase ConditionGroum { get; private set; }
        public ParallelGroum CaseGroums { get; private set; }
    }
}