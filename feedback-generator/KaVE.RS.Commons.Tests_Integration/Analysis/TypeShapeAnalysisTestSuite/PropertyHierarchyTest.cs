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

using System;
using System.Collections.Generic;
using JetBrains.Util;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.TypeShapes;
using KaVE.Commons.Utils;
using NUnit.Framework;
using Fix = KaVE.Commons.TestUtils.Model.Naming.NameFixture;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.TypeShapeAnalysisTestSuite
{
    internal class PropertyHierarchyTest : BaseCSharpCodeCompletionTest
    {
        [Test]
        public void NoSuperMembers()
        {
            CompleteInCSharpFile(@"
                namespace N
                {
                    class C
                    {
                        public object P { get; set; }
                        
                        $
                    }
                }");

            var actual = ResultContext.TypeShape.PropertyHierarchies;
            var expected = new HashSet<PropertyHierarchy>
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
                        public virtual object P { get; set; }
                    }

                    class C1 : C2
                    {
                        public override object P { get; set; }
                        
                        $
                    }
                }");

            var actual = ResultContext.TypeShape.PropertyHierarchies;
            var expected = new HashSet<PropertyHierarchy>
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
                        object P { get; set; }
                    }

                    class C : I
                    {
                        public object P { get; set; }
                        
                        $
                    }
                }
            ");

            var actual = ResultContext.TypeShape.PropertyHierarchies;
            var expected = new HashSet<PropertyHierarchy>
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
                        object P { get; set; }
                    }

                    public class S
                    {
                        public virtual object P { get; set; }
                    }

                    class C : S, I
                    {
                        public override object P { get; set; } 
                        $ 
                    }
                }");

            var actual = ResultContext.TypeShape.PropertyHierarchies;
            var expected = new HashSet<PropertyHierarchy>
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
                        object P { get; set; }
                    }

                    interface I2
                    {
                        object P { get; set; }
                    }

                    class C : I1, I2
                    {
                        public object P { get; set; } 
                        $
                    }
                }");

            var actual = ResultContext.TypeShape.PropertyHierarchies;
            var expected = new HashSet<PropertyHierarchy>
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
                        object P { get; set; }
                    }

                    interface I2
                    {
                        object P { get; set; }
                    }

                    class C : I2, I1
                    {
                        public object P { get; set; } 
                        $
                    }
                }");

            var actual = ResultContext.TypeShape.PropertyHierarchies;
            var expected = new HashSet<PropertyHierarchy>
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
                        object P { get; set; }
                    }

                    interface I2
                    {
                        object P { get; set; }
                    }

                    interface I3 : I2 { }

                    class C : I3, I1
                    {
                        public object P { get; set; } 
                        $
                    }
                }");

            var actual = ResultContext.TypeShape.PropertyHierarchies;
            var expected = new HashSet<PropertyHierarchy>
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
                        object P { get; set; }
                    }

                    interface I2
                    {
                        object P { get; set; }
                    }

                    interface I3 : I2 { }

                    class C : I1, I3
                    {
                        public object P { get; set; } 
                        $
                    }
                }");

            var actual = ResultContext.TypeShape.PropertyHierarchies;
            var expected = new HashSet<PropertyHierarchy>
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
                        object P { get; set; }
                    }

                    public class S : I1
                    {
                        public virtual object P { get; set; }
                    }

                    interface I2
                    {
                        object P { get; set; }
                    }

                    class C : S, I2
                    {
                        public override object P { get; set; }
                        $
                    }
                }");

            var actual = ResultContext.TypeShape.PropertyHierarchies;
            var expected = new HashSet<PropertyHierarchy>
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
                        object P { get; set; }
                    }

                    public class S1
                    {
                        public virtual object P { get; set; }
                    }

                    public class S2 : S1
                    {
                        public override object P { get; set; }
                    }

                    class C : S2, I
                    {
                        public override object P { get; set; }
                        $
                    }
                }");

            var actual = ResultContext.TypeShape.PropertyHierarchies;
            var expected = new HashSet<PropertyHierarchy>
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
                        virtual object P { get; set; }
                    }

                    class C2 : C1
                    {
                        override object P { get; set; }
                    }

                    class C3 : C2
                    {
                        $
                    }
                }               
            ");

            var actual = ResultContext.TypeShape.PropertyHierarchies;
            // ReSharper disable once CollectionNeverUpdated.Local
            var expected = new HashSet<PropertyHierarchy>();

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
                        object P { get; set; }
                    }

                    public class C2 : I
                    {
                        public virtual object P { get; set; }
                    }

                    public class C1 : C2
                    {
                        public override object P { get; set; }
                        $
                    }
                }
            ");

            var actual = ResultContext.TypeShape.PropertyHierarchies;
            var expected = new HashSet<PropertyHierarchy>
            {
                Decl("N.C1", "N.C2", "i:N.I")
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
                        object P { get; set; }
                    }

                    public abstract class S1 : I 
                    {
                        public virtual object P { get; set; }
                    }

                    public class S2 : S1 { }

                    public class S3 : S2 
                    {
                        public override object P { get; set; }
                    }

                    public class S4 : S3 { }

                    public class C : S4
                    {
                        public override object P { get; set; }
                        $
                    }
                }
            ");

            var actual = ResultContext.TypeShape.PropertyHierarchies;
            var expected = new HashSet<PropertyHierarchy>
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
                        V P { get; set; }
                    }

                    public class S<U> : I<U>
                    {
                        public virtual U P { get; set; }
                    }

                    public class C<T> : S<T>
                    {
                        public override T P { get; set; }
                        $
                    }
                }
            ");

            var actual = ResultContext.TypeShape.PropertyHierarchies;
            var expected = new HashSet<PropertyHierarchy>
            {
                CompleteDecl(
                    "set get [{0}] [N.C`1[[T]], TestProject].P()".FormatEx("T"),
                    "set get [{0}] [N.S`1[[U -> T]], TestProject].P()".FormatEx("U"),
                    "set get [{0}] [i:N.I`1[[V -> T]], TestProject].P()".FormatEx("V"))
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
                        U P { get; set; }
                    }

                    public class S<T> : I<T>
                    {
                        public virtual T P { get; set; }
                    }

                    public class C : S<string>
                    {
                        public override string P { get; set; }
                        $
                    }
                }
            ");

            var actual = ResultContext.TypeShape.PropertyHierarchies;
            var expected = new HashSet<PropertyHierarchy>
            {
                CompleteDecl(
                    "set get [{0}] [N.C, TestProject].P()".FormatEx(Fix.String),
                    "set get [{0}] [N.S`1[[T -> {1}]], TestProject].P()".FormatEx("T", Fix.String),
                    "set get [{0}] [i:N.I`1[[U -> {1}]], TestProject].P()".FormatEx("U", Fix.String))
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
                        public abstract U P { get; set; }
                    }

                    public class S<T> : F<T>
                    {
                        public override T P { get; set; }
                    }

                    public class C : S<string>
                    {
                        public override string P { get; set; }
                        $
                    }
                }
            ");

            var actual = ResultContext.TypeShape.PropertyHierarchies;
            var expected = new HashSet<PropertyHierarchy>
            {
                CompleteDecl(
                    "set get [{0}] [N.C, TestProject].P()".FormatEx(Fix.String),
                    "set get [{0}] [N.S`1[[T -> {1}]], TestProject].P()".FormatEx("T", Fix.String),
                    "set get [{0}] [N.F`1[[U -> {1}]], TestProject].P()".FormatEx("U", Fix.String))
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
                        public virtual object P { get; set; }
                    }

                    public class C : S
                    {
                        public new object P { get; set; }
                        $
                    }
                }
            ");

            var actual = ResultContext.TypeShape.PropertyHierarchies;
            var expected = new HashSet<PropertyHierarchy>
            {
                Decl("N.C", null, null)
            };

            CollectionAssert.AreEquivalent(expected, actual);
        }

        private static PropertyHierarchy CompleteDecl(string encType, string superType, string firstType)
        {
            var decl = new PropertyHierarchy(Names.Property(encType));
            if (!superType.IsEmpty())
            {
                decl.Super = Names.Property(superType);
            }
            if (!firstType.IsEmpty())
            {
                decl.First = Names.Property(firstType);
            }

            return decl;
        }

        private static PropertyHierarchy Decl(string encType, string superType, string firstType)
        {
            var elem = "set get [{0}] [{1}, TestProject].P()".FormatEx(Fix.Object, encType);
            var super = "set get [{0}] [{1}, TestProject].P()".FormatEx(Fix.Object, superType);
            var first = "set get [{0}] [{1}, TestProject].P()".FormatEx(Fix.Object, firstType);
            return CompleteDecl(elem, superType == null ? null : super, firstType == null ? null : first);
        }
    }
}