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
using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.TypeShapes;
using NUnit.Framework;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.TypeShapeAnalysisTestSuite
{
    internal class TypeHierarchyTest : BaseCSharpCodeCompletionTest
    {
        [Test]
        public void ShouldNotContainObjectInTypeHierarchy()
        {
            CompleteInCSharpFile(@"
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
            CompleteInCSharpFile(@"
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
            CompleteInCSharpFile(@"
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
            // ReSharper disable once PossibleNullReferenceException
            var actual = ResultContext.TypeShape.TypeHierarchy.Extends.Element;

            var expected = TypeName.Get("TestNamespace.SuperClass, TestProject");

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldRetrieveImplementedInterfaces()
        {
            CompleteInCSharpFile(@"
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
            CompleteInCSharpFile(@"
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

            // ReSharper disable once PossibleNullReferenceException
            var actual = ResultContext.TypeShape.TypeHierarchy.Extends.Element;
            var expected = TypeName.Get("N.IC`1[[T -> System.Int32, mscorlib, 4.0.0.0]], TestProject");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldRetrieveFreeSubstitution()
        {
            CompleteInCSharpFile(@"
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
            CompleteInCSharpFile(@"
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
                    Implements =
                    {
                        new TypeHierarchy("i:N.IA, TestProject")
                        {
                            Implements =
                            {
                                new TypeHierarchy("i:N.I0, TestProject")
                            }
                        }
                    }
                },
                Implements =
                {
                    new TypeHierarchy("i:N.IB`1[[TB -> System.Int32, mscorlib, 4.0.0.0]], TestProject"),
                    new TypeHierarchy("i:N.IC, TestProject")
                }
            };
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TryingToEmulateAnStackOverflowExample()
        {
            CompleteInCSharpFile(@"
                namespace N
                {
                    class G1 {}
                    class G2 : G1 {}

                    class T1 {}
                    class T2<TG> : T1 where TG : G1 {}
                    class T3 : T2<G2> {}
                    class T4 : T3
                    {
                        void M()
                        {
                            object.$

                            GetHashCode();
                        }
                    }
                }");

            var actual = ResultContext.TypeShape.TypeHierarchy;
            var expected = new TypeHierarchy
            {
                Element = Type("T4"),
                Extends = new TypeHierarchy
                {
                    Element = Type("T3"),
                    Extends = new TypeHierarchy
                    {
                        Element = Type("T2`1[[TG -> " + Type("G2") + "]]"),
                        Extends = new TypeHierarchy
                        {
                            Element = Type("T1")
                        }
                    }
                }
            };
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void RecursiveTypeDefinition()
        {
            CompleteInCSharpFile(@"
                namespace N
                {
                    class C : C {
                        public override int M() {
                            $
                        }
                    }
                }");

            var actual = ResultContext.TypeShape;
            var expected = new TypeShape
            {
                TypeHierarchy = new TypeHierarchy
                {
                    Element = Type("C")
                },
                MethodHierarchies =
                {
                    new MethodHierarchy(MethodName.Get("[System.Int32, mscorlib, 4.0.0.0] [N.C, TestProject].M()"))
                }
            };

            Assert.AreEqual(expected, actual);
        }

        private static ITypeName Type(string typeName)
        {
            return TypeName.Get("N." + typeName + ", TestProject");
        }
    }
}