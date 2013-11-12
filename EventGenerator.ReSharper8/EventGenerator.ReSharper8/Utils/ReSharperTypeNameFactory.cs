using System;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Names;
using KaVE.Model.Names.CSharp;
using KaVE.Utils.Assertion;

namespace KaVE.EventGenerator.ReSharper8.Utils
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
            var assemblyId = type.Assembly != null ? type.Assembly.FullName : "<unspecified>";
            return TypeName.Get(String.Format("{0}, {1}", type.GetFullName(), assemblyId));
        }

        [NotNull]
        private static ITypeName GetName(this IArrayType arrayType)
        {
            var typeName = arrayType.ElementType.GetName();
            // TODO insert array braces
            return typeName;
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

        private static string GetFullName(this IExpressionType type)
        {
            var longPresentableName = type.GetLongPresentableName(CSharpLanguage.Instance);
            return CSharpNameUtils.GetFullTypeNameFromTypeAlias(longPresentableName);
        }
    }
}