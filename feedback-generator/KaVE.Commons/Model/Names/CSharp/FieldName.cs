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
using KaVE.Commons.Utils.Collections;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.Names.CSharp
{
    public class FieldName : MemberName, IFieldName
    {
        private static readonly WeakNameCache<FieldName> Registry = WeakNameCache<FieldName>.Get(
            id => new FieldName(id));

        public new static IFieldName UnknownName
        {
            get { return Get("[?] [?].???"); }
        }

        public override bool IsUnknown
        {
            get { return Equals(this, UnknownName); }
        }

        /// <summary>
        ///     Field names follow the scheme <code>'modifiers' ['value type name'] ['declaring type name'].'field name'</code>.
        ///     Examples of field names are:
        ///     <list type="buller">
        ///         <item>
        ///             <description>
        ///                 <code>[System.Int32, mscore, 4.0.0.0] [Collections.IList, mscore, 4.0.0.0]._count</code>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <code>static [System.Int32, mscore, 4.0.0.0] [MyClass, MyAssembly, 1.2.3.4].Constant</code>
        ///             </description>
        ///         </item>
        ///     </list>
        /// </summary>
        [NotNull]
        public new static FieldName Get(string identifier)
        {
            return Registry.GetOrCreate(identifier);
        }

        private FieldName(string identifier) : base(identifier) {}
    }
}