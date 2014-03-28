using KaVE.Model.Names;

namespace KaVE.Model.Groum
{
    public class FieldAccessGroum : GroumBase
    {
        public FieldAccessGroum(IFieldName accessedField)
        {
            AccessedField = accessedField;
        }

        public IFieldName AccessedField { get; private set; }
    }
}