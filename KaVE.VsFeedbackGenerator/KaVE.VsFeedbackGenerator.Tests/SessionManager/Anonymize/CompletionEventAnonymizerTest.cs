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
using System.Collections.Generic;
using JetBrains.Util;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names;
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
                Context = new Context
                {
                    EnclosingMethod = MethodName.Get("[R, A, 1.2.3.4] [D, P].M()"),
                    TriggerTarget = TypeName.Get("T, P"),
                    TypeShape = new TypeShape
                    {
                        TypeHierarchy = new TypeHierarchy("C, P")
                        {
                            Extends = new TypeHierarchy("S, P"),
                            Implements = new HashSet<ITypeHierarchy>
                            {
                                new TypeHierarchy("I1, P"),
                                new TypeHierarchy("I2, P")
                            }
                        },
                        MethodHierarchies = new HashSet<MethodHierarchy>
                        {
                            new MethodHierarchy(MethodName.Get("[R, A, 1.2.3.4] [D, P].N()"))
                            {
                                Super = MethodName.Get("[R, A, 1.2.3.4] [S, P].N()"),
                                First = MethodName.Get("[R, A, 1.2.3.4] [I1, P].N()")
                            },
                            new MethodHierarchy(MethodName.Get("[R, A, 4.3.2.1] [D, P].L()"))
                        }
                    },
                    EntryPointToCalledMethods = new Dictionary<IMethodName, ISet<IMethodName>>
                    {
                        {
                            MethodName.Get("[R, A, 1.2.3.4] [D, P].M()"),
                            new HashSet<IMethodName> {MethodName.Get("[R, A, 1.2.3.4] [D, P].N()")}
                        }
                    }
                }
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
        public void ShouldAnonymizeContextEnclosingMethodIfRemoveNamesIsSet()
        {
            ExportSettings.RemoveCodeNames = true;
            var expected =
                MethodName.Get(
                    "[R, A, 1.2.3.4] [BTxSgd7rLC1KLBfBSU59-w==, aUaDMpYpDqsiSh5nQjiWFw==].lNSAgClcjc9lDeUkXybdNQ==()");

            var actual = WhenEventIsAnonymized();

            Assert.AreEqual(expected, actual.Context.EnclosingMethod);
        }

        [Test]
        public void ShouldNotFailWhenEnclosingMethodIsNullAndRemoveNamesIsSet()
        {
            OriginalEvent.Context.EnclosingMethod = null;
            ExportSettings.RemoveCodeNames = true;

            var actual = WhenEventIsAnonymized();

            Assert.IsNull(actual.Context.EnclosingMethod);
        }

        [Test]
        public void ShouldAnonymizeContextTriggerTarget()
        {
            ExportSettings.RemoveCodeNames = true;
            var expected = TypeName.Get("TM6pgLI0nE5n0EEgAKIIFw==, aUaDMpYpDqsiSh5nQjiWFw==");

            var actual = WhenEventIsAnonymized();

            Assert.AreEqual(expected, actual.Context.TriggerTarget);
        }

        [Test]
        public void ShouldNotFailWhenTriggerTargetIsNullAndRemoveNamesIsSet()
        {
            OriginalEvent.Context.TriggerTarget = null;
            ExportSettings.RemoveCodeNames = true;

            var actual = WhenEventIsAnonymized();

            Assert.IsNull(actual.Context.TriggerTarget);
        }

        [Test]
        public void ShouldAnonymizeContextTypeShape()
        {
            ExportSettings.RemoveCodeNames = true;
            var expected = new TypeShape
            {
                TypeHierarchy = new TypeHierarchy("3Rx860ySZTppa3kHpN1N8Q==, aUaDMpYpDqsiSh5nQjiWFw==")
                {
                    Extends = new TypeHierarchy("bwrIwYfO24Nam6NzYDvaPw==, aUaDMpYpDqsiSh5nQjiWFw=="),
                    Implements = new HashSet<ITypeHierarchy>
                    {
                        new TypeHierarchy("eGEyMBjXL4zPn7I6S8mfDw==, aUaDMpYpDqsiSh5nQjiWFw=="),
                        new TypeHierarchy("L_ae-p4-hxBsaXczpcEyIQ==, aUaDMpYpDqsiSh5nQjiWFw==")
                    }
                },
                MethodHierarchies = new HashSet<MethodHierarchy>
                {
                    new MethodHierarchy(
                        MethodName.Get(
                            "[R, A, 1.2.3.4] [BTxSgd7rLC1KLBfBSU59-w==, aUaDMpYpDqsiSh5nQjiWFw==].FrZejHdXesK4GmGTziBKog==()"))
                    {
                        Super =
                            MethodName.Get(
                                "[R, A, 1.2.3.4] [bwrIwYfO24Nam6NzYDvaPw==, aUaDMpYpDqsiSh5nQjiWFw==].FrZejHdXesK4GmGTziBKog==()"),
                        First =
                            MethodName.Get(
                                "[R, A, 1.2.3.4] [eGEyMBjXL4zPn7I6S8mfDw==, aUaDMpYpDqsiSh5nQjiWFw==].FrZejHdXesK4GmGTziBKog==()")
                    },
                    new MethodHierarchy(
                        MethodName.Get(
                            "[R, A, 4.3.2.1] [BTxSgd7rLC1KLBfBSU59-w==, aUaDMpYpDqsiSh5nQjiWFw==].teEFVPLjq1yy_faHQwbDSg==()"))
                }
            };

            var actual = WhenEventIsAnonymized();

            Assert.AreEqual(expected, actual.Context.TypeShape);
        }

        [Test]
        public void ShouldAnonymizeEntryPointsAndCalledMethods()
        {
            ExportSettings.RemoveCodeNames = true;
            var expected = new Dictionary<IMethodName, ISet<IMethodName>>
            {
                {
                    MethodName.Get(
                        "[R, A, 1.2.3.4] [BTxSgd7rLC1KLBfBSU59-w==, aUaDMpYpDqsiSh5nQjiWFw==].lNSAgClcjc9lDeUkXybdNQ==()"),
                    new HashSet<IMethodName>
                    {
                        MethodName.Get(
                            "[R, A, 1.2.3.4] [BTxSgd7rLC1KLBfBSU59-w==, aUaDMpYpDqsiSh5nQjiWFw==].FrZejHdXesK4GmGTziBKog==()")
                    }
                }
            };

            var actual = WhenEventIsAnonymized();

            AssertAreEquivalent(expected, actual.Context.EntryPointToCalledMethods);
        }

        // TODO @Seb: Add tests for entryPointToGroum when groum implementation is done

        private static void AssertAreEquivalent(IDictionary<IMethodName, ISet<IMethodName>> expected,
            IDictionary<IMethodName, ISet<IMethodName>> actual)
        {
            CollectionAssert.AreEqual(expected.Keys, actual.Keys);
            expected.Keys.ForEach(
                key =>
                    CollectionAssert.AreEquivalent(expected[key], actual[key], "Called methods for entry point " + key));
        }

        protected override void AssertThatPropertiesThatAreNotTouchedByAnonymizationAreUnchanged(
            CompletionEvent original,
            CompletionEvent anonymized)
        {
            Assert.AreEqual(original.Prefix, anonymized.Prefix);
        }
    }
}