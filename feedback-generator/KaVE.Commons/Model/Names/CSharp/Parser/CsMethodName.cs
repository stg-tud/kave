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
            get
            {
                return IsUnknown &&
                       (ctx.regularMethod().staticCctor() != null || ctx.regularMethod().nonStaticCtor() != null);
            }
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
            get
            {
                if (IsUnknown)
                {
                    return new CsTypeName(ctx.UNKNOWN().GetText());
                }
                else if (IsConstructor)
                {
                    return new CsTypeName(ctx.regularMethod().staticCctor() != null ? ctx.regularMethod().staticCctor().type() : ctx.regularMethod().nonStaticCtor().type());
                }
                return new CsTypeName(ctx.regularMethod().customMethod().type(1));
            }
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
            get { return ctx.UNKNOWN() != null; }
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
