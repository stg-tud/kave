/*
 * Copyright 2017 Sebastian Proksch
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
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Model.SSTs.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Visitor;
using KaVE.Commons.Model.SSTs.References;

namespace KaVE.FeedbackProcessor.StatisticsUltimate
{
    public class ContextStatisticsExtractor
    {
        public IContextStatistics Extract(IEnumerable<Context> contexts)
        {
            var stats = new ContextStatistics
            {
                NumSolutions = 1
            };

            foreach (var ctx in contexts)
            {
                Extract(ctx, stats);
            }

            return stats;
        }

        private void Extract(Context ctx, ContextStatistics stats)
        {
            var type = ctx.SST.EnclosingType;
            ExtractTypeStats(stats, type);

            var th = ctx.TypeShape.TypeHierarchy;
            if (th.Implements.Count > 0 || th.Extends != null)
            {
                stats.NumTypeExtendsOrImplements++;
            }

            foreach (var md in ctx.SST.Methods)
            {
                stats.NumMethodDecls++;
                var overridden = false;
                foreach (var mh in ctx.TypeShape.MethodHierarchies)
                {
                    if (md.Name.Equals(mh.Element) && (mh.Super != null || mh.First != null))
                    {
                        overridden = true;
                    }
                }
                if (overridden)
                {
                    stats.NumMethodOverridesOrImplements++;
                }
            }

            ctx.SST.Accept(new StatsCountVisitor(), stats);
            stats.EstimatedLinesOfCode += ctx.SST.Accept(new LinesOfCodeVisitor(), 0);
        }

        private static void ExtractTypeStats(ContextStatistics stats, ITypeName type)
        {
            if (type.IsNestedType)
            {
                stats.NumNestedType++;
            }
            else
            {
                stats.NumTopLevelType++;
            }

            if (type.IsUnknown || type.IsPredefined || type.IsArray)
            {
                stats.NumUnusualType++;
            }
            else if (type.IsClassType)
            {
                stats.NumClasses++;
            }
            else if (type.IsInterfaceType)
            {
                stats.NumInterfaces++;
            }
            else if (type.IsDelegateType)
            {
                stats.NumDelegates++;
            }
            else if (type.IsStructType)
            {
                stats.NumStructs++;
            }
            else if (type.IsEnumType)
            {
                stats.NumEnums++;
            }
        }

        private class StatsCountVisitor : AbstractNodeVisitor<ContextStatistics>
        {
            public override void Visit(IInvocationExpression expr, ContextStatistics stats)
            {
                var m = expr.MethodName;
                if (!m.DeclaringType.Assembly.IsLocalProject)
                {
                    stats.NumAsmCalls++;
                    stats.UniqueAsmMethods.Add(m);
                    stats.UniqueAssemblies.Add(m.DeclaringType.Assembly);
                }
            }

            public override void Visit(IFieldReference fieldRef, ContextStatistics stats)
            {
                var f = fieldRef.FieldName;
                if (!f.DeclaringType.Assembly.IsLocalProject)
                {
                    stats.NumAsmFieldRead++;
                    stats.UniqueAsmFields.Add(f);
                    stats.UniqueAssemblies.Add(f.DeclaringType.Assembly);
                }
            }

            public override void Visit(IMethodReference methRef, ContextStatistics stats)
            {
                var m = methRef.MethodName;
                if (!m.DeclaringType.Assembly.IsLocalProject)
                {
                    stats.UniqueAsmMethods.Add(m);
                    stats.UniqueAssemblies.Add(m.DeclaringType.Assembly);
                }
            }

            public override void Visit(IPropertyReference propRef, ContextStatistics stats)
            {
                var p = propRef.PropertyName;
                if (!p.DeclaringType.Assembly.IsLocalProject)
                {
                    stats.NumAsmPropertyRead++;
                    stats.UniqueAsmProperties.Add(p);
                    stats.UniqueAssemblies.Add(p.DeclaringType.Assembly);
                }
            }
        }
    }
}