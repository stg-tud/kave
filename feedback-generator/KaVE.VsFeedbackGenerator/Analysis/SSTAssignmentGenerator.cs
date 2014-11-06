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
 */

using JetBrains.ReSharper.Psi.CSharp.Tree;
using KaVE.Model.SSTs.Declarations;
using KaVE.Model.SSTs.Expressions;
using KaVE.Model.SSTs.Statements;

namespace KaVE.VsFeedbackGenerator.Analysis
{
    public class SSTAssignmentGenerator : TreeNodeVisitor
    {
        private readonly MethodDeclaration _declaration;
        private readonly string _dest;

        public SSTAssignmentGenerator(MethodDeclaration declaration, string dest)
        {
            _declaration = declaration;
            _dest = dest;
        }

        public override void VisitCSharpLiteralExpression(ICSharpLiteralExpression cSharpLiteralExpressionParam)
        {
            _declaration.Body.Add(new Assignment(_dest, new ConstantExpression()));
        }

        public override void VisitReferenceExpression(IReferenceExpression referenceExpressionParam)
        {
            var name = referenceExpressionParam.NameIdentifier.Name;
            _declaration.Body.Add(new Assignment(_dest, ComposedExpression.Create(name)));
        }
    }
}
