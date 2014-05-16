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

using KaVE.JetBrains.Annotations;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis
{
    [TestFixture]
    internal class ContextAnalysisObjectTrackingTest : CalledMethodsTestBase
    {
        [UsedImplicitly]
        private static readonly string[] Visibilities = {"public", "protected", "internal", "private"};

        [TestCaseSource("Visibilities")]
        public void ShouldFindCallInLocalMethod(string helperVisibility)
        {
            CompleteInClass(@"
                public void M1(object o) {
                    this.M2(o);
                    $
                }
        
                " + helperVisibility + @" void M2(object o) {
                    o.GetHashCode();
                }");

            AssertCallDetected("[System.Int32, mscorlib, 4.0.0.0] [System.Object, mscorlib, 4.0.0.0].GetHashCode()");
        }

        [TestCaseSource("Visibilities")]
        public void ShouldNotFindCallToLocalMethod(string helperVisibility)
        {
            CompleteInFile(@"
                class C {
                    public void M1(object o) {
                        this.M2(o);
                        $
                    }
        
                    " + helperVisibility + @" void M2(object o) {}
                }");

            AssertNoCallDetected(
                "[System.Void, mscorlib, 4.0.0.0] [C, TestProject].M2([System.Object, mscorlib, 4.0.0.0] o)");
        }

        [Test, Ignore("currently we cannot detect this from the analysis result")]
        public void ShouldIncludeHelpersMultipleTimes()
        {
            CompleteInClass(@"
                public override void M1(object o) { H(o); }
                public override void M2(object o) { H(o); }
                
                private void H(object o)
                {
                    o.GetHashCode();
                }");
            Assert.Fail();
        }

        [Test]
        public void ShouldFindCallToMethodDeclaredBySupertype()
        {
            CompleteInFile(@"
                public interface I
                {
                    void M(object o);
                }

                public class C : I
                {
                    public override void M(object o) {}

                    public void E(object o)
                    {
                        M(o);
                        $
                    }
                }");

            AssertCallDetected(
                "E",
                "[System.Void, mscorlib, 4.0.0.0] [i:I, TestProject].M([System.Object, mscorlib, 4.0.0.0] o)");
        }

        [Test]
        public void ShouldNotFindCallInMethodDeclaredBySupertype()
        {
            CompleteInFile(@"
                public interface I
                {
                    void M(object o);
                }

                public class C : I
                {
                    public override void M(object o)
                    {
                        o.GetHashCode();
                    }

                    public void E(object o)
                    {
                        M(o);
                        $
                    }
                }");

            AssertNoCallDetected(
                "E",
                "[System.Int32, mscorlib, 4.0.0.0] [System.Object, mscorlib, 4.0.0.0].GetHashCode()");
        }

        [Test]
        public void ShouldFindCallToAbstractLocalMethod()
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

            AssertCallDetected(
                "[System.Void, mscorlib, 4.0.0.0] [A, TestProject].M([System.Object, mscorlib, 4.0.0.0] o)");
        }

        [Test]
        public void ShouldFindCallToMethodFromOtherClass()
        {
            CompleteInFile(@"
                public class HelperClass
                {
                    public void M() {}
                }
                
                public class C
                {
                    public void E(HelperClass h)
                    {
                        h.M();
                        $
                    }
                }");

            AssertCallDetected("[System.Void, mscorlib, 4.0.0.0] [HelperClass, TestProject].M()");
        }

        [Test]
        public void ShouldFindCallToMethodFromOtherClassByItsFirstDeclaration()
        {
            CompleteInFile(@"
                public interface I
                {
                    void M();
                }

                public class HelperClass : I
                {
                    public override void M() {}
                }
                
                public class C
                {
                    public void E(HelperClass h)
                    {
                        h.M();
                        $
                    }
                }");

            AssertCallDetected("[System.Void, mscorlib, 4.0.0.0] [i:I, TestProject].M()");
        }
    }
}