using KaVE.Model.Names.CSharp;
using KaVE.Model.Utils;

namespace KaVE.Model.Names.VisualStudio
{
    public class WindowName : Name, IIDEComponentName
    {
        private static readonly WeakNameCache<WindowName> Registry = WeakNameCache<WindowName>.Get(id => new WindowName(id));

        public new static WindowName Get(string identifier)
        {
            return Registry.GetOrCreate(identifier);
        }

        private WindowName(string identifier)
            : base(identifier)
        {
        }

        public string Type
        {
            get
            {
                return Identifier.Substring(0, Identifier.IndexOf(' '));
            }
        }

        public string Caption
        {
            get
            {
                var startOfWindowCaption = Type.Length + 1;
                return Identifier.Substring(startOfWindowCaption);
            }
        }
    }
}
