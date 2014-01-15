using KaVE.Model.Names.CSharp;
using KaVE.Model.Utils;

namespace KaVE.Model.Names.VisualStudio
{
    public class ProjectName : Name, IIDEComponentName
    {
        private static readonly WeakNameCache<ProjectName> Registry =
            WeakNameCache<ProjectName>.Get(id => new ProjectName(id));

        public new static ProjectName Get(string identifier)
        {
            return Registry.GetOrCreate(identifier);
        }

        private ProjectName(string identifier)
            : base(identifier)
        {
        }
    }
}