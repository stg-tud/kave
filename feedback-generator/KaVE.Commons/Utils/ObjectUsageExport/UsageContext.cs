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

using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;

namespace KaVE.Commons.Utils.ObjectUsageExport
{
    public class UsageContext
    {
        internal ITypeName EnclosingType { get; set; }
        internal IMethodName EnclosingMethod { get; set; }
        internal CoReTypeName TargetType { get; set; }

        internal IKaVEList<Query> AllQueries { get; private set; }
        internal ScopedNameResolver NameResolver { get; private set; }

        public UsageContext()
        {
            EnclosingType = TypeName.UnknownName;
            EnclosingMethod = MethodName.UnknownName;
            AllQueries = Lists.NewList<Query>();
            NameResolver = new ScopedNameResolver();
        }

        public void EnterNewScope()
        {
            NameResolver = new ScopedNameResolver(NameResolver);
        }

        public void LeaveCurrentScope()
        {
            Asserts.NotNull(NameResolver.ParentResolver);
            NameResolver = NameResolver.ParentResolver;
        }

        public void DefineVariable(string id, ITypeName type, DefinitionSite defSite)
        {
            if (NameResolver.IsExistingInCurrentScope(id))
            {
                return;
            }

            if (NameResolver.IsExistingInCurrentScope(type))
            {
                var q = NameResolver.Find(type);
                NameResolver.Register(id, q);
            }
            else
            {
                var q = NewQueryFor(type, defSite);
                NameResolver.Register(id, q);
                NameResolver.Register(type, q);
            }
        }

        public void RegisterCallsite(string id, IMethodName callsite)
        {
            // TODO @seb fix analysis and then remove this check!
            if (Equals("", callsite.Name))
            {
                return;
            }

            if (NameResolver.IsExisting(id))
            {
                var q = NameResolver.Find(id);
                q.sites.Add(CallSites.CreateReceiverCallSite(callsite));
            }
            else
            {
                var type = callsite.DeclaringType;
                if (NameResolver.IsExisting(type))
                {
                    var q = NameResolver.Find(type);
                    q.sites.Add(CallSites.CreateReceiverCallSite(callsite));
                }
                else
                {
                    var q = NewQueryFor(type, DefinitionSites.CreateUnknownDefinitionSite());
                    q.sites.Add(CallSites.CreateReceiverCallSite(callsite));

                    NameResolver.Register(type, q);
                }
            }
        }

        public void RegisterDefinition(string id, DefinitionSite defSite)
        {
            if (NameResolver.IsExisting(id))
            {
                var q = NameResolver.Find(id);
                q.definition = defSite;
            }
        }

        private Query NewQueryFor(ITypeName type, DefinitionSite defSite)
        {
            var q = new Query
            {
                type = type.ToCoReName(),
                classCtx = EnclosingType.ToCoReName(),
                methodCtx = EnclosingMethod.ToCoReName(),
                definition = defSite
            };
            AllQueries.Add(q);
            return q;
        }
    }
}