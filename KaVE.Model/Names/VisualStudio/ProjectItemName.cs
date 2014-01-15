using KaVE.Model.Names.CSharp;
using KaVE.Model.Utils;

namespace KaVE.Model.Names.VisualStudio
{
    public class ProjectItemName : Name, IIDEComponentName
    {
        private static readonly WeakNameCache<ProjectItemName> Registry =
            WeakNameCache<ProjectItemName>.Get(id => new ProjectItemName(id));

        public new static ProjectItemName Get(string identifier)
        {
            return Registry.GetOrCreate(identifier);
        }

        private ProjectItemName(string identifier) : base(identifier)
        {
        }
    }
}