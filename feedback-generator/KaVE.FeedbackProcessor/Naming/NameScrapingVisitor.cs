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

using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.SSTs.Declarations;
using KaVE.Commons.Model.SSTs.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Visitor;
using KaVE.Commons.Model.SSTs.References;
using KaVE.Commons.Model.SSTs.Statements;
using KaVE.Commons.Utils.Collections;

namespace KaVE.FeedbackProcessor.Naming
{
    internal class NameScrapingVisitor : AbstractNodeVisitor<IKaVESet<IName>>
    {
        public override void Visit(IFieldReference fieldRef, IKaVESet<IName> context)
        {
            context.Add(fieldRef.FieldName);
            base.Visit(fieldRef, context);
        }

        public override void Visit(IEventReference eventRef, IKaVESet<IName> context)
        {
            context.Add(eventRef.EventName);
            base.Visit(eventRef, context);
        }

        public override void Visit(IMethodReference methodRef, IKaVESet<IName> context)
        {
            context.Add(methodRef.MethodName);
            base.Visit(methodRef, context);
        }

        public override void Visit(IPropertyReference propertyRef, IKaVESet<IName> context)
        {
            context.Add(propertyRef.PropertyName);
            base.Visit(propertyRef, context);
        }

        public override void Visit(IVariableDeclaration stmt, IKaVESet<IName> context)
        {
            context.Add(stmt.Type);
            base.Visit(stmt, context);
        }

        public override void Visit(IDelegateDeclaration stmt, IKaVESet<IName> context)
        {
            context.Add(stmt.Name);
            base.Visit(stmt, context);
        }

        public override void Visit(IFieldDeclaration stmt, IKaVESet<IName> context)
        {
            context.Add(stmt.Name);
            base.Visit(stmt, context);
        }

        public override void Visit(IEventDeclaration stmt, IKaVESet<IName> context)
        {
            context.Add(stmt.Name);
            base.Visit(stmt, context);
        }

        public override void Visit(IMethodDeclaration stmt, IKaVESet<IName> context)
        {
            context.Add(stmt.Name);
            base.Visit(stmt, context);
        }

        public override void Visit(ILambdaExpression expr, IKaVESet<IName> context)
        {
            context.Add(expr.Name);
            base.Visit(expr, context);
        }

        public override void Visit(IInvocationExpression entity, IKaVESet<IName> context)
        {
            context.Add(entity.MethodName);
            base.Visit(entity, context);
        }
    }
}