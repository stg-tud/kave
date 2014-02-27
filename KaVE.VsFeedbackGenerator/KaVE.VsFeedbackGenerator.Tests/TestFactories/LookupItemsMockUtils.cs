using System;
using System.Collections.Generic;
using System.Globalization;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Impl.Resolve;
using Moq;

namespace KaVE.VsFeedbackGenerator.Tests.TestFactories
{
    static class LookupItemsMockUtils
    {
        private static readonly Random Random = new Random();

        private static char GetRandomUpperCaseLetter()
        {
            var offset = Random.Next(0, 26);
            return (char)('A' + offset);
        }

        private static char GetRandomLowerCaseLetter()
        {
            var offset = Random.Next(0, 26);
            return (char)('a' + offset);
        }

        private static string GetRandomName()
        {
            var lengthOfName = Random.Next(2, 6);
            var name = GetRandomUpperCaseLetter().ToString(CultureInfo.InvariantCulture);
            for (int i = 0; i < lengthOfName; i++)
            {
                name += GetRandomLowerCaseLetter();
            }
            return name;
        }

        public static IList<ILookupItem> MockLookupItemList(int numberOfItems)
        {
            IList<ILookupItem> result = new List<ILookupItem>();
            for (var i = 0; i < numberOfItems; i++)
            {
                result.Add(MockLookupItem());
            }
            return result;
        }

        private static ILookupItem MockLookupItem()
        {
            var lookupItem = new Mock<IDeclaredElementLookupItem>();
            var typeName = GetRandomName();
            var assemblyName = GetRandomName();
            var declaredElementInstance = new DeclaredElementInstance(TypeMockUtils.MockTypeElement(typeName, assemblyName, "1.2.3.4"), new SubstitutionImpl());
            lookupItem.Setup(i => i.PreferredDeclaredElement).Returns(declaredElementInstance);
            return lookupItem.Object;
        }
    }
}