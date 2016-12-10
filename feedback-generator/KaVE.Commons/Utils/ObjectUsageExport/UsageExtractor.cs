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
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Declarations;
using KaVE.Commons.Model.TypeShapes;
using KaVE.Commons.Utils.Collections;

namespace KaVE.Commons.Utils.ObjectUsageExport
{
    public interface IUsageExtractor
    {
        IKaVEList<Query> Export(Context ctx);
    }

    public class UsageExtractor : IUsageExtractor
    {
        private readonly UsageExtractionVisitor _collectorVisitor = new UsageExtractionVisitor();

        public IKaVEList<Query> Export(Context ctx)
        {
            var allQueries = Extract(ctx.SST);
            var queriesWithCalls = Lists.NewListFrom(allQueries.Where(q => q.sites.Count > 0));
            RewriteContexts(queriesWithCalls, ctx.TypeShape);
            RewriteThisType(queriesWithCalls, ctx.TypeShape);
            return queriesWithCalls;
        }

        public IKaVEList<Query> Extract(ISST sst)
        {
            var queries = Lists.NewList<Query>();

            foreach (var method in sst.Methods)
            {
                var context = new UsageContext
                {
                    Enclosings =
                    {
                        Type = sst.EnclosingType,
                        Method = method.Name
                    }
                };

                AddDefaultQueries(sst, context);

                Extract(method, context);

                foreach (var query in context.AllQueries)
                {
                    queries.Add(query);
                }
            }

            return queries;
        }

        private static void AddDefaultQueries(ISST sst, UsageContext context)
        {
            context.DefineVariable("this", sst.EnclosingType, DefinitionSites.CreateDefinitionByThis());
            context.DefineVariable("base", sst.EnclosingType, DefinitionSites.CreateDefinitionByThis());

            foreach (var field in sst.Fields)
            {
                var id = field.Name.Name;
                var type = field.Name.ValueType;
                var definition = DefinitionSites.CreateDefinitionByField(field.Name);
                context.DefineVariable(id, type, definition);
            }

            foreach (var property in sst.Properties)
            {
                var id = property.Name.Name;
                var type = property.Name.ValueType;
                var definition = DefinitionSites.CreateDefinitionByField(Names.UnknownField);
                context.DefineVariable(id, type, definition);
            }
        }

        private void Extract(IMethodDeclaration methodDecl, UsageContext context)
        {
            context.EnterNewScope();

            var parameters = methodDecl.Name.Parameters;
            for (var argIndex = 0; argIndex < parameters.Count; argIndex++)
            {
                var parameter = parameters[argIndex];

                var id = parameter.Name;
                var type = parameter.ValueType;
                var def = DefinitionSites.CreateDefinitionByParam(methodDecl.Name, argIndex);

                context.DefineVariable(id, type, def);
            }

            ProcessMethodBody(methodDecl, context);

            context.LeaveCurrentScope();
        }

        protected virtual void ProcessMethodBody(IMethodDeclaration methodDecl, UsageContext context)
        {
            foreach (var stmt in methodDecl.Body)
            {
                stmt.Accept(_collectorVisitor, context);
            }
        }

        protected void RewriteContexts(IEnumerable<Query> queries, ITypeShape typeShape)
        {
            foreach (var query in queries)
            {
                query.classCtx = GetClassContext(query.classCtx, typeShape.TypeHierarchy);
                query.methodCtx = GetMethodContext(query.methodCtx, typeShape.MethodHierarchies);
            }
        }

        protected void RewriteThisType(IEnumerable<Query> queries, ITypeShape typeShape)
        {
            var hasSuper = typeShape.TypeHierarchy.Extends != null;
            if (hasSuper)
            {
                var superType = typeShape.TypeHierarchy.Extends.Element;
                var allThisQueries = queries.Where(q => q.definition.kind == DefinitionSiteKind.THIS);
                foreach (var q in allThisQueries)
                {
                    q.type = superType.ToCoReName();
                }
            }
        }

        private static CoReTypeName GetClassContext(CoReTypeName classCtx, ITypeHierarchy typeHierarchy)
        {
            var wasLambdaCtx = IsLambdaContext(classCtx);
            if (typeHierarchy.Extends != null)
            {
                // TODO @seb: fix analysis and remove check
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (typeHierarchy.Extends.Element != null)
                {
                    classCtx = typeHierarchy.Extends.Element.ToCoReName();
                }
            }
            var isLambdaCtx = IsLambdaContext(classCtx);
            if (wasLambdaCtx && !isLambdaCtx)
            {
                return new CoReTypeName(classCtx + "$Lambda");
            }
            return classCtx;
        }

        private static bool IsLambdaContext(CoReName cName)
        {
            return cName.Name.Contains("$Lambda");
        }

        private static CoReMethodName GetMethodContext(CoReMethodName method,
            IEnumerable<IMemberHierarchy<IMethodName>> hierarchies)
        {
            var orig = method;
            var wasLambdaName = IsLambdaContext(method);
            method = new CoReMethodName(method.Name.Replace("$Lambda", ""));

            foreach (var methodHierarchy in hierarchies)
            {
                // TODO @seb: fix analysis and then remove this check
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (methodHierarchy.Element == null)
                {
                    continue;
                }

                var elem = methodHierarchy.Element.ToCoReName();
                if (elem.Equals(method))
                {
                    var outMethod = method;
                    if (methodHierarchy.First != null)
                    {
                        outMethod = methodHierarchy.First.ToCoReName();
                    }
                    else if (methodHierarchy.Super != null)
                    {
                        outMethod = methodHierarchy.Super.ToCoReName();
                    }

                    if (wasLambdaName)
                    {
                        var oldName = "." + outMethod.Method + "(";
                        var newName = "." + outMethod.Method + "$Lambda(";
                        var newId = outMethod.Name.Replace(oldName, newName);
                        outMethod = new CoReMethodName(newId);
                    }

                    return outMethod;
                }
            }

            return orig;
        }
    }
}