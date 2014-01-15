using System;

namespace KaVE.Model.Names.CSharp
{
    public abstract class MemberName : Name, IMemberName
    {
        public const string StaticModifier = "static";

        protected MemberName(string identifier) : base(identifier) {}

        public string Modifiers
        {
            get { return Identifier.Substring(0, Identifier.IndexOf('[')); }
        }

        public ITypeName DeclaringType
        {
            get
            {
                var startIndexOfDeclaringTypeIdentifier = Identifier.IndexOf("] [", StringComparison.Ordinal) + 3;
                var endIndexOfDeclaringTypeIdentifier = Identifier.IndexOf("].", StringComparison.Ordinal);
                var lengthOfDeclaringTypeIdentifier = endIndexOfDeclaringTypeIdentifier - startIndexOfDeclaringTypeIdentifier;
                return TypeName.Get(Identifier.Substring(startIndexOfDeclaringTypeIdentifier, lengthOfDeclaringTypeIdentifier));
            }
        }

        public bool IsStatic
        {
            get { return Modifiers.Contains(StaticModifier); }
        }

        public abstract string Name { get; }

        public ITypeName ValueType
        {
            get
            {
                var startIndexOfValueTypeIdentifier = Identifier.IndexOf('[') + 1;
                var lastIndexOfValueTypeIdentifer = Identifier.IndexOf("] [", StringComparison.Ordinal);
                var lengthOfValueTypeIdentifier = lastIndexOfValueTypeIdentifer - startIndexOfValueTypeIdentifier;
                return TypeName.Get(Identifier.Substring(startIndexOfValueTypeIdentifier, lengthOfValueTypeIdentifier));
            }
        }
    }
}
