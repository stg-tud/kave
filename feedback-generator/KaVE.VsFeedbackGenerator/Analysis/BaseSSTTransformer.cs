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

using System;
using System.Linq;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using KaVE.Model.Names;
using KaVE.Model.SSTs.Declarations;
using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.Utils.Names;

namespace KaVE.VsFeedbackGenerator.Analysis
{
    public abstract class BaseSSTTransformer : TreeNodeVisitor
    {
        protected readonly MethodDeclaration Declaration;

        protected BaseSSTTransformer(MethodDeclaration declaration)
        {
            Declaration = declaration;
        }

        protected void HandleInvocationExpression(IInvocationExpression invocationExpressionParam,
            Action<MethodDeclaration, string, IMethodName, string[], ITypeName> handler)
        {
            var invokedExpression = invocationExpressionParam.InvokedExpression as IReferenceExpression;
            if (invocationExpressionParam.Reference != null && invokedExpression != null)
            {
                var methodName = invocationExpressionParam.Reference.ResolveMethod().GetName<IMethodName>();
                var typeName = invocationExpressionParam.Type().GetName();
                string callee;
                if (invokedExpression.QualifierExpression is IReferenceExpression)
                {
                    callee = (invokedExpression.QualifierExpression as IReferenceExpression).NameIdentifier.Name;
                }
                else if (invokedExpression.QualifierExpression is IInvocationExpression)
                {
                    var refCollector = new SSTReferenceCollector(Declaration);
                    invokedExpression.QualifierExpression.Accept(refCollector);
                    Asserts.That(refCollector.References.Count() == 1);
                    callee = refCollector.References[0];
                }
                else
                {
                    return;
                }
                var argCollector = new SSTArgumentCollector(Declaration);
                invocationExpressionParam.ArgumentList.Accept(argCollector);
                handler(Declaration, callee, methodName, argCollector.Arguments, typeName);
            }
        }
    }
}