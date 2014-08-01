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

using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis
{
    [TestFixture]
    internal class ContextAnalysisCalledMethodsTest : CalledMethodsTestBase
    {
        [Test]
        public void ShouldFindNoCalls()
        {
            CompleteInMethod(@"
                $
            ");

            AssertNoCallsDetected();
        }

        [Test]
        public void ShouldFindCallOnTopLevel()
        {
            CompleteInFile(@"
                interface I
                {
                    void Do(int i);
                }

                class C
                {
                    public void M(I i)
                    {
                        i.Do(1);
                        $
                    }
                }");

            AssertCallDetected(
                "[System.Void, mscorlib, 4.0.0.0] [i:I, TestProject].Do([System.Int32, mscorlib, 4.0.0.0] i)");
        }

        [Test]
        public void ShouldFindCallInCondition()
        {
            CompleteInFile(@"
                interface I
                {
                    bool Is();
                }

                class C
                {
                    public void M(I i)
                    {
                        if (i.Is())
                        {
                            $
                        }
                    }
                }");

            AssertCallDetected("[System.Boolean, mscorlib, 4.0.0.0] [i:I, TestProject].Is()");
        }

        [Test]
        public void ShouldFindCallInSubBlock()
        {
            CompleteInFile(@"
                interface I
                {
                    bool Is();
                    C GetC();
                }

                class C
                {
                    public void M(I i)
                    {
                        if (i.Is())
                        {
                            i.GetC();
                        }
                        $
                    }
                }");

            AssertCallDetected("[C, TestProject] [i:I, TestProject].GetC()");
        }

        [Test]
        public void ShouldFindNestedCall()
        {
            CompleteInFile(@"
                    interface I
                    {
                        int Get();
                        void Do(int i);
                    }

                    class C
                    {
                        public void M(I i)
                        {
                            i.Do(i.Get());
                            $
                        }
                    }
                }");

            AssertCallDetected("[System.Int32, mscorlib, 4.0.0.0] [i:I, TestProject].Get()");
        }

        [Test]
        public void ShouldFindCallWithUndefinedTypeLevelTypeParameter()
        {
            CompleteInFile(@"
                interface I<TI1>
                {
                    TI1 Get();
                    TI2 Get<TI2>();
                }

                class C
                {
                    public void M<TM1>(I<TM1> i1)
                    {
                        i1.Get();
                        $
                    }
                }");

            AssertCallDetected("[TI1] [i:I`1[[TI1 -> TM1]], TestProject].Get()");
        }

        [Test]
        public void ShouldFindCallWithDefinedTypeLevelTypeParameter()
        {
            CompleteInFile(@"
                interface I<TI1>
                {
                    TI1 Get();
                }

                class C
                {
                    public void M()
                    {
                        I<string> i2 = null;
                        i2.Get();
                        $
                    }
                }");

            AssertCallDetected("[TI1] [i:I`1[[TI1 -> System.String, mscorlib, 4.0.0.0]], TestProject].Get()");
        }

        [Test]
        public void ShouldFindCallWithMethodLevelTypeParameter()
        {
            CompleteInFile(@"
                interface I<TI1>
                {
                    TI1 Get();
                    TI2 Get<TI2>();
                }

                class C
                {
                    public void M<TM1>(I<TM1> i1)
                    {
                        i1.Get<int>();
                        $
                    }
                }");

            AssertCallDetected(
                "[TI2] [i:I`1[[TI1 -> TM1]], TestProject].Get`1[[TI2 -> System.Int32, mscorlib, 4.0.0.0]]()");
        }

        [Test]
        public void ShouldFindCallOnFreeTypeParameterInstance()
        {
            CompleteInClass(@"
                public void M<TM2>(TM2 p)
                {
                    p.GetHashCode();
                    $
                }");

            AssertCallDetected("[System.Int32, mscorlib, 4.0.0.0] [System.Object, mscorlib, 4.0.0.0].GetHashCode()");
        }

        [Test(Description = "Marker: (5)")]
        public void ShouldFindCallOnTransitivlyConstraintTypeParameterInstance()
        {
            CompleteInFile(@"
                interface I<TI1>
                {
                    TI1 Get();
                }

                class D<B> : I<B>
                {
                    public B Get() { return default(B); }
                }

                class C
                {
                    public void M<TM1, TM2>(D<TM2> d) where TM2 : I<object>
                    {
                        d.Get();
                        $
                    }
                }");

            AssertCallDetected("[B] [D`1[[B -> TM2]], TestProject].Get()");
        }

        [Test]
        public void ShouldResolveAmbiguousCall()
        {
            CompleteInMethod(@"
                this.Equals();
                $
            ");

            AssertCallDetected(
                "[System.Boolean, mscorlib, 4.0.0.0] [System.Object, mscorlib, 4.0.0.0].Equals([System.Object, mscorlib, 4.0.0.0] obj)");
        }

        [Test]
        public void ShouldResolveUnresolvableCall()
        {
            CompleteInFile(@"
                class C
                {
                    public void M()
                    {
                        this.Unknown();
                        $
                    }
                }");

            AssertNoCallsDetected();
        }

        [Test]
        public void ShouldFindChainedCalls()
        {
            CompleteInMethod(@"
                object o;
                o.ToString().GetHashCode();
                $
            ");

            AssertNumberOfCalls("M", 2);
            AssertCallDetected("M", "[System.String, mscorlib, 4.0.0.0] [System.Object, mscorlib, 4.0.0.0].ToString()");
            AssertCallDetected(
                "M",
                "[System.Int32, mscorlib, 4.0.0.0] [System.Object, mscorlib, 4.0.0.0].GetHashCode()");
        }

        [Test]
        public void ShouldFindCallsAtParameterPosition()
        {
            CompleteInClass(@"
                public void M(string s) {}
                
                public void E(object o)
                {
                    M(o.ToString());
                    $
                }");

            AssertCallDetected("[System.String, mscorlib, 4.0.0.0] [System.Object, mscorlib, 4.0.0.0].ToString()");
        }

        [Test]
        public void ShouldFindCallToInterfaceImplementationInSameClass()
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

            AssertCallDetected("M", "[System.Void, mscorlib, 4.0.0.0] [i:I, TestProject].EP()");
        }
    }
}