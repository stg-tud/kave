using KaVE.Model.Utils;

namespace KaVE.Model.Names.CSharp
{
    public class PropertyName : MemberName, IPropertyName
    {
        public const string SetterModifier = "set";
        public const string GetterModifier = "get";

        private static readonly WeakNameCache<PropertyName> Registry =
            WeakNameCache<PropertyName>.Get(id => new PropertyName(id));

        /// <summary>
        /// Property names follow the scheme <code>'modifiers' ['value type name'] ['declaring type name'].'property name'</code>.
        /// Examples of property names are:
        /// <list type="buller">
        ///     <item><description><code>[System.Int32, mscore, Version=4.0.0.0] [Collections.IList, mscore, Version=4.0.0.0].Internal</code></description></item>
        ///     <item><description><code>get [System.Int32, mscore, Version=4.0.0.0] [MyClass, MyAssembly, Version=1.2.3.4].Count</code> (property with public getter)</description></item>
        ///     <item><description><code>set [System.Int32, mscore, Version=4.0.0.0] [MyClass, MyAssembly, Version=1.2.3.4].Position</code> (property with public setter)</description></item>
        /// </list>
        /// </summary>
        public new static PropertyName Get(string identifier)
        {
            return Registry.GetOrCreate(identifier);
        }

        private PropertyName(string identifier)
            : base(identifier) {}

        public override string Name
        {
            get { return Identifier.Substring(Identifier.IndexOf("].", System.StringComparison.Ordinal) + 2); }
        }

        public bool HasSetter
        {
            get { return Modifiers.Contains(SetterModifier); }
        }

        public bool HasGetter
        {
            get { return Modifiers.Contains(GetterModifier); }
        }

        public new bool IsStatic
        {
            get { return Modifiers.Contains(StaticModifier); }
        }
    }
}