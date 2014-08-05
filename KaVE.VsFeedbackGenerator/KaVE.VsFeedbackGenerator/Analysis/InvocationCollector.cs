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
 *    - Sven Amann
 */

using System.Collections.Generic;
using JetBrains.ProjectModel.Model2.Assemblies.Interfaces;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using KaVE.Model.Names;
using KaVE.VsFeedbackGenerator.Utils.Names;

namespace KaVE.VsFeedbackGenerator.Analysis
{
    public class InvocationCollector : TreeNodeVisitor<ISet<MethodRef>>
    {
        public override void VisitNode(ITreeNode node, ISet<MethodRef> context)
        {
            foreach (var childNode in node.Children<ICSharpTreeNode>())
            {
                childNode.Accept(this, context);
            }
        }

        public override void VisitInvocationExpression(IInvocationExpression invocation, ISet<MethodRef> context)
        {
            base.VisitInvocationExpression(invocation, context);
            AnalyzeInvocation(invocation, context);
        }

        private void AnalyzeInvocation(IInvocationExpression invocation, ISet<MethodRef> context)
        {
            var invocationRef = invocation.Reference;
            if (invocationRef != null)
            {
                var method = invocationRef.ResolveMethod();
                if (method != null)
                {
                    var methodName = method.GetName<IMethodName>();
                    MethodRef entryPoint;

                    if (IsFromAssembly(method))
                    {
                        entryPoint = MethodRef.CreateAssemblyReference(methodName, method.Element);
                    }
                    else
                    {
                        var methodDeclaration = method.Element.GetDeclaration();
                        entryPoint = MethodRef.CreateLocalReference(methodName, method.Element, methodDeclaration);
                    }
                    context.Add(entryPoint);
                }
            }
        }

        private static bool IsFromAssembly(DeclaredElementInstance<IMethod> method)
        {
            var assembly = method.Element.Module.ContainingProjectModule as IAssembly;
            return assembly != null;
        }
    }
}