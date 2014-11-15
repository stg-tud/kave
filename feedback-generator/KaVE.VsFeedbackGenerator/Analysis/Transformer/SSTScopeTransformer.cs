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
 *    - Dennis Albrecht
 *    - Uli Fahrer
 */

using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.Util;
using KaVE.Model.SSTs.Declarations;
using KaVE.Model.SSTs.Statements;
using KaVE.VsFeedbackGenerator.Analysis.Util;
using KaVE.VsFeedbackGenerator.Utils.Names;

namespace KaVE.VsFeedbackGenerator.Analysis.Transformer
{
    public class ScopeTransformerContext : ITransformerContext
    {
        public ScopeTransformerContext(ITransformerContext context)
            : this(context.Factory, context.Generator, context.Scope) {}

        public ScopeTransformerContext(ISSTFactory factory,
            ITempVariableGenerator generator,
            IScope scope)
        {
            Generator = generator;
            Factory = factory;
            Scope = scope;
        }

        public ISSTFactory Factory { get; private set; }
        public ITempVariableGenerator Generator { get; private set; }
        public IScope Scope { get; private set; }
    }

    public class SSTScopeTransformer : BaseSSTTransformer<ScopeTransformerContext>
    {
        public override void VisitMethodDeclaration(IMethodDeclaration methodDeclarationParam,
            ScopeTransformerContext context)
        {
            methodDeclarationParam.Body.Accept(this, context);
        }

        public override void VisitBlock(IBlock blockParam, ScopeTransformerContext context)
        {
            blockParam.Statements.ForEach(s => s.Accept(this, context));
        }

        public override void VisitDeclarationStatement(IDeclarationStatement declarationStatementParam,
            ScopeTransformerContext context)
        {
            declarationStatementParam.Declaration.Accept(this, context);
        }

        public override void VisitExpressionStatement(IExpressionStatement expressionStatementParam,
            ScopeTransformerContext context)
        {
            expressionStatementParam.Expression.Accept(this, context);
        }

        public override void VisitMultipleLocalVariableDeclaration(
            IMultipleLocalVariableDeclaration multipleLocalVariableDeclarationParam,
            ScopeTransformerContext context)
        {
            multipleLocalVariableDeclarationParam.Declarators.ForEach(d => d.Accept(this, context));
        }

        public override void VisitMultipleDeclarationMember(IMultipleDeclarationMember multipleDeclarationMemberParam,
            ScopeTransformerContext context)
        {
            var name = multipleDeclarationMemberParam.NameIdentifier.Name;
            var type = multipleDeclarationMemberParam.Type.GetName();
            context.Scope.Body.Add(new VariableDeclaration(name, type));
        }

        public override void VisitLocalVariableDeclaration(ILocalVariableDeclaration localVariableDeclarationParam,
            ScopeTransformerContext context)
        {
            base.VisitLocalVariableDeclaration(localVariableDeclarationParam, new ScopeTransformerContext(context));

            if (localVariableDeclarationParam.Initializer is IExpressionInitializer)
            {
                var expression = (localVariableDeclarationParam.Initializer as IExpressionInitializer).Value;
                var dest = localVariableDeclarationParam.NameIdentifier.Name;
                expression.Accept(
                    context.Factory.AssignmentGenerator(),
                    new AssignmentGeneratorContext(context, dest));
            }
        }

        public override void VisitAssignmentExpression(IAssignmentExpression assignmentExpressionParam,
            ScopeTransformerContext context)
        {
            if (assignmentExpressionParam.Dest is IReferenceExpression)
            {
                var dest = (assignmentExpressionParam.Dest as IReferenceExpression).NameIdentifier.Name;
                assignmentExpressionParam.Source.Accept(
                    context.Factory.AssignmentGenerator(),
                    new AssignmentGeneratorContext(context, dest));
            }
        }

        public override void VisitInvocationExpression(IInvocationExpression invocationExpressionParam,
            ScopeTransformerContext context)
        {
            HandleInvocationExpression(
                invocationExpressionParam,
                context,
                (callee, method, args, retType) =>
                    context.Scope.Body.Add(new InvocationStatement(callee, method, args)));
        }
    }
}