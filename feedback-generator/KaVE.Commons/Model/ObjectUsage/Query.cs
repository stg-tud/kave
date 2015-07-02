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
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Collections;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.ObjectUsage
{
    // ReSharper disable InconsistentNaming
    public class Query
    {
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

        public bool HasReceiverCallSites
        {
            get { return sites.Any(c => c.kind == CallSiteKind.RECEIVER); }
        }

        public Query()
            : this(Lists.NewList<CallSite>()) {}

        public Query(IEnumerable<CallSite> callSites)
        {
            type = TypeName.UnknownName.ToCoReName();
            methodCtx = MethodName.UnknownName.ToCoReName();
            classCtx = TypeName.UnknownName.ToCoReName();
            definition = DefinitionSites.CreateUnknownDefinitionSite();
            sites = Lists.NewList(callSites.ToArray());
        }

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

        public override string ToString()
        {
            return this.ToStringReflection();
        }
    }
}