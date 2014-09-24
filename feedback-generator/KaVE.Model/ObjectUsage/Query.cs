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
 *    - Dennis Albrecht
 *    - Uli Fahrer
 */

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Collections;
using KaVE.Utils;
using KaVE.Utils.Assertion;

namespace KaVE.Model.ObjectUsage
{
    // ReSharper disable InconsistentNaming
    public class Query
    {
        public Query()
        {
            sites = Lists.NewList<CallSite>();
        }

        public Query(IEnumerable<CallSite> callSites)
        {
            sites = Lists.NewList(callSites.ToArray());
        }

        [NotNull]
        public CoReTypeName type { get; set; }

        [NotNull]
        public CoReTypeName classCtx { get; set; }

        [NotNull]
        public CoReMethodName methodCtx { get; set; }

        [NotNull]
        public DefinitionSite definition { get; set; }

        [NotNull]
        public IList<CallSite> sites { get; private set; }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        private bool Equals(Query oth)
        {
            return Equals(type, oth.type) && Equals(definition, oth.definition) && Equals(classCtx, oth.classCtx) &&
                   Equals(methodCtx, oth.methodCtx) && Equals(sites, oth.sites);
        }

        public override int GetHashCode()
        {
            var hashCode = 397;
            hashCode = (hashCode*397) ^ type.GetHashCode();
            hashCode = (hashCode*397) ^ definition.GetHashCode();
            hashCode = (hashCode*397) ^ classCtx.GetHashCode();
            hashCode = (hashCode*397) ^ methodCtx.GetHashCode();
            hashCode = (hashCode*397) ^ sites.GetHashCode();
            return hashCode;
        }
    }

    public class DefinitionSite
    {
        public DefinitionSiteKind kind { get; set; }

        [NotNull]
        public CoReFieldName field { get; set; }

        [NotNull]
        public CoReMethodName method { get; set; }

        public int argIndex { get; set; }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        private bool Equals(DefinitionSite oth)
        {
            return kind.Equals(oth.kind) && argIndex == oth.argIndex && Equals(method, oth.method) &&
                   Equals(field, oth.field);
        }

        public override int GetHashCode()
        {
            var hashCode = 397;
            hashCode = (hashCode*397) ^ kind.GetHashCode();
            hashCode = (hashCode*397) ^ method.GetHashCode();
            hashCode = (hashCode*397) ^ field.GetHashCode();
            hashCode = (hashCode*397) ^ argIndex.GetHashCode();
            return hashCode;
        }
    }

    public enum DefinitionSiteKind
    {
        THIS,
        RETURN,
        NEW,
        PARAM,
        FIELD,
        CONSTANT,
        UNKNOWN,
    }

    public class CallSite
    {
        public CallSiteKind kind { get; set; }

        [NotNull]
        public CoReMethodName method { get; set; }

        public int argIndex { get; set; }

        /// <summary>
        ///     This method is used by the serialization to determine if the property should be serialized or not.
        ///     As the method's name depends directly on the property's name, the method must not be renamed.
        /// </summary>
        public bool ShouldSerializeargIndex()
        {
            return argIndex != 0;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        private bool Equals(CallSite oth)
        {
            return kind.Equals(oth.kind) && argIndex == oth.argIndex && Equals(method, oth.method);
        }

        public override int GetHashCode()
        {
            var hashCode = 397;
            hashCode = (hashCode*397) ^ kind.GetHashCode();
            hashCode = (hashCode*397) ^ method.GetHashCode();
            hashCode = (hashCode*397) ^ argIndex.GetHashCode();
            return hashCode;
        }
    }

    public enum CallSiteKind
    {
        RECEIVER,
        PARAMETER,
    }

    // ReSharper restore InconsistentNaming

    public abstract class CoReName
    {
        protected CoReName(string name, string validationPattern)
        {
            var regex = new Regex("^" + validationPattern + "$");
            Asserts.That(regex.IsMatch(name));
            Name = name;
        }

        public string Name { get; private set; }

        public override bool Equals(object obj)
        {
            var name = obj as CoReName;
            return name != null && Name.Equals(name.Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override string ToString()
        {
            return GetType().Name + "[" + Name + "]";
        }
    }

    public class CoReTypeName : CoReName
    {
        public CoReTypeName(string name) : base(name, ValidationPattern()) {}

        internal static string ValidationPattern()
        {
            return @"\[*L([a-zA-Z0-9]+/)*[a-zA-Z0-9$]+";
        }
    }

    public class CoReMethodName : CoReName
    {
        public CoReMethodName(string name) : base(name, ValidationPattern()) {}

        public string Method
        {
            get { return Name.Split('.', '(')[1]; }
        }

        private static string ValidationPattern()
        {
            return string.Format(@"{0}\.[a-zA-Z0-9]+\(({0};)*\){0};", CoReTypeName.ValidationPattern());
        }
    }

    public class CoReFieldName : CoReName
    {
        public CoReFieldName(string name) : base(name, ValidationPattern()) {}

        private static string ValidationPattern()
        {
            return string.Format(@"{0}\.[a-zA-Z0-9]+;{0}", CoReTypeName.ValidationPattern());
        }
    }
}