using System;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Names;
using KaVE.Model.Names.CSharp;

namespace KaVE.VsFeedbackGenerator.Utils
{
    internal static class TypeNameExtensions
    {
        // TODO testen oder raus
        public static string ToTypeCategory([NotNull] this ITypeName elem)
        {
            if (elem.IsReferenceType)
            {
                if (elem.IsClassType)
                {
                    return "class";
                }
                if (elem.IsInterfaceType)
                {
                    return "interface";
                }
                if (elem.IsArrayType)
                {
                    return "array";
                }
                if (elem.IsDelegateType)
                {
                    return "delegate";
                }
                throw new ArgumentException(@"Given ITypeName claims to be a ReferenceType but does not match any subtype", "elem");
            }
            if (elem.IsValueType)
            {
                if (elem.IsVoidType)
                {
                    return "void";
                }
                if (elem.IsEnumType)
                {
                    return "enum";
                }
                if (elem.IsStructType)
                {
                    return elem.IsNullableType ? "nullable" : (elem.IsSimpleType ? "simple" : "struct");
                }
                throw new ArgumentException(@"Given ITypeName claims to be a ValueType but does not match any subtype", "elem");
            }
            if (elem.IsUnknownType)
            {
                return TypeName.UnknownTypeIdentifier;
            }
            throw new ArgumentException(@"Given ITypeName does not match any type", "elem");
        }
    }
}