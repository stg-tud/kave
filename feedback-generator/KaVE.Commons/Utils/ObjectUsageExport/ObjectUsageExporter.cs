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
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils.Assertion;

namespace KaVE.Commons.Utils.ObjectUsageExport
{
    public class ObjectUsageExporter
    {
        public ICollection<Query> Export(Context ctx)
        {
            var collectorVisitor = new InvocationCollectorVisitor();
            var queryContext = new InvocationCollectorVisitor.QueryContext();
            ctx.SST.Accept(collectorVisitor, queryContext);

            var queries = queryContext.GetQueries();
            foreach (var query in queries)
            {
                query.methodCtx = GetMethodContext(ctx, query);
            }

            return queries;
        }

        public ICollection<Query> CleanExport(Context ctx)
        {
            var queries = Export(ctx);
            var cleanedQueries = new Collection<Query>();

            foreach (var query in queries)
            {
                if (!query.sites.Count.Equals(0))
                {
                    cleanedQueries.Add(query);
                }
            }

            return cleanedQueries;
        }

        private CoReMethodName GetMethodContext(Context ctx, Query query)
        {
            foreach (var methodHierarchy in ctx.TypeShape.MethodHierarchies)
            {
                try
                {
                    var methodCtx = methodHierarchy.Element.ToCoReName();
                    if (methodCtx.Equals(query.methodCtx))
                    {
                        if (methodHierarchy.First != null)
                        {
                            return methodHierarchy.First.ToCoReName();
                        }
                        if (methodHierarchy.Super != null)
                        {
                            return methodHierarchy.Super.ToCoReName();
                        }
                    }
                }
                catch (AssertException e)
                {
                    // TODO test and proper handling
                    Console.WriteLine("error getting methog context:\n{0}", e);
                    return MethodName.UnknownName.ToCoReName();
                }
            }

            return query.methodCtx;
        }
    }
}