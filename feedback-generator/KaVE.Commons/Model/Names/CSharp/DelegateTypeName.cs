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
        internal static bool IsDelegateTypeIdentifier(string identifier)
        {
            return identifier.StartsWith("d:");
        }

        [UsedImplicitly]
        internal new static ITypeName Get(string identifier)
        {
            return TypeName.Get(identifier);
        }

        internal DelegateTypeName(string identifier) : base(identifier) {}

        public override bool IsDelegateType
        {
            get { return true; }
        }

        public override string FullName
        {
            get
            {
                var baseFullName = base.FullName;
                var indexOfParameterList = baseFullName.IndexOf('(');
                if (indexOfParameterList > 0)
                {
                    return baseFullName.Substring(0, indexOfParameterList);
                }
                return baseFullName;
            }
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

        public ITypeName ReturnType
        {
            get { throw new NotImplementedException(); }
        }
    }
}