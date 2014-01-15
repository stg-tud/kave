using KaVE.Model.Names.CSharp;
using KaVE.Model.Utils;

namespace KaVE.Model.Names.VisualStudio
{
    public class SolutionName : Name, IIDEComponentName
    {
        private static readonly WeakNameCache<SolutionName> Registry = WeakNameCache<SolutionName>.Get(id => new SolutionName(id));

        public new static SolutionName Get(string identifier)
        {
            return Registry.GetOrCreate(identifier);
        }

        private SolutionName(string identifier)
            : base(identifier)
        {
        }
    }
}