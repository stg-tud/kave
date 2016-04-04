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
using JetBrains.Util;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.TypeShapes;
using NUnit.Framework;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.TypeShapeAnalysisTestSuite
{
    internal class MethodHierarchyTest : BaseCSharpCodeCompletionTest
    {
        [Test]
        public void NoSuperMembers()
        {
            CompleteInCSharpFile(@"
                namespace N
                {
                    class C
                    {
                        public void M()
                        {
                            $
                        }
                    }
                }");

            var actual = ResultContext.TypeShape.MethodHierarchies;
            var expected = new HashSet<MethodHierarchy>
            {
                Decl("N.C", null, null)
            };

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void SuperClass()
        {
            CompleteInCSharpFile(@"
                namespace N
                {
                    class C2 
                    {
                        public virtual void M() { }
                    }

                    class C1 : C2
                    {
                        public override void M()
                        {
                            $
                        }
                    }
                }");

            var actual = ResultContext.TypeShape.MethodHierarchies;
            var expected = new HashSet<MethodHierarchy>
            {
                Decl("N.C1", "N.C2", null)
            };

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void FirstInterface()
        {
            CompleteInCSharpFile(@"
                namespace N
                {
                    interface I
                    {
                        void M();
                    }

                    class C : I
                    {
                        public void M()
                        {
                            $
                        }
                    }
                }
            ");

            var actual = ResultContext.TypeShape.MethodHierarchies;
            var expected = new HashSet<MethodHierarchy>
            {
                Decl("N.C", null, "i:N.I")
            };

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void EquivalentSupermembers_InterfaceBeforeClass()
        {
            CompleteInCSharpFile(@"
                namespace N
                {
                    interface I
                    {
                        void M();
                    }

                    public class S
                    {
                        public virtual void M() { }
                    }

                    class C : S, I
                    {
                        public override void M() { $ }
                    }
                }");

            var actual = ResultContext.TypeShape.MethodHierarchies;
            var expected = new HashSet<MethodHierarchy>
            {
                Decl("N.C", null, "i:N.I")
            };

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void EquivalentSupermembers_TwoInterfaces1()
        {
            CompleteInCSharpFile(@"
                namespace N
                {
                    interface I1
                    {
                        void M();
                    }

                    interface I2
                    {
                        void M();
                    }

                    class C : I1, I2
                    {
                        public void M() { $ }
                    }
                }");

            var actual = ResultContext.TypeShape.MethodHierarchies;
            var expected = new HashSet<MethodHierarchy>
            {
                Decl("N.C", null, "i:N.I1")
            };

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void EquivalentSupermembers_TwoInterfaces2()
        {
            CompleteInCSharpFile(@"
                namespace N
                {
                    interface I1
                    {
                        void M();
                    }

                    interface I2
                    {
                        void M();
                    }

                    class C : I2, I1
                    {
                        public void M() { $ }
                    }
                }");

            var actual = ResultContext.TypeShape.MethodHierarchies;
            var expected = new HashSet<MethodHierarchy>
            {
                Decl("N.C", null, "i:N.I2")
            };

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void EquivalentSupermembers_ThreeInterfaces1()
        {
            CompleteInCSharpFile(@"
                namespace N
                {
                    interface I1
                    {
                        void M();
                    }

                    interface I2
                    {
                        void M();
                    }

                    interface I3 : I2 { }

                    class C : I3, I1
                    {
                        public void M() { $ }
                    }
                }");

            var actual = ResultContext.TypeShape.MethodHierarchies;
            var expected = new HashSet<MethodHierarchy>
            {
                Decl("N.C", null, "i:N.I2")
            };

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void EquivalentSupermembers_ThreeInterfaces2()
        {
            CompleteInCSharpFile(@"
                namespace N
                {
                    interface I1
                    {
                        void M();
                    }

                    interface I2
                    {
                        void M();
                    }

                    interface I3 : I2 { }

                    class C : I1, I3
                    {
                        public void M() { $ }
                    }
                }");

            var actual = ResultContext.TypeShape.MethodHierarchies;
            var expected = new HashSet<MethodHierarchy>
            {
                Decl("N.C", null, "i:N.I1")
            };

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void EquivalentSupermembers_ClassAndInterfaceBeforeInterface()
        {
            CompleteInCSharpFile(@"
                namespace N
                {
                    interface I1
                    {
                        void M();
                    }

                    public class S : I1
                    {
                        public virtual void M() { }
                    }

                    interface I2
                    {
                        void M();
                    }

                    class C : S, I2
                    {
                        public override void M() { $ }
                    }
                }");

            var actual = ResultContext.TypeShape.MethodHierarchies;
            var expected = new HashSet<MethodHierarchy>
            {
                Decl("N.C", "N.S", "i:N.I1")
            };

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void EquivalentSupermembers_InterfaceBeforeClass2()
        {
            CompleteInCSharpFile(@"
                namespace N
                {
                    interface I
                    {
                        void M();
                    }

                    public class S1
                    {
                        public virtual void M() { }
                    }

                    public class S2 : S1
                    {
                        public override void M() { }
                    }

                    class C : S2, I
                    {
                        public override void M() { $ }
                    }
                }");

            var actual = ResultContext.TypeShape.MethodHierarchies;
            var expected = new HashSet<MethodHierarchy>
            {
                Decl("N.C", null, "i:N.I")
            };

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void SuperImplementationsAreNotTakenIntoAccount()
        {
            CompleteInCSharpFile(@"
                namespace N
                {
                    class C1
                    {
                        virtual void M() {}
                    }

                    class C2 : C1
                    {
                        override void M() {}
                    }

                    class C3 : C2
                    {
                        $
                    }
                }               
            ");

            var actual = ResultContext.TypeShape.MethodHierarchies;
            // ReSharper disable once CollectionNeverUpdated.Local
            var expected = new HashSet<MethodHierarchy>();

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void SuperClassAndFirstInterface()
        {
            CompleteInCSharpFile(@"
                namespace N
                {
                    public interface I
                    {
                        void M();
                    }

                    public class C2 : I
                    {
                        public virtual void M() {}
                    }

                    public class C1 : C2
                    {
                        public override void M() {}
                        $
                    }
                }
            ");

            var actual = ResultContext.TypeShape.MethodHierarchies;
            var expected = new HashSet<MethodHierarchy>
            {
                Decl("N.C1", "N.C2", "i:N.I")
            };

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void MoreComplexHierarchy_Overloads()
        {
            CompleteInCSharpFile(@"
                namespace N
                {
                    public interface I
                    {
                        void M();
                    }

                    public class S : I
                    {
                        public virtual void M() {}
                        public virtual void M(string p) {}
                    }

                    public class C : S
                    {
                        public override void M() {}
                        $
                    }
                }
            ");

            var actual = ResultContext.TypeShape.MethodHierarchies;
            var expected = new HashSet<MethodHierarchy>
            {
                Decl("N.C", "N.S", "i:N.I")
            };

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void VeryLongHierarchy()
        {
            CompleteInCSharpFile(@"
                namespace N
                {
                    public interface I
                    {
                        void M();
                    }

                    public abstract class S1 : I 
                    {
                        public virtual void M() {}
                    }

                    public class S2 : S1 { }

                    public class S3 : S2 
                    {
                        public override void M() {} 
                    }

                    public class S4 : S3 { }

                    public class C : S4
                    {
                        public override void M() {}
                        $
                    }
                }
            ");

            var actual = ResultContext.TypeShape.MethodHierarchies;
            var expected = new HashSet<MethodHierarchy>
            {
                Decl("N.C", "N.S3", "i:N.I")
            };

            CollectionAssert.AreEquivalent(expected, actual);
        }

        
        [Test]
        public void MoreComplexHierarchy_UnboundGenerics()
        {
            CompleteInCSharpFile(@"
                namespace N
                {
                    public interface I<V>
                    {
                        void M(V p);
                    }

                    public class S<U> : I<U>
                    {
                        public virtual void M(U p) {}
                    }

                    public class C<T> : S<T>
                    {
                        public override void M(T p) {}
                        $
                    }
                }
            ");

            var actual = ResultContext.TypeShape.MethodHierarchies;
            var expected = new HashSet<MethodHierarchy>
            {
                CompleteDecl("[System.Void, mscorlib, 4.0.0.0] [N.C`1[[T -> T]], TestProject].M([T] p)", 
                    "[System.Void, mscorlib, 4.0.0.0] [N.S`1[[U -> T]], TestProject].M([U] p)", 
                    "[System.Void, mscorlib, 4.0.0.0] [i:N.I`1[[V -> T]], TestProject].M([V] p)")
            };

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void MoreComplexHierarchy_BoundGenerics()
        {
            CompleteInCSharpFile(@"
                namespace N
                {
                    public interface I<U>
                    {
                        void M(U p);
                    }

                    public class S<T> : I<T>
                    {
                        public virtual void M(T p) {}
                    }

                    public class C : S<string>
                    {
                        public override void M(string p) {}
                        $
                    }
                }
            ");

            var actual = ResultContext.TypeShape.MethodHierarchies;
            var expected = new HashSet<MethodHierarchy>
            {
                CompleteDecl("[System.Void, mscorlib, 4.0.0.0] [N.C, TestProject].M([System.String, mscorlib, 4.0.0.0] p)", 
                    "[System.Void, mscorlib, 4.0.0.0] [N.S`1[[T -> System.String, mscorlib, 4.0.0.0]], TestProject].M([T] p)", 
                    "[System.Void, mscorlib, 4.0.0.0] [i:N.I`1[[U -> System.String, mscorlib, 4.0.0.0]], TestProject].M([U] p)")
            };

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void MoreComplexHierarchy_BoundGenerics_AbstractClass()
        {
            CompleteInCSharpFile(@"
                namespace N
                {
                    public abstract class F<U>
                    {
                        public abstract void M(U p);
                    }

                    public class S<T> : F<T>
                    {
                        public override void M(T p) {}
                    }

                    public class C : S<string>
                    {
                        public override void M(string p) {}
                        $
                    }
                }
            ");

            var actual = ResultContext.TypeShape.MethodHierarchies;
            var expected = new HashSet<MethodHierarchy>
            {
                CompleteDecl("[System.Void, mscorlib, 4.0.0.0] [N.C, TestProject].M([System.String, mscorlib, 4.0.0.0] p)", 
                    "[System.Void, mscorlib, 4.0.0.0] [N.S`1[[T -> System.String, mscorlib, 4.0.0.0]], TestProject].M([T] p)", 
                    "[System.Void, mscorlib, 4.0.0.0] [N.F`1[[U -> System.String, mscorlib, 4.0.0.0]], TestProject].M([U] p)")
            };

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void NoStepDownIntoShadowedMethods()
        {
            CompleteInCSharpFile(@"
                namespace N
                {
                    public class S
                    {
                        public virtual void M() {}
                    }

                    public class C : S
                    {
                        public new void M() {}
                        $
                    }
                }
            ");

            var actual = ResultContext.TypeShape.MethodHierarchies;
            var expected = new HashSet<MethodHierarchy>
            {
                Decl("N.C", null, null)
            };

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test, Ignore]
        public void AddTestsForConstructors()
        {
            // TODO @Seb: Add tests for constructors!
        }

        private static MethodHierarchy CompleteDecl(string encType, string superType, string firstType)
        {
            var decl = new MethodHierarchy(MethodName.Get(encType));
            if (!superType.IsEmpty())
            {
                decl.Super = MethodName.Get(superType);
            }
            if (!firstType.IsEmpty())
            {
                decl.First = MethodName.Get(firstType);
            }

            return decl;
        }

        private static MethodHierarchy Decl(string encType, string superType, string firstType)
        {
            var elem = "[System.Void, mscorlib, 4.0.0.0] [" + encType + ", TestProject].M()";
            var super = "[System.Void, mscorlib, 4.0.0.0] [" + superType + ", TestProject].M()";
            var first = "[System.Void, mscorlib, 4.0.0.0] [" + firstType + ", TestProject].M()";
            return CompleteDecl(elem, superType == null ? null : super, firstType == null ? null : first);
        }
    }
}