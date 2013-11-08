using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.DeclaredElements;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Names;
using KaVE.Model.Names.CSharp;
using KaVE.Utils;
using KaVE.Utils.Assertion;

namespace KaVE.EventGenerator.ReSharper8.Utils
{
    public static class ReSharperCSharpNameFactory
    {
        [NotNull]
        public static IName GetName([NotNull] this IDeclaredElement enclosingElement)
        {
            var namespaceProposal = enclosingElement as INamespace;
            if (namespaceProposal != null)
            {
                return namespaceProposal.GetName();
            }
            var typeElement = enclosingElement as ITypeElement;
            if (typeElement != null)
            {
                return typeElement.GetName();
            }
            var function = enclosingElement as IFunction;
            if (function != null)
            {
                return function.GetName();
            }
            var parameter = enclosingElement as IParameter;
            if (parameter != null)
            {
                return parameter.GetName();
            }
            var field = enclosingElement as IField;
            if (field != null)
            {
                return field.GetName();
            }
            var variable = enclosingElement as ITypeOwner;
            if (variable != null)
            {
                return variable.GetName();
            }
            var alias = enclosingElement as IAlias;
            if (alias != null)
            {
                return alias.GetName();
            }

            // TODO identify other cases and add them here

            Asserts.Fail("unknown kind of declared element: {0}", enclosingElement.GetType());
            return null;
        }

        [NotNull]
        private static ITypeName GetName(this ITypeElement typeElement)
        {
            // TODO add the type kind (struct, enum, class, ...) to the name information
            //var typeElementIdentifier = typeElement.toString();
            //var typeKind = typeElementIdentifier.SubString(1, typeElementIdentifier.IndexOf(':') - 1);
            // TODO do we want assembly versions here? This information is not contained in the typeElement...
            return TypeName.Get(typeElement.GetAssemblyQualifiedName());
        }

        [NotNull]
        private static INamespaceName GetName(this INamespace ns)
        {
            return NamespaceName.Get(ns.QualifiedName);
        }

        [NotNull]
        private static IParameterName GetName(this IParameter parameter)
        {
            var identifier = new StringBuilder();
            identifier.AppendIf(parameter.IsParameterArray, ParameterName.VarArgsModifier);
            identifier.AppendIf(parameter.Kind == ParameterKind.OUTPUT, ParameterName.OutputModifier);
            identifier.AppendIf(parameter.IsOptional, ParameterName.OptionalModifier);
            identifier.AppendIf(parameter.Kind == ParameterKind.REFERENCE, ParameterName.PassByReferenceModifier);
            identifier.AppendType(parameter.Type).Append(" ").Append(parameter.ShortName);
            return ParameterName.Get(identifier.ToString());
        }

        [NotNull]
        private static IMethodName GetName(this IFunction method)
        {
            var identifier = new StringBuilder();
            identifier.AppendIf(method.IsStatic, MemberName.StaticModifier);
            identifier.Append(method, method.ReturnType);
            identifier.Append("(").Append(String.Join(", ", method.Parameters.GetNames())).Append(")");
            return MethodName.Get(identifier.ToString());
        }

        [NotNull]
        private static IEnumerable<IParameterName> GetNames(this IEnumerable<IParameter> parameters)
        {
            return parameters.Select(GetName);
        }

        [NotNull]
        private static IFieldName GetName(this IField field)
        {
            var identifier = new StringBuilder();
            identifier.AppendIf(field.IsStatic, MemberName.StaticModifier);
            identifier.Append(field, field.Type);
            return FieldName.Get(identifier.ToString());
        }

        [NotNull]
        private static LocalVariableName GetName(this ITypeOwner variable)
        {
            var identifier = new StringBuilder();
            identifier.AppendType(variable.Type).Append(' ').Append(variable.ShortName);
            return LocalVariableName.Get(identifier.ToString());
        }

        [NotNull]
        private static IName GetName(this IAlias alias)
        {
            return AliasName.Get(alias.ShortName);
        }

        [NotNull]
        public static ITypeName GetName(this IType type)
        {
            var declaredType = type as IDeclaredType;
            if (declaredType != null)
            {
                return declaredType.GetName();
            }
            var arrayType = type as IArrayType;
            if (arrayType != null)
            {
                return arrayType.GetName();
            }
            var anonymousType = type as IAnonymousType;
            if (anonymousType != null)
            {
                return anonymousType.GetName();
            }
            var multitype = type as IMultitype;
            if (multitype != null)
            {
                return multitype.GetName();
            }
            var pointerType = type as IPointerType;
            if (pointerType != null)
            {
                return pointerType.GetName();
            }

            Asserts.Fail("unknown IType case: {0}", type.GetType());
            return null;
        }

        [NotNull]
        private static ITypeName GetName(this IDeclaredType type)
        {
            var assemblyId = type.Assembly != null ? type.Assembly.FullName : "<unspecified>";
            return TypeName.Get(string.Format("{0}, {1}", type.GetFullName(), assemblyId));
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

        private static void Append(this StringBuilder identifier, IClrDeclaredElement member, IType valueType)
        {
            identifier.AppendType(valueType).Append(' ').AppendType(member.GetContainingType()).Append('.').Append(member.ShortName);
        }

        private static StringBuilder AppendType(this StringBuilder identifier, IType type)
        {
            return identifier.Append('[').Append(type.GetName().Identifier).Append(']');
        }

        [NotNull]
        private static StringBuilder AppendType(this StringBuilder identifier, ITypeElement type)
        {
            return identifier.Append('[').Append(type.GetAssemblyQualifiedName()).Append(']');
        }

        [NotNull]
        private static String GetAssemblyQualifiedName(this ITypeElement type)
        {
            return string.Format("{0}, {1}", type.GetClrName().FullName, type.Module.Name);
        }
    }
}
