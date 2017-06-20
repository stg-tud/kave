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
using KaVE.Commons.Model.Naming.Impl.v0;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Model.SSTs.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Visitor;
using KaVE.Commons.Model.SSTs.References;

namespace KaVE.FeedbackProcessor.StatisticsUltimate
{
    public class ContextStatisticsExtractor
    {
        private readonly IContextFilter _cf;

        public ContextStatisticsExtractor(IContextFilter cf)
        {
            _cf = cf;
        }

        public IContextStatistics Extract(IEnumerable<Context> contexts)
        {
            var stats = new ContextStatistics
            {
                NumSolutions = 1
            };

            foreach (var ctx in contexts)
            {
                if (_cf.ShouldProcessOrRegister(ctx.SST))
                {
                    Extract(ctx, stats);
                }
            }

            return stats;
        }

        private void Extract(Context ctx, ContextStatistics stats)
        {
            var type = ctx.SST.EnclosingType;
            if (ctx.SST.IsPartialClass)
            {
                stats.NumPartial++;
            }
            ExtractTypeStats(stats, type);

            var th = ctx.TypeShape.TypeHierarchy;
            if (th.Implements.Count > 0 || th.Extends != null)
            {
                stats.NumTypeDeclExtendsOrImplements++;
            }

            foreach (var md in ctx.SST.Methods)
            {
                if (!_cf.ShouldProcessOrRegister(md.Name.RemoveGenerics()))
                {
                    continue;
                }

                stats.NumMethodDeclsTotal++;
                var overridden = false;
                var overriddenAsm = false;
                foreach (var mh in ctx.TypeShape.MethodHierarchies)
                {
                    if (md.Name.Equals(mh.Element))
                    {
                        overridden = mh.Super != null || mh.First != null;

                        var isOverridingSuperAsm = mh.Super != null && !mh.Super.DeclaringType.Assembly.IsLocalProject;
                        var isOverridingFirstAsm = mh.First != null && !mh.First.DeclaringType.Assembly.IsLocalProject;
                        overriddenAsm = isOverridingSuperAsm || isOverridingFirstAsm;
                    }
                }
                if (overridden)
                {
                    stats.NumMethodDeclsOverrideOrImplement++;
                }
                if (overriddenAsm)
                {
                    stats.NumMethodDeclsOverrideOrImplementAsm++;
                    stats.UniqueMethodDeclsOverrideOrImplementAsm.Add(md.Name.RemoveGenerics());
                }
            }

            ctx.SST.Accept(new StatsCountVisitor(), stats);
            stats.EstimatedLinesOfCode += ctx.SST.Accept(new LinesOfCodeVisitor(), 0);
        }

        private static void ExtractTypeStats(ContextStatistics stats, ITypeName type)
        {
            stats.UniqueTypeDecl.Add(type);

            if (type.IsNestedType)
            {
                stats.NumTypeDeclNested++;
            }
            else
            {
                stats.NumTypeDeclTopLevel++;
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
                if (m.IsUnknown)
                {
                    stats.NumUnknownInvocations++;
                }
                else
                {
                    stats.NumValidInvocations++;

                    if (!m.DeclaringType.Assembly.IsLocalProject)
                    {
                        if (m.IsDelegateInvocation)
                        {
                            stats.NumAsmDelegateCalls++;
                        }
                        else
                        {
                            stats.NumAsmCalls++;
                        }
                        stats.UniqueAsmMethods.Add(m.RemoveGenerics());
                        stats.UniqueAssemblies.Add(m.DeclaringType.Assembly);
                    }
                }
            }

            public override void Visit(IFieldReference fieldRef, ContextStatistics stats)
            {
                var f = fieldRef.FieldName;
                if (!f.IsUnknown && !f.DeclaringType.Assembly.IsLocalProject)
                {
                    stats.NumAsmFieldRead++;
                    stats.UniqueAsmFields.Add(f.RemoveGenerics());
                    stats.UniqueAssemblies.Add(f.DeclaringType.Assembly);
                }
            }

            public override void Visit(IMethodReference methRef, ContextStatistics stats)
            {
                var m = methRef.MethodName;
                if (!m.IsUnknown && !m.DeclaringType.Assembly.IsLocalProject)
                {
                    stats.UniqueAsmMethods.Add(m.RemoveGenerics());
                    stats.UniqueAssemblies.Add(m.DeclaringType.Assembly);
                }
            }

            public override void Visit(IPropertyReference propRef, ContextStatistics stats)
            {
                var p = propRef.PropertyName;
                if (!p.IsUnknown && !p.DeclaringType.Assembly.IsLocalProject)
                {
                    stats.NumAsmPropertyRead++;
                    stats.UniqueAsmProperties.Add(p.RemoveGenerics());
                    stats.UniqueAssemblies.Add(p.DeclaringType.Assembly);
                }
            }
        }
    }
}