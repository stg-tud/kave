using System;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Util;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Names;
using KaVE.Model.Names.CSharp;
using KaVE.Utils.Assertion;

namespace KaVE.VsFeedbackGenerator.Utils.Names
{
    public static class ReSharperTypeNameFactory
    {
        [NotNull]
        public static ITypeName GetName(this IType type)
        {
            return IfTypeIs<IDeclaredType>(type, GetName) ??
                   IfTypeIs<IArrayType>(type, GetName) ??
                   IfTypeIs<IAnonymousType>(type, GetName) ??
                   IfTypeIs<IMultitype>(type, GetName) ??
                   IfTypeIs<IPointerType>(type, GetName) ??
                   Asserts.Fail<ITypeName>("unknown IType case: {0}", type.GetType());
        }

        private static ITypeName IfTypeIs<TE>(IType element, Func<TE, ITypeName> map) where TE : class, IType
        {
            var specificElement = element as TE;
            return specificElement != null ? map(specificElement) : null;
        }

        [NotNull]
        private static ITypeName GetName(this IDeclaredType type)
        {
            var typeElement = type.GetTypeElement();
            // typeElement can be null, for example when resolving the second
            // parameter type in the incomplete method declaration
            // > public void M(int i, )
            return typeElement == null
                ? TypeName.Get(TypeName.UnknownTypeIdentifier)
                : (ITypeName) typeElement.GetName(type.GetSubstitution());
        }

        [NotNull]
        private static ITypeName GetName(this IArrayType arrayType)
        {
            return arrayType.ElementType.GetName().DeriveArrayTypeName(arrayType.Rank);
        }

        [NotNull]
        private static ITypeName GetName(this IAnonymousType type)
        {
            Asserts.Fail("cannot create name for anonymous type");
            return null;
        }

        [NotNull]
        private static ITypeName GetName(this IMultitype type)
        {
            Asserts.Fail("cannot create name for multitype");
            return null;
        }

        [NotNull]
        private static ITypeName GetName(this IPointerType type)
        {
            return type.ElementType.GetName();
        }
    }
}