using KaVE.Model.Utils;

namespace KaVE.Model.Names.CSharp
{
    public class LocalVariableName : Name
    {
        private static readonly WeakNameCache<LocalVariableName> Registry = WeakNameCache<LocalVariableName>.Get(id => new LocalVariableName(id));

        /// <summary>
        /// Local variable names have the form '[value-type-identifier] variable-name'.
        /// </summary>
        public new static LocalVariableName Get(string identifier)
        {
            return Registry.GetOrCreate(identifier);
        }

        private LocalVariableName(string identifier) : base(identifier) { }

        public string Name
        {
            get
            {
                var indexOfName = Identifier.LastIndexOf(']') + 2;
                return Identifier.Substring(indexOfName);
            }
        }

        public ITypeName ValueType
        {
            get
            {
                var lengthOfTypeIdentifier = Identifier.LastIndexOf(']') - 1;
                return TypeName.Get(Identifier.Substring(1, lengthOfTypeIdentifier));
            }
        }
    }
}
