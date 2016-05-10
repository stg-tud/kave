/**
 * Copyright 2016 Technische Universität Darmstadt
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KaVE.Commons.Utils.Collections;

namespace KaVE.Commons.Model.Names.CSharp.Parser
{
    class CsTypeName : ITypeName
    {
    private static string UNKNOWN_IDENTIFIER = "???";
	protected TypeNamingParser.TypeContext ctx;

	public CsTypeName(String type) {
		TypeNamingParser.TypeContext ctx = TypeNameParseUtil.ValidateTypeName(type);
		this.ctx = ctx;
	}

	public CsTypeName(TypeNamingParser.TypeContext ctx) {
		this.ctx = ctx;
	}
	
    public IAssemblyName Assembly
    {
        get
        {
            String identifier = "???";
            if (ctx.regularType() != null)
            {
                identifier = ctx.regularType().assembly().GetText();
            }
            else if (ctx.arrayType() != null)
            {
                return new CsTypeName(ctx.arrayType().type()).Assembly;
            }
            else if (ctx.delegateType() != null)
            {
                return new CsMethodName(ctx.delegateType().method()).DeclaringType.Assembly;
            }
            return AssemblyName.Get(identifier);
        }
    }

    public INamespaceName Namespace
    {
        get { throw new NotImplementedException(); }
    }

    public ITypeName DeclaringType
    {
        get { throw new NotImplementedException(); }
    }

    public string FullName
    {
        get { throw new NotImplementedException(); }
    }

    public string Name
    {
        get { throw new NotImplementedException(); }
    }

    bool ITypeName.IsUnknownType
    {
        get { throw new NotImplementedException(); }
    }

    bool ITypeName.IsVoidType
    {
        get { throw new NotImplementedException(); }
    }

    bool ITypeName.IsValueType
    {
        get { throw new NotImplementedException(); }
    }

    bool ITypeName.IsSimpleType
    {
        get { throw new NotImplementedException(); }
    }

    bool ITypeName.IsEnumType
    {
        get { throw new NotImplementedException(); }
    }

    bool ITypeName.IsStructType
    {
        get { throw new NotImplementedException(); }
    }

    bool ITypeName.IsNullableType
    {
        get { throw new NotImplementedException(); }
    }

    bool ITypeName.IsReferenceType
    {
        get { throw new NotImplementedException(); }
    }

    bool ITypeName.IsClassType
    {
        get { throw new NotImplementedException(); }
    }

    bool ITypeName.IsInterfaceType
    {
        get { throw new NotImplementedException(); }
    }

    bool ITypeName.IsDelegateType
    {
        get { throw new NotImplementedException(); }
    }

    bool ITypeName.IsNestedType
    {
        get { throw new NotImplementedException(); }
    }

    bool ITypeName.IsArrayType
    {
        get { throw new NotImplementedException(); }
    }

    public ITypeName ArrayBaseType
    {
        get { throw new NotImplementedException(); }
    }

    bool ITypeName.IsTypeParameter
    {
        get { throw new NotImplementedException(); }
    }

    public string TypeParameterShortName
    {
        get { throw new NotImplementedException(); }
    }

    public ITypeName TypeParameterType
    {
        get { throw new NotImplementedException(); }
    }

    bool IGenericName.IsGenericEntity
    {
        get { throw new NotImplementedException(); }
    }

    bool IGenericName.HasTypeParameters
    {
        get { throw new NotImplementedException(); }
    }

    public IList<ITypeName> TypeParameters
    {
        get { throw new NotImplementedException(); }
    }

    public string Identifier
    {
        get { return ctx.GetText(); }
    }

    bool IName.IsUnknown
    {
        get { throw new NotImplementedException(); }
    }

    public bool IsHashed
    {
        get { throw new NotImplementedException(); }
    }
    }
}