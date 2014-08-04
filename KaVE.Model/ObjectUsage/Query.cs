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
 */

using System.Collections.Generic;
using System.Text.RegularExpressions;
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

        public CoReTypeName type { get; set; }
        public DefinitionSite definition { get; set; }
        public CoReTypeName classCtx { get; set; }
        public CoReMethodName methodCtx { get; set; }
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
            hashCode = (hashCode*397) ^ (type != null ? type.GetHashCode() : 0);
            hashCode = (hashCode*397) ^ (definition != null ? definition.GetHashCode() : 0);
            hashCode = (hashCode*397) ^ (classCtx != null ? classCtx.GetHashCode() : 0);
            hashCode = (hashCode*397) ^ (methodCtx != null ? methodCtx.GetHashCode() : 0);
            hashCode = (hashCode*397) ^ (sites != null ? sites.GetHashCode() : 0);
            return hashCode;
        }
    }

    public class DefinitionSite
    {
        public DefinitionKind kind { get; set; }
        public CoReTypeName type { get; set; }
        public CoReMethodName method { get; set; }
        public CoReFieldName field { get; set; }
        public int arg { get; set; }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        private bool Equals(DefinitionSite oth)
        {
            return kind.Equals(oth.kind) && arg == oth.arg && Equals(type, oth.type) && Equals(method, oth.method) &&
                   Equals(field, oth.field);
        }

        public override int GetHashCode()
        {
            var hashCode = 397;
            hashCode = (hashCode*397) ^ (kind.GetHashCode());
            hashCode = (hashCode*397) ^ (type != null ? type.GetHashCode() : 0);
            hashCode = (hashCode*397) ^ (method != null ? method.GetHashCode() : 0);
            hashCode = (hashCode*397) ^ (field != null ? field.GetHashCode() : 0);
            hashCode = (hashCode*397) ^ (arg.GetHashCode());
            return hashCode;
        }
    }

    public enum DefinitionKind
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
        public CoReMethodName call { get; set; }
        public int argumentIndex { get; set; }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        private bool Equals(CallSite oth)
        {
            return kind.Equals(oth.kind) && argumentIndex == oth.argumentIndex && Equals(call, oth.call);
        }

        public override int GetHashCode()
        {
            var hashCode = 397;
            hashCode = (hashCode*397) ^ (kind.GetHashCode());
            hashCode = (hashCode*397) ^ (call != null ? call.GetHashCode() : 0);
            hashCode = (hashCode*397) ^ (argumentIndex.GetHashCode());
            return hashCode;
        }
    }

    public enum CallSiteKind
    {
        RECEIVER_CALL_SITE,
        PARAM_CALL_SITE,
    }

    // ReSharper restore InconsistentNaming

    public abstract class CoReName
    {
        protected CoReName(string name, string validationPattern)
        {
            var regex = new Regex(validationPattern);
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
    }

    public class CoReTypeName : CoReName
    {
        public CoReTypeName(string name) : base(name, ValidationPattern()) {}

        internal static string ValidationPattern()
        {
            return "L([a-zA-Z]+/)+[a-zA-Z]+";
        }
    }

    public class CoReMethodName : CoReName
    {
        public CoReMethodName(string name) : base(name, ValidationPattern()) {}

        private static string ValidationPattern()
        {
            return string.Format(@"{0}\.[a-zA-Z]+\(({0};)*\){0};", CoReTypeName.ValidationPattern());
        }
    }

    public class CoReFieldName : CoReName
    {
        public CoReFieldName(string name) : base(name, ValidationPattern()) {}

        private static string ValidationPattern()
        {
            return string.Format(@"{0}\.[a-zA-Z]+;{0}", CoReTypeName.ValidationPattern());
        }
    }
}