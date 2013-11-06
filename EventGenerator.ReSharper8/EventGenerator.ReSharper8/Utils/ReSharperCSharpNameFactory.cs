using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.DeclaredElements;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Names;
using KaVE.Model.Names.CSharp;
using KaVE.Utils;
using KaVE.Utils.Assertion;

namespace KaVE.EventGenerator.ReSharper8.Utils
{
    // TODO implement this with accompanying tests
    public static class ReSharperCSharpNameFactory
    {
        [NotNull]
        public static IName GetName(this IDeclaredElement enclosingElement)
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
            var constructor = enclosingElement as IConstructor;
            if (constructor != null)
            {
                return constructor.GetName();
            }
            var method = enclosingElement as IMethod;
            if (method != null)
            {
                return method.GetName();
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
            var variableDeclaration = enclosingElement as IVariableDeclaration;
            if (variableDeclaration != null)
            {
                return variableDeclaration.GetName();
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
        public static ITypeName GetName(this ITypeElement typeElement)
        {
            // TODO add the type kind (struct, enum, class, ...) to the name information
            //var typeElementIdentifier = typeElement.toString();
            //var typeKind = typeElementIdentifier.SubString(1, typeElementIdentifier.IndexOf(':') - 1);
            // TODO do we want assembly versions here? This information is not contained in the typeElement...
            return TypeName.Get(typeElement.GetAssemblyQualifiedName());
        }

        [NotNull]
        public static INamespaceName GetName(this INamespace ns)
        {
            return NamespaceName.Get(ns.QualifiedName);
        }

        [NotNull]
        public static IParameterName GetName(this IParameter parameter)
        {
            // TODO test parameter flags
            var identifier = new StringBuilder();
            identifier.AppendIf(parameter.IsParameterArray, ParameterName.VarArgsModifier);
            identifier.AppendIf(parameter.IsValueVariable, ParameterName.OutputModifier);
            identifier.AppendIf(parameter.IsOptional, ParameterName.OptionalModifier);
            //identifier.AppendIf(parameter., ParameterName.PassByReferenceModifier);
            identifier.AppendType(parameter.Type).Append(" ").Append(parameter.ShortName);
            return ParameterName.Get(identifier.ToString());
        }

        [NotNull]
        public static IMethodName GetName(this IFunction method)
        {
            var identifier = new StringBuilder();
            identifier.AppendIf(method.IsStatic, MemberName.StaticModifier);
            identifier.Append(method, method.ReturnType);
            identifier.Append("(").Append(String.Join(", ", method.Parameters.GetNames())).Append(")");
            return MethodName.Get(identifier.ToString());
        }

        [NotNull]
        public static IEnumerable<IParameterName> GetNames(this IEnumerable<IParameter> parameters)
        {
            return parameters.Select(GetName);
        }

        [NotNull]
        public static IFieldName GetName(this IField field)
        {
            var identifier = new StringBuilder();
            identifier.AppendIf(field.IsStatic, MemberName.StaticModifier);
            identifier.Append(field, field.Type);
            return FieldName.Get(identifier.ToString());
        }

        [NotNull]
        public static LocalVariableName GetName(this IVariableDeclaration variable)
        {
            var identifier = new StringBuilder();
            identifier.AppendType(variable.Type).Append(' ').Append(variable.DeclaredName);
            return LocalVariableName.Get(identifier.ToString());
        }

        private static void Append(this StringBuilder identifier, IClrDeclaredElement member, IType valueType)
        {
            identifier.AppendType(valueType)
                .Append(' ')
                .AppendType(member.GetContainingType())
                .Append('.')
                .Append(member.ShortName);
        }

        [NotNull]
        public static IName GetName(this IAlias alias)
        {
            return AliasName.Get(alias.ShortName);
        }

        [NotNull]
        private static String GetAssemblyQualifiedName(this ITypeElement type)
        {
            return type.GetClrName().FullName + ", " + type.Module.Name;
        }

        [NotNull]
        private static StringBuilder AppendType(this StringBuilder identifier, IExpressionType type)
        {
            var longPresentableName = type.GetLongPresentableName(CSharpLanguage.Instance);
            var fullTypeName = CSharpNameUtils.GetFullTypeNameFromTypeAlias(longPresentableName);
            return identifier.Append('[').Append(fullTypeName).Append(", ").Append(type.Module.Name).Append(']');
        }

        [NotNull]
        private static StringBuilder AppendType(this StringBuilder identifier, ITypeElement type)
        {
            return identifier.Append('[').Append(type.GetAssemblyQualifiedName()).Append(']');
        }
    }
}
