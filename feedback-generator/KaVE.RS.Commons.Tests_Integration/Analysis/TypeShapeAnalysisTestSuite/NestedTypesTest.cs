using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.TypeShapes;
using KaVE.Commons.Utils.Collections;
using NUnit.Framework;
using Fix = KaVE.Commons.TestUtils.Model.Naming.NameFixture;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.TypeShapeAnalysisTestSuite
{
    internal class NestedTypesTest : BaseCSharpCodeCompletionTest
    {
        [Test]
        public void NestedTypesEmptyWithoutNestedTypes()
        {
            CompleteInNamespace(@"
                public class C
                {
                    public void M()
                    {
                        $
                    }
                }
            ");
            Assert.IsEmpty(ResultContext.TypeShape.NestedTypes);
        }

        [Test]
        public void ShouldRetrieveAllNestedTypes()
        {
            CompleteInCSharpFile(@"
                namespace TestNamespace
                {
                    public interface AnInterface {}

                    public class SuperClass {}

                    public class TestClass
                    {
                        public void Doit()
                        {
                            this.Doit();
                            {caret}
                        }

                        public class N1 {}
                        public class N2 {}
                        public class N3 {}
                    }
                }");
            Assert.AreEqual(Sets.NewHashSet(
                new TypeHierarchy("TestNamespace.TestClass+N1, TestProject"),
                new TypeHierarchy("TestNamespace.TestClass+N2, TestProject"),
                new TypeHierarchy("TestNamespace.TestClass+N3, TestProject")), 
                ResultContext.TypeShape.NestedTypes);
        }

        [Test]
        public void ShouldRetrieveFullTypeHierarchyForNestedType()
        {
            CompleteInCSharpFile(@"
                namespace TestNamespace
                {
                    public interface AnInterface {}

                    public class SuperClass {}

                    public class TestClass 
                    {
                        public class NestedClass : SuperClass, AnInterface {}
                        public void Doit()
                        {
                            this.Doit();
                            {caret}
                        }
                    }
                }");

            CollectionAssert.Contains(ResultContext.TypeShape.NestedTypes, new TypeHierarchy
            {
                Element = Names.Type("TestNamespace.TestClass+NestedClass, TestProject"),
                Extends = new TypeHierarchy("TestNamespace.SuperClass, TestProject"),
                Implements = Sets.NewHashSet<ITypeHierarchy>(new TypeHierarchy("i:TestNamespace.AnInterface, TestProject"))
            });
        }
    }
}