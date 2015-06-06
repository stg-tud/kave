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
 *    - Roman Fojtik
 *    - Sebastian Proksch
 */

using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;

namespace KaVE.Commons.Utils.ObjectUsageExport
{
    internal class QueryContext
    {
        internal ITypeName EnclosingType { get; set; }
        internal IMethodName EnclosingMethod { get; set; }

        internal IKaVEList<Query> AllQueries { get; private set; }
        internal QueryScope Scope { get; private set; }

        public QueryContext()
        {
            EnclosingType = TypeName.UnknownName;
            EnclosingMethod = MethodName.UnknownName;
            AllQueries = Lists.NewList<Query>();
            Scope = new QueryScope();
        }

        public void EnterNewScope()
        {
            Scope = new QueryScope(Scope);
        }

        public void LeaveCurrentScope()
        {
            Asserts.NotNull(Scope.ParentScope);
            Scope = Scope.ParentScope;
        }

        public void DefineVariable(string id, ITypeName type, DefinitionSite defSite)
        {
            if (Scope.IsExistingInCurrentScope(id))
            {
                return;
            }

            if (Scope.IsExistingInCurrentScope(type))
            {
                var q = Scope.Find(type);
                Scope.Register(id, q);
            }
            else
            {
                var q = NewQueryFor(type, defSite);
                Scope.Register(id, q);
                Scope.Register(type, q);
            }
        }

        public void RegisterCallsite(string id, IMethodName callsite)
        {
            // TODO @seb fix analysis and then remove this check!
            if (Equals("", callsite.Name))
            {
                return;
            }

            if (Scope.IsExisting(id))
            {
                var q = Scope.Find(id);
                q.sites.Add(CallSites.CreateReceiverCallSite(callsite));
            }
            else
            {
                var type = callsite.DeclaringType;
                if (Scope.IsExisting(type))
                {
                    var q = Scope.Find(type);
                    q.sites.Add(CallSites.CreateReceiverCallSite(callsite));
                }
                else
                {
                    var q = NewQueryFor(type, DefinitionSites.CreateUnknownDefinitionSite());
                    q.sites.Add(CallSites.CreateReceiverCallSite(callsite));

                    Scope.Register(type, q);
                }
            }
        }

        public void RegisterDefinition(string id, DefinitionSite defSite)
        {
            if (Scope.IsExisting(id))
            {
                var q = Scope.Find(id);
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