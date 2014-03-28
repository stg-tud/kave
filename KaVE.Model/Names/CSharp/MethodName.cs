using System;
using System.Collections.Generic;
using KaVE.Model.Utils;

namespace KaVE.Model.Names.CSharp
{
    public class MethodName : MemberName, IMethodName
    {
        private static readonly WeakNameCache<MethodName> Registry = WeakNameCache<MethodName>.Get(id => new MethodName(id));

        /// <summary>
        /// Method type names follow the scheme <code>'modifiers' ['return type name'] ['declaring type name'].'method name'('parameter names')</code>.
        /// Examples of valid method names are:
        /// <list type="bullet">
        ///     <item><description><code>[System.Void, mscore, 4.0.0.0] [DeclaringType, AssemblyName, 1.2.3.4].MethodName()</code></description></item>
        ///     <item><description><code>static [System.String, mscore, 4.0.0.0] [MyType, MyAssembly, 1.0.0.0].StaticMethod()</code></description></item>
        ///     <item><description><code>[System.String, mscore, 4.0.0.0] [MyType, MyAssembly, 1.0.0.0].AMethod(opt [System.Int32, mscore, 4.0.0.0] length)</code></description></item>
        ///     <item><description><code>[System.String, mscore, 4.0.0.0] [System.String, mscore, 4.0.0.0]..ctor()</code> (Constructor)</description></item>
        /// </list>
        /// </summary>
        public new static MethodName Get(string identifier)
        {
            return Registry.GetOrCreate(identifier);
        }

        private MethodName(string identifier)
            : base(identifier)
        {
        }

        public override string Name
        {
            get
            {
                var endIndexOfMethodName = Identifier.IndexOf('(');
                var startIndexOfMethodName = Identifier.LastIndexOf("].", endIndexOfMethodName, StringComparison.Ordinal) + 2;
                var lengthOfMethodName = endIndexOfMethodName - startIndexOfMethodName;
                return Identifier.Substring(startIndexOfMethodName, lengthOfMethodName);
            }
        }

        public IList<ITypeName> TypeParameters
        {
            get
            {
                return new List<ITypeName>();
            }
        }

        public bool HasTypeParameters { get { return false; } }

        public bool IsGenericType
        {
            get { return false; }
        }

        public IList<IParameterName> Parameters
        {
            get
            {
                var parameters = new List<IParameterName>();
                if (HasParameters)
                {
                    var startOfParameterIdentifier = Identifier.IndexOf('(') + 1;
                    var endOfParameterList = Identifier.IndexOf(')');
                    do
                    {
                        var endOfParameterType = Identifier.IndexOf("] ", startOfParameterIdentifier, StringComparison.Ordinal);
                        var endOfParameterIdentifier = Identifier.IndexOf(',', endOfParameterType);
                        if (endOfParameterIdentifier < 0) endOfParameterIdentifier = endOfParameterList;
                        var lengthOfParameterIdentifier = endOfParameterIdentifier - startOfParameterIdentifier;
                        var identifier = Identifier.Substring(startOfParameterIdentifier, lengthOfParameterIdentifier).Trim();
                        parameters.Add(ParameterName.Get(identifier));
                        startOfParameterIdentifier = endOfParameterIdentifier + 1;
                    } while (startOfParameterIdentifier < endOfParameterList);
                }
                return parameters;
            }
        }

        public bool HasParameters { get { return !Identifier.Contains("()"); } }

        public bool IsConstructor
        {
            get
            {
                return Name.Equals(".ctor");
            }
        }

        public ITypeName ReturnType
        {
            get
            {
                return ValueType;
            }
        }
    }
}