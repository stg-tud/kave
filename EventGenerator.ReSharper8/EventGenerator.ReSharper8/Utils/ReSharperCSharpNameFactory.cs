using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Names;
using KaVE.Model.Names.CSharp;
using KaVE.Utils;

namespace KaVE.EventGenerator.ReSharper8.Utils
{
    // TODO implement this with accompanying tests
    public static class ReSharperCSharpNameFactory
    {
        public static ITypeName GetName(this ITypeElement typeElement)
        {
            // TODO add the type kind (struct, enum, class, ...) to the name information
            //var typeElementIdentifier = typeElement.toString();
            //var typeKind = typeElementIdentifier.SubString(1, typeElementIdentifier.IndexOf(':') - 1);
            // TODO do we want assembly versions here? This information is not contained in the typeElement...
            return TypeName.Get(typeElement.GetAssemblyQualifiedName());
        }

        public static INamespaceName GetName(this INamespace ns)
        {
            return NamespaceName.Get(ns.QualifiedName);
        }

        [NotNull]
        public static IParameterName GetName(this IParameter parameter)
        {
            var identifier = new StringBuilder();
            identifier.AppendIf(parameter.IsParameterArray, ParameterName.VarArgsModifier);
            identifier.AppendIf(parameter.IsValueVariable, ParameterName.OutputModifier);
            identifier.AppendIf(parameter.IsOptional, ParameterName.OptionalModifier);
            //identifier.AppendIf(parameter., ParameterName.PassByReferenceModifier);
            identifier.AppendType(parameter.Type).Append(" ").Append(parameter.ShortName);
            return ParameterName.Get(identifier.ToString());
        }

        public static IMethodName GetName(this IFunction method)
        {
            var identifier = new StringBuilder();
            identifier.AppendIf(method.IsStatic, MemberName.StaticModifier);
            identifier.AppendType(method.ReturnType).Append(" ").AppendType(method.GetContainingType()).Append(".").Append(method.ShortName);
            identifier.Append("(").Append(String.Join(", ", GetNamesFrom(method.Parameters))).Append(")");
            return MethodName.Get(identifier.ToString());
        }

        public static IEnumerable<IParameterName> GetNamesFrom(this IEnumerable<IParameter> parameters)
        {
            return parameters.Select(GetName);
        }

        public static IFieldName GetName(this IField typeElement)
        {
            throw new System.NotImplementedException();
        }

        private static String GetAssemblyQualifiedName(this ITypeElement type)
        {
            return type.GetClrName().FullName + ", " + type.Module.Name;
        }

        private static StringBuilder AppendType(this StringBuilder identifier, IType type)
        {
            var longPresentableName = type.GetLongPresentableName(CSharpLanguage.Instance);
            var fullTypeName = CSharpNameUtils.GetFullTypeNameFromTypeAlias(longPresentableName);
            return identifier.Append("[").Append(fullTypeName).Append(", ").Append(type.Module.Name).Append("]");
        }

        private static StringBuilder AppendType(this StringBuilder identifier, ITypeElement type)
        {
            return identifier.Append("[").Append(type.GetAssemblyQualifiedName()).Append("]");
        }
    }
}
