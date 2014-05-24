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
 *    - Sebastian Proksch
 */

using System.Linq;
using KaVE.Model.Names.CSharp;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis
{
    [TestFixture]
    internal class EntryPointSelectorTest : KaVEBaseTest
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

            AssertEntryPoints("C.EP");
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

            AssertEntryPoints("C.EP");
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

            AssertEntryPoints("C.M");
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

            AssertEntryPoints("C.EP");
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

            AssertEntryPoints("C.EP");
        }

        [Test]
        public void ShouldNotUseMethodAsEntryPointIfItIsCalledFromOtherMethod()
        {
            CompleteInFile(@"
                class C
                {
                    public void M1()
                    {
                        $
                    }

                    public void M2()
                    {
                        M1();
                    }
                }");

            AssertEntryPoints("C.M2");
        }

        [Test]
        public void ShouldNotUseMethodAsEntryPointIfItIsCalledFromOtherMethodEvenForNestedCalls()
        {
            CompleteInFile(@"
                class C
                {
                    public void M1()
                    {
                        M2();
                    }

                    public void M2()
                    {
                        $
                    }

                    public void EP()
                    {
                        M1();
                    }
                }");

            AssertEntryPoints("C.EP");
        }

        [Test]
        public void SimpleRecursion()
        {
            CompleteInFile(@"
                class C
                {
                    public void EP()
                    {
                        EP();
                        $
                    }
                }");

            AssertEntryPoints("C.EP");
        }


        [Test]
        public void TransitiveRecursion()
        {
            CompleteInFile(@"
                class C
                {
                    private void M()
                    {
                        EP();
                    }
                    public void EP()
                    {
                        M();
                        $
                    }
                }");

            // Assumption is that this does not happen that often. We decided to remove both methods to avoid further hassles
            AssertEntryPoints();
        }

        [Test]
        public void MutualRecursion()
        {
            CompleteInFile(@"
                class C
                {
                    public void M1()
                    {
                        M2();
                    }
                    public void M2()
                    {
                        M1();
                        $
                   }
                }");

            // Assumption is that this does not happen that often. We decided to remove both methods to avoid further hassles
            AssertEntryPoints();
        }

        [Test]
        public void PrivateMethodsShouldNeverBeEntryPoints()
        {
            CompleteInFile(@"
                class C
                {
                    private void M()
                    {
                        $
                    }
                }");

            AssertEntryPoints();
        }

        [Test]
        public void InternalMethodsShouldNeverBeEntryPoints()
        {
            CompleteInFile(@"
                class C
                {
                    internal void M()
                    {
                        $
                    }
                }");

            AssertEntryPoints();
        }

        [Test]
        public void ProtectedMethodsShouldBeEntryPoints()
        {
            CompleteInFile(@"
                class C
                {
                    protected void M()
                    {
                        $
                    }
                }");

            AssertEntryPoints("C.M");
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

            AssertEntryPoints("C.EP", "C.M");
        }

        [Test]
        public void Bug_UnresolvedDeclarationsAreNotAnIssue()
        {
            CompleteInFile(@"
                class C {
                    public void M(string s) {}
                
                    public void E(object o)
                    {
                        M(o.ToString());
                        $
                    }
                }");

            AssertNumberOfEntryPoints(1);
            AssertEntryPoint(
                "[System.Void, mscorlib, 4.0.0.0] [C, TestProject].E([System.Object, mscorlib, 4.0.0.0] o)");
        }

        [Test]
        public void Bug_AbstractDeclarationHaveNoBody()
        {
            CompleteInFile(@"
                abstract class A
                {
                    public abstract void M(object o);

                    public void E(object o)
                    {
                        M(o);
                        $
                    }
                }");

            AssertNumberOfEntryPoints(1);
            AssertEntryPoint(
                "[System.Void, mscorlib, 4.0.0.0] [A, TestProject].E([System.Object, mscorlib, 4.0.0.0] o)");
        }

        [Test]
        public void Bug_IncompleteMemberDefinition()
        {
            CompleteInClass(@"
                public void M$
            ");
            AssertNumberOfEntryPoints(0);
        }

        public void AssertEntryPoints(params string[] entryPointNames)
        {
            AssertNumberOfEntryPoints(entryPointNames.Length);

            foreach (string epName in entryPointNames)
            {
                var epParts = epName.Split('.');
                var ep = "[System.Void, mscorlib, 4.0.0.0] [" + epParts[0] + ", TestProject]." + epParts[1] + "()";
                AssertEntryPoint(ep);
            }
        }

        private void AssertNumberOfEntryPoints(int num)
        {
            Assert.AreEqual(num, AnalyzedEntryPoints.Count());
        }

        private void AssertEntryPoint(string ep)
        {
            CollectionAssert.Contains(AnalyzedEntryPoints, MethodName.Get(ep));
        }
    }
}