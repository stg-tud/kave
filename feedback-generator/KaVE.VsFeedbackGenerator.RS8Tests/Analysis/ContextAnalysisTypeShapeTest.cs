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
 *    - Sebastian Proksch
 */

using System.Collections.Generic;
using JetBrains.Util;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names.CSharp;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis
{
    internal class ContextAnalysisTypeShapeTest : BaseTest
    {

        [Test]
        public void ImplementedMethodsAreCaptured()
        {
            CompleteInFile(@"
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
                Decl("N.C", null, "i:N.I"),
            };

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void OverriddendMethodsAreCaptured()
        {
            CompleteInFile(@"
                namespace N
                {
                    class C1
                    {
                        virtual void M() {}
                    }

                    class C2 : C1
                    {
                        override public void M()
                        {
                            $
                        }
                    }
                }               
            ");

            var actual = ResultContext.TypeShape.MethodHierarchies;
            var expected = new HashSet<MethodHierarchy>
            {
                Decl("N.C2", "N.C1", null)
            };

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void SuperImplementationsAreNotTakenIntoAccount()
        {
            CompleteInFile(@"
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

                    class C2 : C1
                    {
                        $
                    }
                }               
            ");

            var actual = ResultContext.TypeShape.MethodHierarchies;
            var expected = new HashSet<MethodHierarchy>();

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void MoreComplexHierarchy()
        {
            CompleteInFile(@"
                namespace N
                {
                    public interface I
                    {
                        void M();
                    }

                    public class S : I
                    {
                        public virtual void M() {}
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
        public void NoStepDownIntoShadowedMethods()
        {
            CompleteInFile(@"
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

        [Test]
        public void ShouldNotContainObjectInTypeHierarchy()
        {
            CompleteInFile(@"
                namespace N
                {
                    public class C
                    {
                        public void M()
                        {
                            $
                        }
                    }
                }
            ");
            Assert.IsNull(ResultContext.TypeShape.TypeHierarchy.Extends);
        }


        [Test]
        public void ShouldRetrieveEnclosingType()
        {
            CompleteInFile(@"
                namespace TestNamespace
                {
                    public interface AnInterface {}

                    public class SuperClass {}

                    public class TestClass : SuperClass, AnInterface
                    {
                        public void Doit()
                        {
                            this.Doit();
                            {caret}
                        }
                    }
                }");
            var actual = ResultContext.TypeShape.TypeHierarchy.Element;

            var expected = TypeName.Get("TestNamespace.TestClass, TestProject");

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldRetrieveSuperType()
        {
            CompleteInFile(@"
                namespace TestNamespace
                {
                    public interface AnInterface {}

                    public class SuperClass {}

                    public class TestClass : SuperClass, AnInterface
                    {
                        public void Doit()
                        {
                            this.Doit();
                            {caret}
                        }
                    }
                }");
            var actual = ResultContext.TypeShape.TypeHierarchy.Extends.Element;

            var expected = TypeName.Get("TestNamespace.SuperClass, TestProject");

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldRetrieveImplementedInterfaces()
        {
            CompleteInFile(@"
                namespace TestNamespace
                {
                    public interface AnInterface {}

                    public class SuperClass {}

                    public class TestClass : SuperClass, AnInterface
                    {
                        public void Doit()
                        {
                            this.Doit();
                            {caret}
                        }
                    }
                }");
            var actual = ResultContext.TypeShape.TypeHierarchy.Implements;

            var expected = new HashSet<ITypeHierarchy>
            {
                new TypeHierarchy("i:TestNamespace.AnInterface, TestProject")
            };

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldRetrieveSubstitution()
        {
            CompleteInFile(@"
                namespace N
                {
                    class IC<T>
                    {
                        
                    }
                
                    class C<T> : IC<int>
                    {
                        void M()
                        {
                            {caret}
                        }
                    }
                }");

            var actual = ResultContext.TypeShape.TypeHierarchy.Extends.Element;
            var expected = TypeName.Get("N.IC`1[[T -> System.Int32, mscorlib, 4.0.0.0]], TestProject");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldRetrieveFreeSubstitution()
        {
            CompleteInFile(@"
                namespace N
                {
                    class IC<T>
                    {
                        
                    }
                
                    class C<T> : IC<int>
                    {
                        void M()
                        {
                            {caret}
                        }
                    }
                }");

            var actual = ResultContext.TypeShape.TypeHierarchy.Element;
            var expected = TypeName.Get("N.C`1[[T]], TestProject");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldRetrieveLargeHierarchy()
        {
            CompleteInFile(@"
                namespace N
                {
                    interface I0
                    {
                         
                    }
                
                    interface IA : I0
                    {
                         
                    }
                
                    interface IB<TB>
                    {
                        
                    }
                
                    interface IC
                    {
                        
                    }
                
                    class A : IA
                    {
                        
                    }
                
                    class B : A, IB<int>, IC
                    {
                        public void m()
                        {
                            {caret}
                        }
                    }
                }");

            var actual = ResultContext.TypeShape.TypeHierarchy;
            var expected = new TypeHierarchy("N.B, TestProject")
            {
                Extends = new TypeHierarchy("N.A, TestProject")
                {
                    Implements = new HashSet<ITypeHierarchy>
                    {
                        new TypeHierarchy("i:N.IA, TestProject")
                        {
                            Implements = new HashSet<ITypeHierarchy>
                            {
                                new TypeHierarchy("i:N.I0, TestProject")
                            }
                        }
                    }
                },
                Implements = new HashSet<ITypeHierarchy>
                {
                    new TypeHierarchy("i:N.IB`1[[TB -> System.Int32, mscorlib, 4.0.0.0]], TestProject"),
                    new TypeHierarchy("i:N.IC, TestProject")
                }
            };
            Assert.AreEqual(expected, actual);
        }


        private static MethodHierarchy Decl(string encType, string superType, string firstType)
        {
            var elem = "[System.Void, mscorlib, 4.0.0.0] [" + encType + ", TestProject].M()";
            var super = "[System.Void, mscorlib, 4.0.0.0] [" + superType + ", TestProject].M()";
            var first = "[System.Void, mscorlib, 4.0.0.0] [" + firstType + ", TestProject].M()";

            var decl = new MethodHierarchy(MethodName.Get(elem));
            if (!superType.IsEmpty())
            {
                decl.Super = MethodName.Get(super);
            }
            if (!firstType.IsEmpty())
            {
                decl.First = MethodName.Get(first);
            }

            return decl;
        }
    }
}