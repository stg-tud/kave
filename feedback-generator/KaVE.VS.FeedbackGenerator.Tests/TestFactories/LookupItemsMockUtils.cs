/*
 * Copyright 2014 Technische Universität Darmstadt
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure.LookupItems;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Impl.Resolve;
using JetBrains.UI.RichText;
using KaVE.RS.Commons.Tests_Unit.TestFactories;
using Moq;

namespace KaVE.VS.FeedbackGenerator.Tests.TestFactories
{
    internal static class LookupItemsMockUtils
    {
        private static readonly Random Random = new Random();

        private static char GetRandomUpperCaseLetter()
        {
            var offset = Random.Next(0, 26);
            return (char) ('A' + offset);
        }

        private static char GetRandomLowerCaseLetter()
        {
            var offset = Random.Next(0, 26);
            return (char) ('a' + offset);
        }

        private static string GetRandomName()
        {
            var lengthOfName = Random.Next(2, 6);
            var name = GetRandomUpperCaseLetter().ToString(CultureInfo.InvariantCulture);
            for (var i = 0; i < lengthOfName; i++)
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
            var declaredElementInstance =
                new DeclaredElementInstance(
                    TypeMockUtils.MockTypeElement(typeName, assemblyName, "1.2.3.4"),
                    new SubstitutionImpl());
            lookupItem.Setup(i => i.PreferredDeclaredElement).Returns(declaredElementInstance);
            lookupItem.Setup(i => i.DisplayName).Returns(new RichText(GetRandomName()));
            return lookupItem.Object;
        }
    }
}