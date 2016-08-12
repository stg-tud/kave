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

using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Utils.Collections;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.Naming.Impl.v0.CodeElements
{
    public class PropertyName : MemberName, IPropertyName
    {
        public const string SetterModifier = "set";
        public const string GetterModifier = "get";

        public PropertyName() : this(UnknownMemberIdentifier) {}

        public PropertyName([NotNull] string identifier) : base(identifier) {}

        public override bool IsUnknown
        {
            get { return Equals(Identifier, UnknownMemberIdentifier); }
        }

        public bool HasSetter
        {
            get { return Modifiers.Contains(SetterModifier); }
        }

        public bool HasGetter
        {
            get { return Modifiers.Contains(GetterModifier); }
        }

        public bool IsIndexer
        {
            get { return false; }
        }

        public bool HasParameters
        {
            get { return false; }
        }

        public IKaVEList<IParameterName> Parameters
        {
            get { return Lists.NewList<IParameterName>(); }
        }
    }
}