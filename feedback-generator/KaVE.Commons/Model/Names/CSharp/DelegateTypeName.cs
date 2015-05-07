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
 * 
 * Contributors:
 *    - Sven Amann
 */

using System;
using System.Collections.Generic;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.Names.CSharp
{
    public class DelegateTypeName : TypeName, IDelegateTypeName
    {
        private const string Prefix = "d:";

        internal static bool IsDelegateTypeIdentifier(string identifier)
        {
            return identifier.StartsWith(Prefix);
        }

        [UsedImplicitly]
        public new static IDelegateTypeName Get(string identifier)
        {
            // fix legacy delegate names without parameters
            if (!identifier.Contains("("))
            {
                var endOfTypeName = GetLengthOfTypeName(identifier);
                identifier = identifier.Insert(endOfTypeName, "()");
            }

            return (IDelegateTypeName) TypeName.Get(identifier);
        }

        internal DelegateTypeName(string identifier) : base(identifier) {}

        public override bool IsDelegateType
        {
            get { return true; }
        }

        private String FullNameWithParameters()
        {
            var endOfParameters = Identifier.LastIndexOf(')') + 1;
            return Identifier.Substring(Prefix.Length, endOfParameters - Prefix.Length);
        }

        public override string FullName
        {
            get
            {
                var fullNameWithParameters = FullNameWithParameters();
                var startOfParameterList = fullNameWithParameters.IndexOf('(');
                return fullNameWithParameters.Substring(0, startOfParameterList);
            }
        }

        public string Signature
        {
            get { return FullNameWithParameters().Substring(Namespace.Identifier.Length + 1); }
        }

        public IList<IParameterName> Parameters
        {
            get { return Identifier.GetParameterNames(); }
        }

        public bool HasParameters
        {
            get { return Identifier.HasParameters(); }
        }

        public ITypeName ReturnType
        {
            get { throw new NotImplementedException(); }
        }
    }
}