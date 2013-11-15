using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.DeclaredElements;
using JetBrains.ReSharper.Psi.ExtensionsAPI;
using JetBrains.Util;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Names;
using KaVE.Model.Names.CSharp;
using KaVE.Utils;
using KaVE.Utils.Assertion;

namespace KaVE.EventGenerator.ReSharper8.Utils
{
    public static class ReSharperDeclaredElementNameFactory
    {
        [NotNull]
        public static IName GetName([NotNull] this IDeclaredElement element)
        {
            if (element.ShortName == SharedImplUtil.MISSING_DECLARATION_NAME)
            {
                return Name.Get(SharedImplUtil.MISSING_DECLARATION_NAME);
            }
            return IfElementIs<INamespace>(element, GetName) ??
                   IfElementIs<ITypeElement>(element, GetName) ??
                   IfElementIs<IFunction>(element, GetName) ??
                   IfElementIs<IParameter>(element, GetName) ??
                   IfElementIs<IField>(element, GetName) ??
                   IfElementIs<IProperty>(element, GetName) ??
                   IfElementIs<IEvent>(element, GetName) ??
                   IfElementIs<ITypeOwner>(element, GetName) ??
                   IfElementIs<IAlias>(element, GetName) ??
                   Asserts.Fail<IName>("unknown kind of declared element: {0}", element.GetType());
        }

        private static IName IfElementIs<TE>(IDeclaredElement element, Func<TE, IName> map)
            where TE : class, IDeclaredElement
        {
            var specificElement = element as TE;
            return specificElement != null ? map(specificElement) : null;
        }

        [NotNull]
        public static ITypeName GetName(this ITypeElement typeElement)
        {
            // TODO add the type kind (struct, enum, class, ...) to the name information
            //var typeElementIdentifier = typeElement.toString();
            //var typeKind = typeElementIdentifier.SubString(1, typeElementIdentifier.IndexOf(':') - 1);
            return TypeName.Get((typeElement is ITypeParameter) ? typeElement.ShortName + " -> ?" : typeElement.GetAssemblyQualifiedName());
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
            identifier.Append(method.GetMemberIdentifier(method.ReturnType));
            identifier.AppendParameters(method);
            return MethodName.Get(identifier.ToString());
        }

        [NotNull]
        private static IFieldName GetName(this IField field)
        {
            return FieldName.Get(field.GetMemberIdentifier(field.Type));
        }

        [NotNull]
        private static IEventName GetName(this IEvent evt)
        {
            return EventName.Get(evt.GetMemberIdentifier(evt.Type));
        }

        [NotNull]
        private static IPropertyName GetName(this IProperty property)
        {
            var identifier = new StringBuilder();
            identifier.AppendIf(property.IsWritable, PropertyName.SetterModifier);
            identifier.AppendIf(property.IsReadable, PropertyName.GetterModifier);
            identifier.Append(property.GetMemberIdentifier(property.ReturnType));
            identifier.AppendParameters(property);
            return PropertyName.Get(identifier.ToString());
        }

        private static string GetMemberIdentifier(this ITypeMember member, IType valueType)
        {
            var identifier = new StringBuilder();
            identifier.AppendIf(member.IsStatic, MemberName.StaticModifier);
            identifier.Append(member, valueType);
            return identifier.ToString();
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

        private static void Append(this StringBuilder identifier, IClrDeclaredElement member, IType valueType)
        {
            // TODO can we resolve type parameters of the containing type?
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
            var containingModule = type.Module.ContainingProjectModule;
            Asserts.NotNull(containingModule, "module is null");
            if (type.TypeParameters.IsEmpty())
            {
                return string.Format("{0}, {1}", type.GetClrName().FullName, containingModule.GetQualifiedName());
            }
            else
            {
                return string.Format(
                    "{0}[[{2}]], {1}",
                    type.GetClrName().FullName,
                    containingModule.GetQualifiedName(),
                    type.TypeParameters.Select(tp => tp.GetName().Identifier).Join("],["));
            }
        }

        private static string GetQualifiedName(this IModule containingModule)
        {
            var containingProject = containingModule as IProject;
            if (containingProject != null)
            {
                var assemblyInfo = containingProject.GetOutputAssemblyInfo();
                return assemblyInfo != null ? assemblyInfo.AssemblyNameInfo.FullName : containingModule.Name;
            }
            // containingModule is IAssembly
            return containingModule.Presentation;
        }

        private static void AppendParameters(this StringBuilder identifier, IParametersOwner parametersOwner)
        {
            identifier.Append("(").Append(String.Join(", ", parametersOwner.Parameters.GetNames())).Append(")");
        }

        [NotNull]
        private static IEnumerable<IParameterName> GetNames(this IEnumerable<IParameter> parameters)
        {
            return parameters.Select(GetName);
        }
    }
}