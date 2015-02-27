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
 *    - Sebastian Proksch
 */

using KaVE.Model.SSTs.Declarations;
using KaVE.Model.SSTs.Expressions.Assignable;
using KaVE.Model.SSTs.Expressions.LoopHeader;
using KaVE.Model.SSTs.Expressions.Simple;
using KaVE.Model.SSTs.References;
using KaVE.Model.SSTs.Visitor;

namespace KaVE.Model.SSTs.Impl.Visitor
{
    public class AbstractNodeVisitor<TContext> : ISSTNodeVisitor<TContext>
    {
        public virtual void Visit(ISST sst, TContext context) {}

        // declarations
        public virtual void Visit(IDelegateDeclaration stmt, TContext context) {}
        public virtual void Visit(IEventDeclaration stmt, TContext context) {}
        public virtual void Visit(IFieldDeclaration stmt, TContext context) {}
        public virtual void Visit(IMethodDeclaration stmt, TContext context) {}
        public virtual void Visit(IPropertyDeclaration stmt, TContext context) {}

        // statements
        public virtual void Visit(IStatement stmt, TContext context) {}

        // Expressions
        public virtual void Visit(IComposedExpression expr, TContext context) {}
        public virtual void Visit(IExpressionCompletion expr, TContext context) {}
        public virtual void Visit(IIfElseExpression expr, TContext context) {}
        public virtual void Visit(IInvocationExpression expr, TContext context) {}
        public virtual void Visit(ILambdaExpression expr, TContext context) {}
        public virtual void Visit(ILoopHeaderBlockExpression expr, TContext context) {}
        public virtual void Visit(IConstantValueExpression expr, TContext context) {}
        public virtual void Visit(INullExpression expr, TContext context) {}
        public virtual void Visit(IReferenceExpression expr, TContext context) {}

        // References
        public virtual void Visit(IEventReference eventRef, TContext context) {}
        public virtual void Visit(IFieldReference fieldRef, TContext context) {}
        public virtual void Visit(IMethodReference methodRef, TContext context) {}
        public virtual void Visit(IPropertyReference methodRef, TContext context) {}
        public virtual void Visit(IVariableReference varRef, TContext context) {}
    }
}