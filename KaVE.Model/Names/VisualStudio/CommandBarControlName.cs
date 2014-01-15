using KaVE.Model.Names.CSharp;
using KaVE.Model.Utils;

namespace KaVE.Model.Names.VisualStudio
{
    public class CommandBarControlName : Name, IIDEComponentName
    {
        public const char HierarchySeperator = '|';

        private static readonly WeakNameCache<CommandBarControlName> Registry =
            WeakNameCache<CommandBarControlName>.Get(id => new CommandBarControlName(id));

        public new static CommandBarControlName Get(string identifier)
        {
            return Registry.GetOrCreate(identifier);
        }

        private CommandBarControlName(string identifier)
            : base(identifier) {}

        public CommandBarControlName Parent
        {
            get
            {
                var endOfParentIdentifier = Identifier.LastIndexOf(HierarchySeperator);
                return endOfParentIdentifier < 0 ? null : Get(Identifier.Substring(0, endOfParentIdentifier));
            }
        }

        public string Name
        {
            get
            {
                var startOfName = Identifier.LastIndexOf(HierarchySeperator) + 1;
                return Identifier.Substring(startOfName);
            }
        }
    }
}
