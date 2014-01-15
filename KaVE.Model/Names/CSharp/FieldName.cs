using System;
using KaVE.Model.Utils;

namespace KaVE.Model.Names.CSharp
{
    public class FieldName : MemberName, IFieldName
    {
        private static readonly WeakNameCache<FieldName> Registry = WeakNameCache<FieldName>.Get(id => new FieldName(id));

        /// <summary>
        /// Field names follow the scheme <code>'modifiers' ['value type name'] ['declaring type name'].'field name'</code>.
        /// Examples of field names are:
        /// <list type="buller">
        ///     <item><description><code>[System.Int32, mscore, Version=4.0.0.0] [Collections.IList, mscore, Version=4.0.0.0]._count</code></description></item>
        ///     <item><description><code>static [System.Int32, mscore, Version=4.0.0.0] [MyClass, MyAssembly, Version=1.2.3.4].Constant</code></description></item>
        /// </list>
        /// </summary>
        public new static FieldName Get(string identifier)
        {
            return Registry.GetOrCreate(identifier);
        }

        private FieldName(string identifier) : base(identifier) { }

        public override string Name { get { return Identifier.Substring(Identifier.IndexOf("].", StringComparison.Ordinal) + 2); } }
    }
}
