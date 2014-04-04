using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Resolve;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using KaVE.Model.Names;
using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.Utils.Names;
using NuGet;

namespace KaVE.VsFeedbackGenerator.Analysis
{
    internal class CalledMethodsAnalysis
    {
        public ISet<IMethodName> Analyze(IMethodDeclaration methodDeclaration, ITypeName enclosingType)
        {
            var context = new CollectionContext();
            if (methodDeclaration.Body != null)
            {
                methodDeclaration.Body.Accept(new MethodInvocationCollector(enclosingType), context);
            }
            return context.CalledMethods;
        }

        private class CollectionContext
        {
            public readonly ISet<IMethodName> CalledMethods = new HashSet<IMethodName>();
            public readonly ISet<IMethodName> AnalyzedMethods = new HashSet<IMethodName>();
        }

        private class MethodInvocationCollector : TreeNodeVisitor<CollectionContext>
        {
            private readonly ITypeName _enclosingType;

            public MethodInvocationCollector(ITypeName enclosingType)
            {
                _enclosingType = enclosingType;
            }

            public override void VisitNode(ITreeNode node, CollectionContext context)
            {
                foreach (var childNode in node.Children<ICSharpTreeNode>())
                {
                    childNode.Accept(this, context);
                }
            }

            public override void VisitInvocationExpression(IInvocationExpression invocation, CollectionContext context)
            {
                // TODO test calls nested as call parameters
                VisitSubexpressions(invocation, context);
                AnalyzeInvocation(invocation, context);
            }

            private void VisitSubexpressions(IInvocationExpression invocation, CollectionContext context)
            {
                base.VisitInvocationExpression(invocation, context);
            }

            private void AnalyzeInvocation(IInvocationExpression invocation, CollectionContext context)
            {
                var invocationRef = invocation.Reference;
                if (invocationRef != null)
                {
                    AnalyzeInvocationReference(context, invocationRef);
                }
            }

            private void AnalyzeInvocationReference(CollectionContext context, ICSharpInvocationReference invocationRef)
            {
                var method = ResolveMethod(invocationRef);
                var methodName = method.GetName<IMethodName>();
                if (IsLocalHelper(methodName) && !method.Element.IsOverride)
                {
                    if (context.AnalyzedMethods.Contains(methodName) || method.Element.IsAbstract)
                    {
                        return;
                    }
                    context.AnalyzedMethods.Add(methodName);
                    var declaration = GetDeclaration(method.Element);
                    declaration.Body.Accept(this, context);
                }
                else
                {
                    context.CalledMethods.Add(methodName);
                }
            }

            private bool IsLocalHelper(IMemberName method)
            {
                return _enclosingType == method.DeclaringType;
            }

            private static DeclaredElementInstance<IMethod> ResolveMethod(ICSharpInvocationReference invocationRef)
            {
                var resolvedRef = invocationRef.Resolve().Result;
                IMethod declaration = null;
                ISubstitution substitution = null;
                if (resolvedRef.DeclaredElement != null)
                {
                    declaration = (IMethod) resolvedRef.DeclaredElement;
                    substitution = resolvedRef.Substitution;
                }
                else if (!resolvedRef.Candidates.IsEmpty())
                {
                    declaration = (IMethod) resolvedRef.Candidates.First();
                    substitution = resolvedRef.CandidateSubstitutions.First();
                }

                if (declaration != null)
                {
                    return new DeclaredElementInstance<IMethod>(declaration, substitution);
                }
                return Asserts.Fail<DeclaredElementInstance<IMethod>>("unresolvable method");
            }

            private static IMethodDeclaration GetDeclaration(IMethod method)
            {
                var declarations = method.GetDeclarations();
                Asserts.That(declarations.Count <= 1, "more than one declaration for invoked method");
                var declaration = (IMethodDeclaration) declarations.First();
                return declaration;
            }
        }
    }
}