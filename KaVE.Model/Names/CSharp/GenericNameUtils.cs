using System.Collections.Generic;

namespace KaVE.Model.Names.CSharp
{
    internal static class GenericNameUtils
    {
        /// <summary>
        ///     Parses the type parameter list from a type's full name or a method's signature.
        /// </summary>
        /// <param name="fullNameOrSignature">Expected to contain a type-parameter list.</param>
        public static List<ITypeName> ParseTypeParameters(this string fullNameOrSignature)
        {
            var parameters = new List<ITypeName>();
            var openBraces = 0;
            var startIndex = fullNameOrSignature.IndexOf('[') + 1;
            for (var currentIndex = startIndex; currentIndex < fullNameOrSignature.Length; currentIndex++)
            {
                var c = fullNameOrSignature[currentIndex];

                if (c == '[')
                {
                    openBraces++;
                }
                else if (c == ']')
                {
                    openBraces--;

                    if (openBraces == 0)
                    {
                        var indexAfterOpeningBrace = startIndex + 1;
                        var lengthToBeforeClosingBrace = currentIndex - startIndex - 1;
                        var descriptor = fullNameOrSignature.Substring(
                            indexAfterOpeningBrace,
                            lengthToBeforeClosingBrace);
                        var parameterTypeName = TypeName.Get(descriptor);
                        parameters.Add(parameterTypeName);
                        startIndex = fullNameOrSignature.IndexOf('[', currentIndex);
                    }
                }
            }
            return parameters;
        }
    }
}