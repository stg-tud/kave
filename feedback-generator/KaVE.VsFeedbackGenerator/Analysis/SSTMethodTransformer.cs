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
using KaVE.VsFeedbackGenerator.Utils.Names;

namespace KaVE.VsFeedbackGenerator.Analysis
{
    public class SSTMethodTransformer : TreeNodeVisitor
    {
        private readonly MethodDeclaration _declaration;

        public SSTMethodTransformer(MethodDeclaration declaration)
        {
            _declaration = declaration;
        }

        public override void VisitMethodDeclaration(IMethodDeclaration methodDeclarationParam)
        {
            methodDeclarationParam.Body.Accept(this);
        }

        public override void VisitBlock(IBlock blockParam)
        {
            blockParam.Statements.ForEach(s => s.Accept(this));
        }

        public override void VisitDeclarationStatement(IDeclarationStatement declarationStatementParam)
        {
            declarationStatementParam.Declaration.Accept(this);
        }

        public override void VisitExpressionStatement(IExpressionStatement expressionStatementParam)
        {
            expressionStatementParam.Expression.Accept(this);
        }

        public override void VisitMultipleLocalVariableDeclaration(
            IMultipleLocalVariableDeclaration multipleLocalVariableDeclarationParam)
        {
            multipleLocalVariableDeclarationParam.Declarators.ForEach(d => d.Accept(this));
        }

        public override void VisitMultipleDeclarationMember(IMultipleDeclarationMember multipleDeclarationMemberParam)
        {
            var name = multipleDeclarationMemberParam.NameIdentifier.Name;
            var type = multipleDeclarationMemberParam.Type.GetName();
            _declaration.Body.Add(new VariableDeclaration(name, type));
        }

        public override void VisitLocalVariableDeclaration(ILocalVariableDeclaration localVariableDeclarationParam)
        {
            base.VisitLocalVariableDeclaration(localVariableDeclarationParam);

            if (localVariableDeclarationParam.Initializer is IExpressionInitializer)
            {
                var expression = (localVariableDeclarationParam.Initializer as IExpressionInitializer).Value;
                expression.Accept(new SSTAssignmentGenerator(_declaration, localVariableDeclarationParam.NameIdentifier.Name));
            }
        }

        public override void VisitAssignmentExpression(IAssignmentExpression assignmentExpressionParam)
        {
            if (assignmentExpressionParam.Dest is IReferenceExpression)
            {
                var dest = (assignmentExpressionParam.Dest as IReferenceExpression).NameIdentifier.Name;
                assignmentExpressionParam.Source.Accept(new SSTAssignmentGenerator(_declaration, dest));
            }
        }
    }
}