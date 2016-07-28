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

using KaVE.Commons.Utils;
using NUnit.Framework;
using Fix = KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.RS.Commons.Tests_Integration.Utils.Naming.ReSharperDeclaredElementNameFactoryTestSuite
{
    internal class Types : NameFactoryBaseTest
    {
        #region basic cases

        [Test]
        public void Arrays()
        {
            CompleteInNamespace(@"
                public class C
                {
                    public void M(int[] ns) { $ }
                }
            ");

            AssertParameterTypes(Fix.IntArray.Identifier);
        }

        [Test]
        public void Delegates()
        {
            CompleteInNamespace(@"
                public class C
                {
                    public delegate void D(int i);
                    public void M(D d) { $ }
                }
            ");

            var delType = "d:[{0}] [N.C+D, TestProject].([{1}] i)".FormatEx(Fix.Void, Fix.Int);
            AssertParameterTypes(delType);
        }

        [Test]
        public void Delegates_BuiltIn()
        {
            CompleteInNamespace(@"
                public class C
                {
                    public void M(Action<int> a) { $ }
                }
            ");

            var id = "d:[{0}] [System.Action`1[[T -> {1}]], mscorlib, 4.0.0.0].([T] obj)".FormatEx(Fix.Void, Fix.Int);
            AssertParameterTypes(id);
        }

        [Test]
        public void Basic_Enums()
        {
            CompleteInNamespace(@"
                public enum E {}
                public class C
                {
                    public void M(E d) { $ }
                }
            ");

            AssertParameterTypes("e:N.E, TestProject");
        }

        [Test]
        public void Basic_Interfaces()
        {
            CompleteInNamespace(@"
                public interface I {}
                public class C
                {
                    public void M(I i) { $ }
                }
            ");

            AssertParameterTypes("i:N.I, TestProject");
        }

        [Test]
        public void Basic_Structs()
        {
            CompleteInNamespace(@"
                public struct S {}
                public class C
                {
                    public void M(S s) { $ }
                }
            ");

            AssertParameterTypes("s:N.S, TestProject");
        }

        [Test, Ignore("Special handling for built-in types is inconsistent, but ignored for now")]
        public void Basic_Structs_BuiltIn()
        {
            CompleteInNamespace(@"
                public class C
                {
                    public void M(int i) { $ }
                }
            ");

            AssertParameterTypes("s:System.Int32, mscorlib, 4.0.0.0");
        }

        #endregion

        #region generics

        [Test]
        public void Generics_Free()
        {
            CompleteInNamespace(@"
                class C<G1>
                {
                    public void M()
                    {
                        $
                    }
                }
            ");

            AssertSingleMethodName("[{0}] [N.C`1[[G1]], TestProject].M()", Fix.Void);
        }

        [Test]
        public void Generics_Bound_Type()
        {
            CompleteInNamespace(@"
                class G<G1> {}
                class C
                {
                    public void M(G<int> p)
                    {
                        $
                    }
                }
            ");

            AssertSingleMethodName("[{0}] [N.C, TestProject].M([N.G`1[[G1 -> {1}]], TestProject] p)", Fix.Void, Fix.Int);
        }

        [Test]
        public void Generics_Bound_Unknown()
        {
            CompleteInNamespace(@"
                class G<G1> {}
                class C
                {
                    public void M(G<XYZ> p)
                    {
                        $
                    }
                }
            ");

            AssertSingleMethodName("[{0}] [N.C, TestProject].M([N.G`1[[G1 -> ?]], TestProject] p)", Fix.Void);
        }

        [Test]
        public void Generics_Bound_TypeParameter()
        {
            CompleteInNamespace(@"
                class G<G1> {}
                class C<G2>
                {
                    public void M(G<G2> p)
                    {
                        $
                    }
                }
            ");

            AssertSingleMethodName("[{0}] [N.C`1[[G2]], TestProject].M([N.G`1[[G1 -> G2]], TestProject] p)", Fix.Void);
        }

        [Test]
        public void Generics_Bound_TypeParameterWithSameName()
        {
            CompleteInNamespace(@"
                class G<G1> {}
                class C<G1>
                {
                    public void M(G<G1> p)
                    {
                        $
                    }
                }
            ");

            AssertSingleMethodName("[{0}] [N.C`1[[G1]], TestProject].M([N.G`1[[G1 -> G1]], TestProject] p)", Fix.Void);
        }

        [Test]
        public void Generics_Combination()
        {
            CompleteInNamespace(@"
                class G<G1> {}
                class C<G2>
                {
                    public void M(G2 p1, G<G2> p2, G<int> p3)
                    {
                        $
                    }
                }
            ");

            AssertSingleMethodName(
                "[{0}] [N.C`1[[G2]], TestProject].M([G2] p1, [N.G`1[[G1 -> G2]], TestProject] p2, [N.G`1[[G1 -> {1}]], TestProject] p3)",
                Fix.Void,
                Fix.Int);
        }

        [Test]
        public void Regression_WildCombination()
        {
            CompleteInCSharpFile(@"
                namespace N
                {
                    class Outer<T0>
                    {
                        public class C1<T1>
                        {
                            public class C2<T2> {}
                        }

                        public class C
                        {
                            public void M(C1<int>.C2<int> p)
                            {
                                $
                            }
                        }
                    }
                }
            ");

            AssertParameterTypes("N.Outer`1[[T0]]+C1`1[[T1 -> {0}]]+C2`1[[T2 -> {0}]], TestProject".FormatEx(Fix.Int));
        }

        #endregion
    }
}