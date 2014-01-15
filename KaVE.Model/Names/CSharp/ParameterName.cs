using KaVE.Model.Utils;

namespace KaVE.Model.Names.CSharp
{
    public class ParameterName : Name, IParameterName
    {
        public const string PassByReferenceModifier = "ref";
        public const string OutputModifier = "out";
        public const string VarArgsModifier = "params";
        public const string OptionalModifier = "opt";

        private static readonly WeakNameCache<ParameterName> Registry = WeakNameCache<ParameterName>.Get(id => new ParameterName(id));

        /// <summary>
        /// Parameter names follow the scheme <code>'modifiers' ['parameter type name'] 'parameter name'</code>.
        /// Examples of parameter names are:
        /// <list type="bullet">
        ///     <item><description><code>[System.Int32, mscore, Version=4.0.0.0] size</code></description></item>
        ///     <item><description><code>out [System.Int32, mscore, Version=4.0.0.0] outputParameter</code></description></item>
        ///     <item><description><code>params [System.Int32, mscore, Version=4.0.0.0] varArgsParamter</code></description></item>
        ///     <item><description><code>ref [System.Int32, mscore, Version=4.0.0.0] referenceParameter</code></description></item>
        ///     <item><description><code>opt [System.Int32, mscore, Version=4.0.0.0] optionalParameter</code> (i.e., parameter with default value)</description></item>
        /// </list>
        /// </summary>
        public new static ParameterName Get(string identifier)
        {
            return Registry.GetOrCreate(identifier);
        }

        private ParameterName(string identifier)
            : base(identifier)
        {
        }

        public ITypeName ValueType
        {
            get
            {
                var startOfValueTypeIdentifier = Identifier.IndexOf('[') + 1;
                var endOfValueTypeIdentifier = Identifier.LastIndexOf(']');
                var lengthOfValueTypeIdentifier = endOfValueTypeIdentifier - startOfValueTypeIdentifier;
                return TypeName.Get(Identifier.Substring(startOfValueTypeIdentifier, lengthOfValueTypeIdentifier));
            }
        }

        public string Name
        {
            get { return Identifier.Substring(Identifier.LastIndexOf(' ') + 1); }
        }

        public bool IsPassedByReference
        {
            get { return ValueType.IsReferenceType || Modifiers.Contains(PassByReferenceModifier); }
        }

        private string Modifiers
        {
            get { return Identifier.Substring(0, Identifier.IndexOf('[')); }
        }

        public bool IsOutput
        {
            get { return Modifiers.Contains(OutputModifier); }
        }

        public bool IsParameterArray
        {
            get { return Modifiers.Contains(VarArgsModifier); }
        }

        public bool IsOptional
        {
            get { return Modifiers.Contains(OptionalModifier); }
        }
    }
}
