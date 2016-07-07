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
using System.Linq;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;

namespace KaVE.Commons.Utils.ObjectUsageExport
{
    public class UsageContext
    {
        internal ScopedEnclosings Enclosings { get; private set; }
        internal CoReTypeName TargetType { get; set; }

        internal IKaVEList<Query> AllQueries { get; private set; }
        internal ScopedNameResolver NameResolver { get; private set; }

        public UsageContext()
        {
            Enclosings = new ScopedEnclosings();
            AllQueries = Lists.NewList<Query>();
            NameResolver = new ScopedNameResolver();
        }

        public void EnterNewScope()
        {
            NameResolver = new ScopedNameResolver(NameResolver);
            Enclosings = new ScopedEnclosings(Enclosings);
        }

        public void EnterNewLambdaScope()
        {
            var names = NameResolver.BoundNames.ToList();

            EnterNewScope();
            Enclosings.Type = AddLambda(Enclosings.Type);
            Enclosings.Method = AddLambda(Enclosings.Method);

            foreach (var name in names)
            {
                var q = NameResolver.Find(name);
                DefineVariable(name, q.type, q.definition);
            }
        }

        internal static ITypeName AddLambda(ITypeName type)
        {
            var oldName = type.Name + ",";
            var newName = type.Name + "$Lambda,";
            var newId = type.Identifier.Replace(oldName, newName);
            return Names.Type(newId);
        }

        internal static IMethodName AddLambda(IMethodName method)
        {
            var oldName = "." + method.Name + "(";
            var newName = "." + method.Name + "$Lambda(";
            var newId = method.Identifier.Replace(oldName, newName);
            return Names.Method(newId);
        }

        public void LeaveCurrentScope()
        {
            Asserts.NotNull(NameResolver.ParentResolver);
            NameResolver = NameResolver.ParentResolver;
            Asserts.NotNull(Enclosings.Parent);
            Enclosings = Enclosings.Parent;
        }

        public void DefineVariable(string id, ITypeName type, DefinitionSite defSite)
        {
            DefineVariable(id, type.ToCoReName(), defSite);
        }

        public void DefineVariable(string id, CoReTypeName type, DefinitionSite defSite)
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
                var type = callsite.DeclaringType.ToCoReName();
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

        private Query NewQueryFor(CoReTypeName type, DefinitionSite defSite)
        {
            var q = new Query();
            AllQueries.Add(q);
            try
            {
                q.definition = defSite;
                q.type = type;
                q.classCtx = Enclosings.Type.ToCoReName();
                q.methodCtx = Enclosings.Method.ToCoReName();
                return q;
            }
            catch (Exception)
            {
                Console.WriteLine("failed to create new Query, falling back to Unknown");
                return q;
            }
        }
    }
}