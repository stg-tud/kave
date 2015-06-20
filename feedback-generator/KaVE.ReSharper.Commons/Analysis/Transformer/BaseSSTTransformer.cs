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
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using KaVE.Commons.Model.Names;
using KaVE.RS.Commons.Analysis.Transformer.Context;
using KaVE.RS.Commons.Utils.Names;

namespace KaVE.RS.Commons.Analysis.Transformer
{
    public abstract class BaseSSTTransformer<TContext> : TreeNodeVisitor<TContext> where TContext : ITransformerContext
    {
        public override void VisitNode(ITreeNode node, TContext context)
        {
            //Registry.GetComponent<ILogger>().Info(
            //    string.Format("{0} has no treatment for TreeNodes of type {1}", GetType().Name, node.GetType().Name));
        }

        protected static void HandleInvocationExpression(IInvocationExpression invocationExpressionParam,
            ITransformerContext context,
            Action<string, IMethodName, string[], ITypeName> handler)
        {
            var invokedExpression = invocationExpressionParam.InvokedExpression as IReferenceExpression;
            if (invocationExpressionParam.Reference != null && invokedExpression != null)
            {
                var resolvedMethod = invocationExpressionParam.Reference.ResolveMethod();
                if (resolvedMethod != null)
                {
                    var methodName = resolvedMethod.GetName<IMethodName>();
                    var typeName = invocationExpressionParam.Type().GetName();
                    string callee = null;
                    if (invokedExpression.QualifierExpression == null ||
                        invokedExpression.QualifierExpression is IThisExpression)
                    {
                        callee = "this";
                    }
                    else if (invokedExpression.QualifierExpression is IBaseExpression)
                    {
                        callee = "base";
                    }
                    else if (invokedExpression.QualifierExpression is IReferenceExpression)
                    {
                        var referenceExpression = invokedExpression.QualifierExpression as IReferenceExpression;
                        if (referenceExpression.IsClassifiedAsVariable)
                        {
                            callee = referenceExpression.NameIdentifier.Name;
                        }
                    }
                    else if (invokedExpression.QualifierExpression is IInvocationExpression)
                    {
                        callee = invokedExpression.QualifierExpression.GetReference(context);
                    }
                    else
                    {
                        return;
                    }
                    var arguments = invocationExpressionParam.ArgumentList.GetArguments(context);
                    handler(callee, methodName, arguments, typeName);
                }
            }
        }
    }
}