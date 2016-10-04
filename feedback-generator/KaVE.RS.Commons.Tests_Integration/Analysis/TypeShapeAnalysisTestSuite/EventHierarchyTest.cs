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
    internal class EventHierarchyTest : BaseCSharpCodeCompletionTest
    {
        [Test]
        public void NoSuperMembers()
        {
            CompleteInCSharpFile(@"
                namespace N
                {
                    class C
                    {
                        public event object E;
                        
                        $
                    }
                }");

            var actual = ResultContext.TypeShape.EventHierarchies;
            var expected = new HashSet<EventHierarchy>
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
                        public virtual event object E;
                    }

                    class C1 : C2
                    {
                        public override event object E;
                        
                        $
                    }
                }");

            var actual = ResultContext.TypeShape.EventHierarchies;
            var expected = new HashSet<EventHierarchy>
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
                        event object E;
                    }

                    class C : I
                    {
                        public event object E;
                        
                        $
                    }
                }
            ");

            var actual = ResultContext.TypeShape.EventHierarchies;
            var expected = new HashSet<EventHierarchy>
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
                        event object E;
                    }

                    public class S
                    {
                        public virtual event object E;
                    }

                    class C : S, I
                    {
                        public override event object E; 
                        $ 
                    }
                }");

            var actual = ResultContext.TypeShape.EventHierarchies;
            var expected = new HashSet<EventHierarchy>
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
                        event object E;
                    }

                    interface I2
                    {
                        event object E;
                    }

                    class C : I1, I2
                    {
                        public event object E; 
                        $
                    }
                }");

            var actual = ResultContext.TypeShape.EventHierarchies;
            var expected = new HashSet<EventHierarchy>
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
                        event object E;
                    }

                    interface I2
                    {
                        event object E;
                    }

                    class C : I2, I1
                    {
                        public event object E; 
                        $
                    }
                }");

            var actual = ResultContext.TypeShape.EventHierarchies;
            var expected = new HashSet<EventHierarchy>
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
                        event object E;
                    }

                    interface I2
                    {
                        event object E;
                    }

                    interface I3 : I2 { }

                    class C : I3, I1
                    {
                        public event object E; 
                        $
                    }
                }");

            var actual = ResultContext.TypeShape.EventHierarchies;
            var expected = new HashSet<EventHierarchy>
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
                        event object E;
                    }

                    interface I2
                    {
                        event object E;
                    }

                    interface I3 : I2 { }

                    class C : I1, I3
                    {
                        public event object E; 
                        $
                    }
                }");

            var actual = ResultContext.TypeShape.EventHierarchies;
            var expected = new HashSet<EventHierarchy>
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
                        event object E;
                    }

                    public class S : I1
                    {
                        public virtual event object E;
                    }

                    interface I2
                    {
                        event object E;
                    }

                    class C : S, I2
                    {
                        public override event object E;
                        $
                    }
                }");

            var actual = ResultContext.TypeShape.EventHierarchies;
            var expected = new HashSet<EventHierarchy>
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
                        event object E;
                    }

                    public class S1
                    {
                        public virtual event object E;
                    }

                    public class S2 : S1
                    {
                        public override event object E;
                    }

                    class C : S2, I
                    {
                        public override event object E;
                        $
                    }
                }");

            var actual = ResultContext.TypeShape.EventHierarchies;
            var expected = new HashSet<EventHierarchy>
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
                        virtual event object E;
                    }

                    class C2 : C1
                    {
                        override event object E;
                    }

                    class C3 : C2
                    {
                        $
                    }
                }               
            ");

            var actual = ResultContext.TypeShape.EventHierarchies;
            // ReSharper disable once CollectionNeverUpdated.Local
            var expected = new HashSet<EventHierarchy>();

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
                        event object E;
                    }

                    public class C2 : I
                    {
                        public virtual event object E;
                    }

                    public class C1 : C2
                    {
                        public override event object E;
                        $
                    }
                }
            ");

            var actual = ResultContext.TypeShape.EventHierarchies;
            var expected = new HashSet<EventHierarchy>
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
                        event object E;
                    }

                    public abstract class S1 : I 
                    {
                        public virtual event object E;
                    }

                    public class S2 : S1 { }

                    public class S3 : S2 
                    {
                        public override event object E;
                    }

                    public class S4 : S3 { }

                    public class C : S4
                    {
                        public override event object E;
                        $
                    }
                }
            ");

            var actual = ResultContext.TypeShape.EventHierarchies;
            var expected = new HashSet<EventHierarchy>
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
                        event V E;
                    }

                    public class S<U> : I<U>
                    {
                        public virtual event U E;
                    }

                    public class C<T> : S<T>
                    {
                        public override event T E;
                        $
                    }
                }
            ");

            var actual = ResultContext.TypeShape.EventHierarchies;
            var expected = new HashSet<EventHierarchy>
            {
                CompleteDecl(
                    "[{0}] [N.C`1[[T]], TestProject].E".FormatEx("T"),
                    "[{0}] [N.S`1[[U -> T]], TestProject].E".FormatEx("U"),
                    "[{0}] [i:N.I`1[[V -> T]], TestProject].E".FormatEx("V"))
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
                        event U E;
                    }

                    public class S<T> : I<T>
                    {
                        public virtual event T E;
                    }

                    public class C : S<string>
                    {
                        public override event string E;
                        $
                    }
                }
            ");

            var actual = ResultContext.TypeShape.EventHierarchies;
            var expected = new HashSet<EventHierarchy>
            {
                CompleteDecl(
                    "[{0}] [N.C, TestProject].E".FormatEx(Fix.String),
                    "[{0}] [N.S`1[[T -> {1}]], TestProject].E".FormatEx("T", Fix.String),
                    "[{0}] [i:N.I`1[[U -> {1}]], TestProject].E".FormatEx("U", Fix.String))
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
                        public abstract event U E;
                    }

                    public class S<T> : F<T>
                    {
                        public override event T E;
                    }

                    public class C : S<string>
                    {
                        public override event string E;
                        $
                    }
                }
            ");

            var actual = ResultContext.TypeShape.EventHierarchies;
            var expected = new HashSet<EventHierarchy>
            {
                CompleteDecl(
                    "[{0}] [N.C, TestProject].E".FormatEx(Fix.String),
                    "[{0}] [N.S`1[[T -> {1}]], TestProject].E".FormatEx("T", Fix.String),
                    "[{0}] [N.F`1[[U -> {1}]], TestProject].E".FormatEx("U", Fix.String))
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
                        public virtual event object E;
                    }

                    public class C : S
                    {
                        public new event object E;
                        $
                    }
                }
            ");

            var actual = ResultContext.TypeShape.EventHierarchies;
            var expected = new HashSet<EventHierarchy>
            {
                Decl("N.C", null, null)
            };

            CollectionAssert.AreEquivalent(expected, actual);
        }

        private static EventHierarchy CompleteDecl(string encType, string superType, string firstType)
        {
            var decl = new EventHierarchy(Names.Event(encType));
            if (!superType.IsEmpty())
            {
                decl.Super = Names.Event(superType);
            }
            if (!firstType.IsEmpty())
            {
                decl.First = Names.Event(firstType);
            }

            return decl;
        }

        private static EventHierarchy Decl(string encType, string superType, string firstType)
        {
            var elem = "[{0}] [{1}, TestProject].E".FormatEx(Fix.Object, encType);
            var super = "[{0}] [{1}, TestProject].E".FormatEx(Fix.Object, superType);
            var first = "[{0}] [{1}, TestProject].E".FormatEx(Fix.Object, firstType);
            return CompleteDecl(elem, superType == null ? null : super, firstType == null ? null : first);
        }
    }
}