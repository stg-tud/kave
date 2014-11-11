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

using System.Linq;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using KaVE.Model.Names;
using KaVE.Model.SSTs.Declarations;
using KaVE.Model.SSTs.Expressions;
using KaVE.Model.SSTs.Statements;
using KaVE.VsFeedbackGenerator.Utils.Names;

namespace KaVE.VsFeedbackGenerator.Analysis
{
    public class SSTAssignmentGenerator : BaseSSTTransformer
    {
        private readonly string _dest;

        public SSTAssignmentGenerator(MethodDeclaration declaration, string dest) : base(declaration)
        {
            _dest = dest;
        }

        public override void VisitCSharpLiteralExpression(ICSharpLiteralExpression cSharpLiteralExpressionParam)
        {
            Declaration.Body.Add(new Assignment(_dest, new ConstantExpression()));
        }

        public override void VisitReferenceExpression(IReferenceExpression referenceExpressionParam)
        {
            var name = referenceExpressionParam.NameIdentifier.Name;
            Declaration.Body.Add(new Assignment(_dest, ComposedExpression.Create(name)));
        }

        public override void VisitInvocationExpression(IInvocationExpression invocationExpressionParam)
        {
            var invokedExpression = invocationExpressionParam.InvokedExpression as IReferenceExpression;
            if (invocationExpressionParam.Reference != null &&
                invokedExpression != null &&
                invokedExpression.QualifierExpression is IReferenceExpression)
            {
                var collector = new SSTArgumentCollector(Declaration);

                var methodName = invocationExpressionParam.Reference.ResolveMethod().GetName<IMethodName>();
                invocationExpressionParam.ArgumentList.Accept(collector);
                var name = (invokedExpression.QualifierExpression as IReferenceExpression).NameIdentifier.Name;

                Declaration.Body.Add(new Assignment(_dest, new InvocationExpression(name, methodName, collector.Arguments)));
            }
        }

        public override void VisitBinaryExpression(IBinaryExpression binaryExpressionParam)
        {
            AddAssignmentAfterCollectingReferences(binaryExpressionParam);
        }

        public override void VisitArrayCreationExpression(IArrayCreationExpression arrayCreationExpressionParam)
        {
            AddAssignmentAfterCollectingReferences(arrayCreationExpressionParam.ArrayInitializer);
        }

        private void AddAssignmentAfterCollectingReferences(ICSharpTreeNode treeNode)
        {
            var collector = new SSTReferenceCollector(Declaration);
            treeNode.Accept(collector);
            var references = collector.References;
            if (references.Any())
            {
                Declaration.Body.Add(new Assignment(_dest, ComposedExpression.Create(references)));
            }
            else
            {
                Declaration.Body.Add(new Assignment(_dest, new ConstantExpression()));
            }
        }
    }
}
