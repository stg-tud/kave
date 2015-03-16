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
 *    - Dennis Albrecht
 */

using System;
using JetBrains.Util;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names.CSharp;
using KaVE.Model.SSTs.Impl;
using KaVE.Model.TypeShapes;
using KaVE.VsFeedbackGenerator.SessionManager.Anonymize;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.SessionManager.Anonymize.CompletionEvents
{
    internal class CompletionEventAnonymizerTest : IDEEventAnonymizerTestBase<CompletionEvent>
    {
        protected override CompletionEvent CreateEventWithAllAnonymizablePropertiesSet()
        {
            return new CompletionEvent
            {
                Prefix = "get",
                ProposalCollection = new ProposalCollection(
                    new[]
                    {
                        new Proposal {Name = TypeName.Get("MyType, EnclosingProject")},
                        new Proposal {Name = TypeName.Get("OtherType, Assembly, 1.2.3.4")},
                        new Proposal {Name = NamespaceName.Get("Some.Namepsace")}
                    }),
                Selections = new[]
                {
                    new ProposalSelection(new Proposal {Name = TypeName.Get("MyType, EnclosingProject")})
                    {
                        SelectedAfter = TimeSpan.FromSeconds(0)
                    },
                    new ProposalSelection(new Proposal {Name = TypeName.Get("OtherType, Assembly, 1.2.3.4")})
                    {
                        SelectedAfter = TimeSpan.FromSeconds(2)
                    }
                },
                Context2 = CreateSimpleContext()
            };
        }

        private static Context CreateSimpleContext()
        {
            return new Context
            {
                TypeShape = new TypeShape {TypeHierarchy = new TypeHierarchy("C, P")},
                SST = new SST {EnclosingType = TypeName.Get("T,P")}
            };
        }

        [Test]
        public void ShouldRemoveSelectionOffsetWhenRemoveDurationsIsSet()
        {
            ExportSettings.RemoveDurations = true;

            var actual = WhenEventIsAnonymized();

            actual.Selections.ForEach(selection => Assert.IsNull(selection.SelectedAfter));
        }

        [Test]
        public void ShouldAnonymizeProposalNamesWhenRemoveNamesIsSet()
        {
            ExportSettings.RemoveCodeNames = true;
            var expected = new[]
            {
                new Proposal {Name = TypeName.Get("Q-vTVCo_g8yayGGoDdH7BA==, qfFVtSOtve-XEFJXWTbfXw==")},
                new Proposal {Name = TypeName.Get("OtherType, Assembly, 1.2.3.4")},
                new Proposal {Name = NamespaceName.Get("A_SHMh611J-1vRjtIJDirA==")}
            };

            var actual = WhenEventIsAnonymized();

            CollectionAssert.AreEqual(expected, actual.ProposalCollection.Proposals);
        }

        [Test]
        public void ShouldAnonymizeProposalNamesInSelectionsWhenRemoveNamesIsSet()
        {
            ExportSettings.RemoveCodeNames = true;
            var expected = new[]
            {
                new ProposalSelection(
                    new Proposal {Name = TypeName.Get("Q-vTVCo_g8yayGGoDdH7BA==, qfFVtSOtve-XEFJXWTbfXw==")})
                {
                    SelectedAfter = TimeSpan.FromSeconds(0)
                },
                new ProposalSelection(new Proposal {Name = TypeName.Get("OtherType, Assembly, 1.2.3.4")})
                {
                    SelectedAfter = TimeSpan.FromSeconds(2)
                }
            };

            var actual = WhenEventIsAnonymized();

            CollectionAssert.AreEqual(expected, actual.Selections);
        }

        [Test]
        public void ShouldAnonymizeContext()
        {
            ExportSettings.RemoveCodeNames = true;
            var expected = new Context
            {
                TypeShape = new TypeShape
                {
                    TypeHierarchy = new TypeHierarchy("3Rx860ySZTppa3kHpN1N8Q==, aUaDMpYpDqsiSh5nQjiWFw=="),
                },
                SST = new SST
                {
                    EnclosingType = TypeName.Get("T,P").ToAnonymousName()
                }
            };

            var actual = WhenEventIsAnonymized().Context2;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldNotFailIfPropertiesAreNotSet()
        {
            ExportSettings.RemoveCodeNames = true;
            ExportSettings.RemoveDurations = true;
            ExportSettings.RemoveStartTimes = true;

            WhenEventIsAnonymized();
        }

        protected override void AssertThatPropertiesThatAreNotTouchedByAnonymizationAreUnchanged(
            CompletionEvent original,
            CompletionEvent anonymized)
        {
            Assert.AreEqual(original.Prefix, anonymized.Prefix);
        }
    }
}