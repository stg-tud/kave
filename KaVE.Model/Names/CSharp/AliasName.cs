using KaVE.Model.Utils;

namespace KaVE.Model.Names.CSharp
{
    /// <summary>
    /// Aliases are defined by using statements, like "using alias = Some.Reference;". A special case is the alias
    /// "global" that represents the global namespace by convention.
    /// </summary>
    public class AliasName : Name
    {
        private static readonly WeakNameCache<AliasName> Registry = WeakNameCache<AliasName>.Get(
            id => new AliasName(id));

        /// <summary>
        /// Alias names are valid C# identifiers that are not keywords, plus the special alias 'global'.
        /// </summary>
        public new static AliasName Get(string identifier)
        {
            return Registry.GetOrCreate(identifier);
        }

        private AliasName(string identifier) : base(identifier) {}
    }
}