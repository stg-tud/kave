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

using System.Collections.Generic;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Impl.v0.CodeElements;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Model.Naming.Types.Organization;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.Naming.Impl.v0.Types
{
    public class DelegateTypeName : BaseTypeName, IDelegateTypeName
    {
        private const string UnknownDelegateIdentifier = "d:[?] [?].()";

        public DelegateTypeName() : this(UnknownDelegateIdentifier) {}

        public DelegateTypeName(string identifier) : base(identifier) {}


        public override bool IsUnknown
        {
            get { return UnknownDelegateIdentifier.Equals(Identifier); }
        }

        public override string Name
        {
            get { return DelegateType.Name; }
        }

        public override string FullName
        {
            get { return DelegateType.FullName; }
        }

        private IMethodName DelegateMethod
        {
            get { return new MethodName(Identifier.Substring(PrefixDelegate.Length)); }
        }

        public ITypeName DelegateType
        {
            get { return DelegateMethod.DeclaringType; }
        }

        public override bool IsNestedType
        {
            get { return DelegateType.IsNestedType; }
        }

        public override ITypeName DeclaringType
        {
            get { return DelegateType.DeclaringType; }
        }

        public override INamespaceName Namespace
        {
            get { return DelegateType.Namespace; }
        }

        public override IAssemblyName Assembly
        {
            get { return DelegateType.Assembly; }
        }

        public bool HasParameters
        {
            get { return DelegateMethod.HasParameters; }
        }

        public IList<IParameterName> Parameters
        {
            get { return DelegateMethod.Parameters; }
        }

        public ITypeName ReturnType
        {
            get
            {
                var rt = DelegateMethod.ReturnType;

                // simple case
                if (!rt.Identifier.Contains(DelegateType.Identifier))
                {
                    return rt;
                }

                // recursive case
                var nrtId = rt.Identifier.Replace(DelegateType.Identifier, Identifier);
                return TypeUtils.CreateTypeName(nrtId);
            }
        }

        public static bool IsDelegateTypeNameIdentifier([NotNull] string identifier)
        {
            return !TypeUtils.IsUnknownTypeIdentifier(identifier) && identifier.StartsWith(PrefixDelegate) &&
                   !ArrayTypeName.IsArrayTypeNameIdentifier(identifier);
        }
    }
}