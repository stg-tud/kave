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
using JetBrains.Util;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names.CSharp;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.SessionManager.Anonymize
{
    [TestFixture]
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
                        new Proposal {Name = TypeName.Get("OtherType, Assembly, 1.2.3.4")}
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
                Context = new Context()
            };
        }

        [Test]
        public void ShouldRemoveSelectionOffsetWhenRemoveDurationsIsSet()
        {
            ExportSettings.RemoveDurations = true;

            var actual = WhenEventIsAnonymized();

            actual.Selections.ForEach(selection => Assert.IsNull(selection.SelectedAfter));
        }

        protected override void AssertThatPropertiesThatAreNotTouchedByAnonymizationAreUnchanged(
            CompletionEvent original,
            CompletionEvent anonymized)
        {
            Assert.AreEqual(original.Prefix, anonymized.Prefix);
        }
    }
}