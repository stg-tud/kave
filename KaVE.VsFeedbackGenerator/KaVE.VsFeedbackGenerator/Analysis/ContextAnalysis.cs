using JetBrains.ReSharper.Feature.Services.CSharp.CodeCompletion.Infrastructure;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Impl.reflection2.elements.Compiled;
using JetBrains.ReSharper.Psi.Tree;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names;
using KaVE.VsFeedbackGenerator.Utils.Names;

namespace KaVE.VsFeedbackGenerator.Analysis
{
    internal class ContextAnalysis
    {
        public Context Analyze(CSharpCodeCompletionContext rsContext)
        {
            var context = new Context();
            var methodDeclaration = FindEnclosing<IMethodDeclaration>(rsContext.NodeInFile);
            var typeDeclaration = FindEnclosing<ITypeDeclaration>(rsContext.NodeInFile);
            var methodName = GetName(methodDeclaration);
            context.EnclosingMethod = methodName;
            var typeName = GetName(typeDeclaration);
            context.EnclosingClassHierarchy = new TypeHierarchy();
            context.EnclosingClassHierarchy.Element = typeName;

            return context;
        }

        private ITypeName GetName(ITypeDeclaration typeDeclaration)
        {
            var declaredElement = typeDeclaration.DeclaredElement;
            var idSubstitution = declaredElement.IdSubstitution;
            return declaredElement.GetName(idSubstitution) as ITypeName;
        }

        private static IMethodName GetName(IMethodDeclaration methodDeclaration)
        {
            var declaredElement = methodDeclaration.DeclaredElement;
            var idSubstitution = declaredElement.IdSubstitution;
            return declaredElement.GetName(idSubstitution) as IMethodName;
        }

        private static TIDeclaration FindEnclosing<TIDeclaration>(ITreeNode node) where TIDeclaration : class,IDeclaration
        {
            if (node == null)
            {
                return null;
            }

            var methodDeclaration = node as TIDeclaration;
            return methodDeclaration ?? FindEnclosing<TIDeclaration>(node.Parent);
        }
    }
}