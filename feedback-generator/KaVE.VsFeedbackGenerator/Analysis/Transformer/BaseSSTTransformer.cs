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
using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.Analysis.Util;
using KaVE.VsFeedbackGenerator.Utils.Names;

namespace KaVE.VsFeedbackGenerator.Analysis.Transformer
{
    public interface ITransformerContext
    {
        ISSTFactory Factory { get; }
        ITempVariableGenerator Generator { get; }
        IScope Scope { get; }
    }

    public abstract class BaseSSTTransformer<TContext> : TreeNodeVisitor<TContext> where TContext : ITransformerContext
    {
        protected static void HandleInvocationExpression(IInvocationExpression invocationExpressionParam,
            ITransformerContext context,
            Action<string, IMethodName, string[], ITypeName> handler)
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
                    var refCollectorContext = new ReferenceCollectorContext(context);
                    invokedExpression.QualifierExpression.Accept(
                        context.Factory.ReferenceCollector(),
                        refCollectorContext);
                    Asserts.That(refCollectorContext.References.Count() == 1);
                    callee = refCollectorContext.References[0];
                }
                else
                {
                    return;
                }
                var argCollectorContext = new ArgumentCollectorContext(context);
                invocationExpressionParam.ArgumentList.Accept(context.Factory.ArgumentCollector(), argCollectorContext);
                handler(callee, methodName, argCollectorContext.Arguments.ToArray(), typeName);
            }
        }
    }
}