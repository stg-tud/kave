using KaVE.Model.Names.CSharp;
using KaVE.Model.Utils;

namespace KaVE.Model.Names.VisualStudio
{
    public class CommandName : Name, IIDEComponentName
    {
        private static readonly WeakNameCache<CommandName> Registry = WeakNameCache<CommandName>.Get(id => new CommandName(id));

        /// <summary>
        /// <code>{guid}:id:name</code>
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public new static CommandName Get(string identifier)
        {
            return Registry.GetOrCreate(identifier);
        }

        private CommandName(string identifier)
            : base(identifier)
        {
        }

        public string Guid
        {
            get
            {
                return Identifier.Substring(0, Identifier.IndexOf(':'));
            }
        }

        public int Id
        {
            get
            {
                var parts = Identifier.Split(':');
                return int.Parse(parts[1]);
            }
        }

        public string Name
        {
            get
            {
                var parts = Identifier.Split(':');
                return parts.Length == 3 ? parts[2] : null;
            }
        }
    }
}
