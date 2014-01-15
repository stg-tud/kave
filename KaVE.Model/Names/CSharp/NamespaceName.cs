using KaVE.Model.Utils;

namespace KaVE.Model.Names.CSharp
{
    public class NamespaceName : Name, INamespaceName
    {
        private const string GlobaleNamespaceIdentifier = "";

        private static readonly WeakNameCache<NamespaceName> Registry = WeakNameCache<NamespaceName>.Get(id => new NamespaceName(id));

        public static readonly INamespaceName GlobalNamespace = Get(GlobaleNamespaceIdentifier);

        /// <summary>
        /// Namespace names follow the scheme <code>'parent namespace name'.'namespace name'</code>. An exception is the global namespace, which has the empty string as its identfier.
        /// Examples of namespace names are:
        /// <list type="bullet">
        ///     <item><description><code>System</code></description></item>
        ///     <item><description><code>CodeCompletion.Model.Names.CSharp</code></description></item>
        /// </list>
        /// </summary>
        public new static NamespaceName Get(string identifier)
        {
            return Registry.GetOrCreate(identifier);
        }

        private NamespaceName(string identifier)
            : base(identifier)
        {
        }

        public INamespaceName ParentNamespace
        {
            get
            {
                if (IsGlobalNamespace)
                {
                    return null;
                }
                var lastSeperatorIndex = Identifier.LastIndexOf('.');
                return lastSeperatorIndex == -1
                    ? GlobalNamespace
                    : Get(Identifier.Substring(0, lastSeperatorIndex));
            }
        }

        public string Name
        {
            get
            {
                var lastSeperatorIndex = Identifier.LastIndexOf('.');
                return Identifier.Substring(lastSeperatorIndex + 1);
            }
        }

        public bool IsGlobalNamespace
        {
            get { return Identifier.Equals(GlobaleNamespaceIdentifier); }
        }
    }
}
