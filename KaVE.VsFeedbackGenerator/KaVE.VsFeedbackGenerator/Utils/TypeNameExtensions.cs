using System;
using KaVE.Model.Names;
using KaVE.Model.Names.CSharp;
using KaVE.Utils.Assertion;

namespace KaVE.VsFeedbackGenerator.Utils
{
    internal static class TypeNameExtensions
    {
        public static string KindOfType(this ITypeName elem)
        {
            Asserts.NotNull(elem, "elem");
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
                    if (elem.IsNullableType)
                    {
                        return "nullable";
                    }
                    if (elem.IsSimpleType)
                    {
                        return "simple";
                    }
                    return "struct";
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
