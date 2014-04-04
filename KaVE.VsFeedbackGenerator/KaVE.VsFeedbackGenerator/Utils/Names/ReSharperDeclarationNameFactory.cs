using JetBrains.ReSharper.Psi.CSharp.Tree;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Names;
using KaVE.Utils.Assertion;

namespace KaVE.VsFeedbackGenerator.Utils.Names
{
    public static class ReSharperDeclarationNameFactory
    {
        [CanBeNull]
        public static IMethodName GetName([NotNull] this IMethodDeclaration methodDeclaration)
        {
            var declaredElement = methodDeclaration.DeclaredElement;
            Asserts.NotNull(declaredElement, "no declared element in declaration");
            return declaredElement.GetName<IMethodName>();
        }
    }
}
