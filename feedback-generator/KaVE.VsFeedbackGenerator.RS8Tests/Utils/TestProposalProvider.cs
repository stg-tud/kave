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
 * 
 * Contributors:
 *    - Sven Amann
 */

using System;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure.LookupItems;
using JetBrains.ReSharper.Feature.Services.CSharp.CodeCompletion.Infrastructure;
using JetBrains.ReSharper.Features.Intellisense.CodeCompletion.CSharp.Rules;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Xaml.DeclaredElements;
using JetBrains.UI.RichText;
using Moq;

namespace KaVE.ReSharper.Commons.Tests_Integration.Utils
{
    [Language(typeof (CSharpLanguage))]
    public class TestProposalProvider : CSharpItemsProviderBasic
    {
        public static Boolean Enabled { get; set; }

        protected override bool IsAvailable(CSharpCodeCompletionContext context)
        {
            return Enabled;
        }

        protected override bool AddLookupItems(CSharpCodeCompletionContext context, GroupedItemsCollector collector)
        {
            // TODO RS9: was "AddToTop"
            collector.Add(CreateXamlLookupItem());
            return true;
        }

        private static IDeclaredElementLookupItem CreateXamlLookupItem()
        {
            var xamlDeclaredElement = new Mock<IXamlDeclaredElement>();
            var xamlDeclaredElementInstance = new DeclaredElementInstance(xamlDeclaredElement.Object);
            var xamlLookupItem = new Mock<IDeclaredElementLookupItem>();
            xamlLookupItem.Setup(li => li.DisplayName).Returns(new RichText("XamlLookupItem"));
            xamlLookupItem.Setup(li => li.PreferredDeclaredElement).Returns(xamlDeclaredElementInstance);
            return xamlLookupItem.Object;
        }
    }
}