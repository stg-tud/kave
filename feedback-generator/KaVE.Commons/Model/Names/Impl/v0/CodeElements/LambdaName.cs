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

using System;
using System.Collections.Generic;
using KaVE.Commons.Utils.Collections;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.Names.CSharp
{
    public class LambdaName : Name, ILambdaName
    {
        private static readonly WeakNameCache<LambdaName> Registry = WeakNameCache<LambdaName>.Get(
            id => new LambdaName(id));

        public new static LambdaName UnknownName
        {
            get { return Get(UnknownNameIdentifier); }
        }

        /// <summary>
        ///     Lambda type names follow the scheme
        ///     <code>['return type name'] ('parameter names')</code>.
        ///     Examples of valid lambda names are:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <code>[System.String, mscore, 4.0.0.0] ()</code>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <code>[System.String, mscore, 4.0.0.0] ([System.Int32, mscore, 4.0.0.0] length)</code>
        ///             </description>
        ///         </item>
        ///     </list>
        /// </summary>
        [NotNull]
        public new static LambdaName Get(string identifier)
        {
            return Registry.GetOrCreate(identifier);
        }

        [NotNull]
        public static ILambdaName Get(string identifier, params object[] args)
        {
            return Get(string.Format(identifier, args));
        }

        private LambdaName(string identifier) : base(identifier) {}

        public string Signature
        {
            get
            {
                var startOfSignature = Identifier.IndexOf('(');
                return Identifier.Substring(startOfSignature);
            }
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
            get
            {
                var startIndexOfValueTypeIdentifier = Identifier.IndexOf('[') + 1;
                var lastIndexOfValueTypeIdentifer = Identifier.IndexOf("]", StringComparison.Ordinal);
                var lengthOfValueTypeIdentifier = lastIndexOfValueTypeIdentifer - startIndexOfValueTypeIdentifier;
                return TypeName.Get(Identifier.Substring(startIndexOfValueTypeIdentifier, lengthOfValueTypeIdentifier));
            }
        }
    }
}