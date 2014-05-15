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

using KaVE.Model.Names.CSharp;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis
{
    [TestFixture]
    class EntryPointDetectionTest : KaVEBaseTest
    {
        [Test]
        public void ShouldUseInterfaceImplementationAsEntryPoint()
        {
            CompleteInFile(@"
                interface I
                {
                    void EP();
                }

                class C : I
                {
                    public void EP()
                    {
                        $
                    }
                }");

            AssertIsEntryPoint("[System.Void, mscorlib, 4.0.0.0] [C, TestProject].EP()");
        }

        [Test]
        public void ShouldUseOverrideAsEntryPoint()
        {
            CompleteInFile(@"
                abstract class A
                {
                    public abstract void EP();
                }

                class C : A
                {
                    public override void EP()
                    {
                        $
                    }
                }");

            AssertIsEntryPoint("[System.Void, mscorlib, 4.0.0.0] [C, TestProject].EP()");
        }

        [Test]
        public void ShouldNotUseAbstractImplementationAsEntryPoint()
        {
            CompleteInFile(@"
                interface I
                {
                    void EP();
                }

                abstract class C : I
                {
                    public abstract void EP();

                    public void M()
                    {
                        $
                    }
                }");

            AssertIsNotEntryPoint("[System.Void, mscorlib, 4.0.0.0] [C, TestProject].EP()");
        }

        [Test]
        public void ShouldUseMethodAsEntryPointIfItIsNotCalledFromOtherMethod()
        {
            CompleteInFile(@"
                class C
                {
                    public void EP()
                    {
                        $
                    }
                }
            ");

            AssertIsEntryPoint("[System.Void, mscorlib, 4.0.0.0] [C, TestProject].EP()");
        }

        [Test]
        public void ShouldNotUseMethodAsEntryPointIfItIsCalledFromOtherEntryPoint()
        {
            CompleteInFile(@"
                interface I
                {
                    void EP();
                }

                class C : I
                {
                    public void EP()
                    {
                        M();
                    }

                    public void M()
                    {
                        $
                    }
                }");

            AssertIsNotEntryPoint("[System.Void, mscorlib, 4.0.0.0] [C, TestProject].M()");
        }

        [Test]
        public void ShouldNotUseMethodAsEntryPointIfItIsCalledFromOtherMethod()
        {
            CompleteInFile(@"
                class C
                {
                    public void M()
                    {
                        $
                    }

                    public void EP()
                    {
                        M();
                    }
                }");

            AssertIsNotEntryPoint("[System.Void, mscorlib, 4.0.0.0] [C, TestProject].M()");
        }

        [Test]
        public void ShouldUseImplementationAsEntryPointEvenIfItIsCalledFromOtherMethod()
        {
            CompleteInFile(@"
                interface I
                {
                    void EP();
                }

                class C : I
                {
                    public void EP() { }

                    public void M()
                    {
                        EP();
                        $
                    }
                }");

            AssertIsEntryPoint("[System.Void, mscorlib, 4.0.0.0] [C, TestProject].EP()");
        }

        private void AssertIsEntryPoint(string systemVoidMscorlibCTestprojectEp)
        {
            CollectionAssert.Contains(ResultContext.EntryPoints, MethodName.Get(systemVoidMscorlibCTestprojectEp));
        }

        private void AssertIsNotEntryPoint(string systemVoidMscorlibCTestprojectEp)
        {
            CollectionAssert.DoesNotContain(ResultContext.EntryPoints, MethodName.Get(systemVoidMscorlibCTestprojectEp));
        }
    }
}
