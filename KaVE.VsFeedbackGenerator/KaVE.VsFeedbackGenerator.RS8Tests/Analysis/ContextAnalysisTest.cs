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
using System.Collections.Generic;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names.CSharp;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis
{
    // TODO check if any of these tests are still relevant and either move or remove them
    [TestFixture]
    internal class ContextAnalysisTest : BaseTest
    {
        [Test]
        public void ShouldRetrieveContext()
        {
            WhenCodeCompletionIsInvokedInFile("ProofOfConcept");
            Assert.IsNotNull(ResultContext);
        }

        [Test]
        public void ShouldRetrieveCorrectEnclosingMethodDeclaration()
        {
            WhenCodeCompletionIsInvokedInFile("ProofOfConcept");

            var actual = ResultContext.EnclosingMethod;
            var expected =
                MethodName.Get("[System.Void, mscorlib, 4.0.0.0] [TestNamespace.TestClass, TestProject].Doit()");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldNotContainObjectInTypeHierarchy()
        {
            WhenCodeCompletionIsInvokedInFile("ProofOfConcept");
            Assert.IsNull(ResultContext.TypeShape.TypeHierarchy.Extends);
        }

        [Test]
        public void ShouldRetrieveEnclosingType()
        {
            WhenCodeCompletionIsInvokedInFile("TypeHierarchy");
            var actual = ResultContext.TypeShape.TypeHierarchy.Element;

            var expected = TypeName.Get("TestNamespace.TestClass, TestProject");

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldRetrieveSuperType()
        {
            WhenCodeCompletionIsInvokedInFile("TypeHierarchy");
            var actual = ResultContext.TypeShape.TypeHierarchy.Extends.Element;

            var expected = TypeName.Get("TestNamespace.SuperClass, TestProject");

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldRetrieveImplementedInterfaces()
        {
            WhenCodeCompletionIsInvokedInFile("TypeHierarchy");
            var actual = ResultContext.TypeShape.TypeHierarchy.Implements;

            var expected = new HashSet<ITypeHierarchy>
            {
                new TypeHierarchy("i:TestNamespace.AnInterface, TestProject")
            };

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldRetrieveSubstitution()
        {
            WhenCodeCompletionIsInvokedInFile("GenericTypeHierarchy");

            var actual = ResultContext.TypeShape.TypeHierarchy.Extends.Element;
            var expected = TypeName.Get("N.IC`1[[T -> System.Int32, mscorlib, 4.0.0.0]], TestProject");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldRetrieveFreeSubstitution()
        {
            WhenCodeCompletionIsInvokedInFile("GenericTypeHierarchy");

            var actual = ResultContext.TypeShape.TypeHierarchy.Element;
            var expected = TypeName.Get("N.C`1[[T]], TestProject");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldRetrieveLargeHierarchy()
        {
            WhenCodeCompletionIsInvokedInFile("LargeHierarchy");
            var actual = ResultContext.TypeShape.TypeHierarchy;
            var expected = new TypeHierarchy("N.B, TestProject")
            {
                Extends = new TypeHierarchy("N.A, TestProject")
                {
                    Implements = new HashSet<ITypeHierarchy>
                    {
                        new TypeHierarchy("i:N.IA, TestProject")
                        {
                            Implements = new HashSet<ITypeHierarchy>
                            {
                                new TypeHierarchy("i:N.I0, TestProject")
                            }
                        }
                    }
                },
                Implements = new HashSet<ITypeHierarchy>
                {
                    new TypeHierarchy("i:N.IB`1[[TB -> System.Int32, mscorlib, 4.0.0.0]], TestProject"),
                    new TypeHierarchy("i:N.IC, TestProject")
                }
            };
            Assert.AreEqual(expected, actual);
        }
    }
}