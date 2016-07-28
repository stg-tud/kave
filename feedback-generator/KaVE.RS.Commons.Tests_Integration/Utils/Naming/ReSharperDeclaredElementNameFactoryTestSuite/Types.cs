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

using NUnit.Framework;
using Fix = KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;


namespace N
{
    public class C1<T1> {}

    public class C<T2>
    {
        public void M(C1<T2>[] p) {}
    }
}

namespace KaVE.RS.Commons.Tests_Integration.Utils.Naming.ReSharperDeclaredElementNameFactoryTestSuite
{
    internal class Types : NameFactoryBaseTest
    {
        [Test]
        public void Basic_Interfaces()
        {
            Assert.Fail();
        }

        [Test]
        public void Basic_Enums()
        {
            Assert.Fail();
        }

        [Test]
        public void Basic_Structs()
        {
            Assert.Fail();
        }

        [Test]
        public void Basic_Structs_BuiltIn()
        {
            Assert.Fail();
        }

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
    }
}