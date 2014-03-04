using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using KaVE.Model.Names;
using KaVE.VsFeedbackGenerator.Utils.Names;

namespace KaVE.VsFeedbackGenerator.Analysis
{
    internal class MethodInvocationCollector : TreeNodeVisitor<MethodInvocationCollector.CollectionContext>
    {
        private readonly ITypeName _enclosingType;

        internal class CollectionContext
        {
            internal CollectionContext()
            {
                CalledMethods = new HashSet<IMethodName>();
                AnalyzedMethods = new HashSet<IMethodName>();
            }

            internal ISet<IMethodName> CalledMethods { get; private set; }
            internal ISet<IMethodName> AnalyzedMethods { get; private set; }
        }

        private MethodInvocationCollector(ITypeName enclosingType)
        {
            _enclosingType = enclosingType;
        }

        public static ISet<IMethodName> FindCalledMethodsIn(IMethodDeclaration methodDeclaration,
            ITypeName enclosingType)
        {
            var context = new CollectionContext();
            if (methodDeclaration.Body != null)
            {
                methodDeclaration.Body.Accept(new MethodInvocationCollector(enclosingType), context);
            }
            return context.CalledMethods;
        }

        public override void VisitInvocationExpression(IInvocationExpression invocation, CollectionContext context)
        {
            var invocationRef = invocation.Reference;
            if (invocationRef != null)
            {
                var resolvedRef = invocationRef.Resolve();
                var declaredElement = resolvedRef.DeclaredElement;
                var method = (IMethod) declaredElement;
                var methodName = method.GetName<IMethodName>(resolvedRef.Result.Substitution);
                if (IsLocalHelper(methodName))
                {
                    if (context.AnalyzedMethods.Contains(methodName))
                    {
                        return;
                    }
                    context.AnalyzedMethods.Add(methodName);
                    var declaration = (IMethodDeclaration) method.GetDeclarations().First();
                    declaration.Body.Accept(this, context);
                }
                else
                {
                    context.CalledMethods.Add(methodName);
                }
            }
            base.VisitInvocationExpression(invocation, context);
        }

        private bool IsLocalHelper(IMemberName method)
        {
            return _enclosingType == method.DeclaringType;
        }

        public override void VisitNode(ITreeNode node, CollectionContext context)
        {
            foreach (var childNode in node.Children<ICSharpTreeNode>())
            {
                childNode.Accept(this, context);
            }
        }
    }
}