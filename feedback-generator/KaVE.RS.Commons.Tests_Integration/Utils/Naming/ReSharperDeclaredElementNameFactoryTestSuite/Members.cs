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

namespace KaVE.RS.Commons.Tests_Integration.Utils.Naming.ReSharperDeclaredElementNameFactoryTestSuite
{
    internal class Members : NameFactoryBaseTest
    {
        [Test]
        public void GenericMethod()
        {
            CompleteInNamespace(@"
                public class C
                {
                    public void M<T>(T p) { $ }
                }
            ");
            AssertSingleMethodName("[{0}] [N.C, TestProject].M`1[[T]]([T] p)", Fix.Void);
        }

        [Test]
        public void GenericMethodTwoParameters()
        {
            CompleteInNamespace(@"
                public class C
                {
                    public void M<T,U>(T p1, U p2) { $ }
                }
            ");
            AssertSingleMethodName("[{0}] [N.C, TestProject].M`2[[T],[U]]([T] p1, [U] p2)", Fix.Void);
        }

        [Test]
        public void GenericMethodInNestedClass()
        {
            CompleteInNamespace(@"
                public class C 
                {
                    public class N
                    {
                        public void M<T>(T p) { $ }
                    }
                }
            ");
            AssertSingleMethodName("[{0}] [N.C+N, TestProject].M`1[[T]]([T] p)", Fix.Void);
        }
    }
}