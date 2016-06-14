/*
 * Copyright 2014 Technische Universität Darmstadt
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using KaVE.Commons.Model.Names.CSharp.Parser;

namespace KaVE.Commons.Model.Names.CSharp
{
    public class CsParameterName : IName, IParameterName
    {
        private TypeNamingParser.FormalParamContext ctx;

        public CsParameterName(TypeNamingParser.FormalParamContext ctx)
        {
            this.ctx = ctx;
        }

        public string Identifier
        {
            get { return ctx.GetText(); }
        }

        public bool IsUnknown
        {
            get
            {
                return ctx.type().UNKNOWN() == null;
            }
        }

        public bool IsHashed { get { return Identifier.Contains("=="); } }

        public ITypeName ValueType
        {
            get
            {
                return new CsTypeName(ctx.type());
            }
        }

        public string Name
        {
            get { return ctx.id().GetText(); }
        }

        public bool IsPassedByReference
        {
            get { return ctx.parameterModifier() != null && ctx.parameterModifier().refModifier() != null; }
        }

        public bool IsOutput
        {
            get { return ctx.parameterModifier() != null && ctx.parameterModifier().outModifier() != null; }
        }

        public bool IsParameterArray { get { return ctx.parameterModifier() != null && ctx.parameterModifier().paramsModifier() != null; } }
        public bool IsOptional { get { return ctx.parameterModifier() != null && ctx.parameterModifier().optsModifier() != null; } }
        public bool IsExtensionMethodParameter { get { return ctx.parameterModifier() != null && ctx.parameterModifier().extensionModifier() != null; } }
    }
}