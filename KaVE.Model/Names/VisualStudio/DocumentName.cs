using KaVE.Model.Names.CSharp;
using KaVE.Model.Utils;

namespace KaVE.Model.Names.VisualStudio
{
    public class DocumentName : Name, IIDEComponentName
    {
        private static readonly WeakNameCache<DocumentName> Registry = WeakNameCache<DocumentName>.Get(id => new DocumentName(id));

        public new static DocumentName Get(string identifier)
        {
            return Registry.GetOrCreate(identifier);
        }

        private DocumentName(string identifier)
            : base(identifier)
        {
        }
    }
}