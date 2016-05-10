using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaVE.Commons.Model.Names.CSharp.Parser
{
    class CsMethodName : IMethodName
    {
        protected TypeNamingParser.MethodContext ctx;

        public CsMethodName(TypeNamingParser.MethodContext ctx)
        {
            this.ctx = ctx;
        }

        public CsMethodName(string input)
        {
            this.ctx = TypeNameParseUtil.ValidateMethodName(input);
        }

        public string Signature
        {
            get { throw new NotImplementedException(); }
        }

        public IList<IParameterName> Parameters
        {
            get { throw new NotImplementedException(); }
        }

        public bool HasParameters
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsConstructor
        {
            get { throw new NotImplementedException(); }
        }

        public ITypeName ReturnType
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsExtensionMethod
        {
            get { throw new NotImplementedException(); }
        }

        public ITypeName DeclaringType
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsStatic
        {
            get { throw new NotImplementedException(); }
        }

        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        public string Identifier
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsUnknown
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsHashed
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsGenericEntity
        {
            get { throw new NotImplementedException(); }
        }

        public bool HasTypeParameters
        {
            get { throw new NotImplementedException(); }
        }

        public IList<ITypeName> TypeParameters
        {
            get { throw new NotImplementedException(); }
        }
    }
}
