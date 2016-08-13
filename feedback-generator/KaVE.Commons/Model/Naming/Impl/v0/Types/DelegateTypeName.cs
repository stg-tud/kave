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
using System.Linq;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Impl.v0.CodeElements;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Model.Naming.Types.Organization;
using KaVE.Commons.Utils.Collections;
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

        private IMethodName _delegateMethod;

        private IMethodName DelegateMethod
        {
            get
            {
                return _delegateMethod ?? (_delegateMethod = new MethodName(Identifier.Substring(PrefixDelegate.Length)));
            }
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

        public bool IsRecursive
        {
            get
            {
                if (DelegateType.IsUnknown)
                {
                    return false;
                }
                var hasRecursiveReturn = DelegateMethod.ReturnType.Identifier.Contains(DelegateType.Identifier);
                var hasRecursiveParam =
                    DelegateMethod.Parameters.Any(p => p.Identifier.Contains(DelegateType.Identifier));
                return hasRecursiveReturn || hasRecursiveParam;
            }
        }

        public IList<IParameterName> Parameters
        {
            get
            {
                var ps = Lists.NewList<IParameterName>();
                foreach (var p in DelegateMethod.Parameters)
                {
                    var isRecursive = !p.ValueType.IsUnknown && p.Identifier.Contains(DelegateType.Identifier);
                    if (isRecursive)
                    {
                        var id = p.Identifier.Replace(DelegateType.Identifier, Identifier);
                        ps.Add(new ParameterName(id));
                    }
                    else
                    {
                        ps.Add(p);
                    }
                }
                return ps;
            }
        }

        public override IKaVEList<ITypeParameterName> TypeParameters
        {
            get { return DelegateType.TypeParameters; }
        }

        public ITypeName ReturnType
        {
            get
            {
                var rt = DelegateMethod.ReturnType;

                // simple case
                if (rt.IsUnknown || !rt.Identifier.Contains(DelegateType.Identifier))
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
            if (TypeUtils.IsUnknownTypeIdentifier(identifier))
            {
                return false;
            }
            var startsWithD = identifier.StartsWith(PrefixDelegate);
            var isArrayTypeNameIdentifier = ArrayTypeName.IsArrayTypeNameIdentifier(identifier);
            return startsWithD && !isArrayTypeNameIdentifier;
        }
    }
}