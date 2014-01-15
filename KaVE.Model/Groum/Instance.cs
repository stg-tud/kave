using KaVE.Model.Names;

namespace KaVE.Model.Groum
{
    public class Instance
    {
        public Instance(ITypeName type)
        {
            Type = type;
        }

        public ITypeName Type { get; private set; }
    }
}
