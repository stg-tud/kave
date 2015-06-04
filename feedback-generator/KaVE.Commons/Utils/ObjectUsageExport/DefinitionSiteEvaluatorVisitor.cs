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
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Model.SSTs.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.Visitor;
using KaVE.Commons.Model.SSTs.References;
using KaVE.Commons.Utils.Assertion;

namespace KaVE.Commons.Utils.ObjectUsageExport
{
    internal class DefinitionSiteEvaluatorVisitor :
        AbstractNodeVisitor<InvocationCollectorVisitor.QueryContext, DefinitionSite>
    {
        public override DefinitionSite Visit(IInvocationExpression entity,
            InvocationCollectorVisitor.QueryContext context)
        {
            DefinitionSite defSite;
            if (entity.MethodName.IsConstructor)
            {
                defSite = DefinitionSites.CreateDefinitionByConstructor(entity.MethodName);
            }
            else
            {
                try
                {
                    defSite = DefinitionSites.CreateDefinitionByReturn(entity.MethodName);
                }
                catch (AssertException e)
                {
                    // TODO @seb: test and proper handling
                    Console.WriteLine("error creating definition site:\n{0}", e);
                    defSite = DefinitionSites.CreateUnknownDefinitionSite();
                }
            }

            return defSite;
        }

        public override DefinitionSite Visit(IConstantValueExpression expr,
            InvocationCollectorVisitor.QueryContext context)
        {
            return new DefinitionSite
            {
                kind = DefinitionSiteKind.CONSTANT
            };
        }

        public override DefinitionSite Visit(IReferenceExpression expr, InvocationCollectorVisitor.QueryContext context)
        {
            return expr.Reference.Accept(this, context);
        }

        public override DefinitionSite Visit(IFieldReference fieldRef, InvocationCollectorVisitor.QueryContext context)
        {
            return DefinitionSites.CreateDefinitionByField(fieldRef.FieldName);
        }

        public override DefinitionSite Visit(IPropertyReference propertyRef,
            InvocationCollectorVisitor.QueryContext context)
        {
            return
                DefinitionSites.CreateDefinitionByField(
                    InvocationCollectorVisitor.PropertyToFieldName(propertyRef.PropertyName));
        }

        public override DefinitionSite Visit(IVariableReference varRef, InvocationCollectorVisitor.QueryContext context)
        {
            return DefinitionSites.CreateUnknownDefinitionSite();
        }
    }
}