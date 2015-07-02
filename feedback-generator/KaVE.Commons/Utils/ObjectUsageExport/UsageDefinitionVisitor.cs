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
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Model.SSTs.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.Visitor;

namespace KaVE.Commons.Utils.ObjectUsageExport
{
    internal class UsageDefinitionVisitor : AbstractNodeVisitor<UsageContext, DefinitionSite>
    {
        public override DefinitionSite Visit(IInvocationExpression entity,
            UsageContext context)
        {
            try
            {
                // TODO @seb: fix analysis and then remove this fix
                if (Equals("", entity.MethodName.Name))
                {
                    return DefinitionSites.CreateUnknownDefinitionSite();
                }

                if (entity.MethodName.IsConstructor)
                {
                    return DefinitionSites.CreateDefinitionByConstructor(entity.MethodName);
                }

                return DefinitionSites.CreateDefinitionByReturn(entity.MethodName);
            }
            catch (Exception e)
            {
                // TODO @seb: untested!
                Console.WriteLine("UsageDefinitionVisitor: caught exception, falling back to unknown DefinitionSite");
                return DefinitionSites.CreateUnknownDefinitionSite();
            }
        }

        public override DefinitionSite Visit(IConstantValueExpression expr,
            UsageContext context)
        {
            return new DefinitionSite
            {
                kind = DefinitionSiteKind.CONSTANT
            };
        }
    }
}