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
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Model.SSTs.Declarations;

namespace KaVE.Commons.Utils.ObjectUsageExport
{
    public class QueryExtractor : UsageExtractor
    {
        private Query _matchingQuery;

        public Query Extract(Context ctx)
        {
            try
            {
                Extract(ctx.SST);
            }
            catch (CompletionPointFoundException) {}

            if (_matchingQuery == null)
            {
                return null;
            }

            RewriteContexts(new[] {_matchingQuery}, ctx.TypeShape);
            RewriteThisType(new[] {_matchingQuery}, ctx.TypeShape);

            var matchingQuery = _matchingQuery;
            _matchingQuery = null;
            return matchingQuery;
        }

        protected override void ProcessMethodBody(IMethodDeclaration methodDecl, UsageContext context)
        {
            base.ProcessMethodBody(methodDecl, context);

            if (context.TargetType != null)
            {
                _matchingQuery = context.AllQueries.First(q => context.TargetType.Equals(q.type));

                // prevent traversion of the complete SST
                throw new CompletionPointFoundException();
            }
        }

        private class CompletionPointFoundException : Exception {}
    }
}