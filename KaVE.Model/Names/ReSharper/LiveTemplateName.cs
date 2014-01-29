using KaVE.Model.Names.CSharp;
using KaVE.Model.Utils;

namespace KaVE.Model.Names.ReSharper
{
    public class LiveTemplateName : Name
    {
        private static readonly WeakNameCache<LiveTemplateName> Registry =
            WeakNameCache<LiveTemplateName>.Get(id => new LiveTemplateName(id));

        /// <summary>
        /// Template names follow the scheme "&lt;template name&gt;:&lt;template description&gt;".
        /// </summary>
        public new static LiveTemplateName Get(string identifier)
        {
            return Registry.GetOrCreate(identifier);
        }

        private LiveTemplateName(string identifier) : base(identifier) {}

        public string Name
        {
            get
            {
                var endOfName = Identifier.IndexOf(':');
                return Identifier.Substring(0, endOfName);
            }
        }

        public string Description
        {
            get
            {
                var startOfDescription = Identifier.IndexOf(':') + 1;
                return Identifier.Substring(startOfDescription);
            }
        }
    }
}