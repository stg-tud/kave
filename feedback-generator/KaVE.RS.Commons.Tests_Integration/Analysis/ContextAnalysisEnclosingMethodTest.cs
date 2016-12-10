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

using System.Linq;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.TypeShapes;
using NUnit.Framework;

namespace KaVE.RS.Commons.Tests_Integration.Analysis
{
    // TODO split this into enclosingMethod and methodHierarchy tests
    [Ignore]
    internal class ContextAnalysisEnclosingMethodTest : BaseCSharpCodeCompletionTest
    {
        private static IMemberHierarchy<IMethodName> FindEnclosingMethodHierarchy(Context context)
        {
            IMethodName enclosingMethod = null; //context.EnclosingMethod;
            var enclosingMethodHierarchies =
                context.TypeShape.MethodHierarchies.Where(hierarchy => hierarchy.Element.Equals(enclosingMethod))
                       .ToList();
            Assert.AreEqual(1, enclosingMethodHierarchies.Count());
            return enclosingMethodHierarchies.First();
        }

        [Test]
        public void ShouldResolveEnclosingMethod()
        {
            WhenCodeCompletionIsInvokedInFile("SimpleHierarchy");

            var actual = FindEnclosingMethodHierarchy(ResultContext).Element;
            var expected =
                Names.Method(
                    "[System.Void, mscorlib, 4.0.0.0] [N.C6, TestProject].M([System.Int32, mscorlib, 4.0.0.0] i)");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldResolveSuperEnclosingMethod()
        {
            WhenCodeCompletionIsInvokedInFile("SimpleHierarchy");

            var actual = FindEnclosingMethodHierarchy(ResultContext).Super;
            var expected =
                Names.Method(
                    "[System.Void, mscorlib, 4.0.0.0] [N.C4, TestProject].M([System.Int32, mscorlib, 4.0.0.0] i)");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldResolveFirstEnclosingMethod()
        {
            WhenCodeCompletionIsInvokedInFile("SimpleHierarchy");

            var actual = FindEnclosingMethodHierarchy(ResultContext).First;
            var expected =
                Names.Method(
                    "[System.Void, mscorlib, 4.0.0.0] [i:N.I, TestProject].M([System.Int32, mscorlib, 4.0.0.0] i)");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AbstractClassWithImpl_Enc()
        {
            WhenCodeCompletionIsInvokedInFile("AbstractClassWithImpl");

            var actual = FindEnclosingMethodHierarchy(ResultContext).Element;
            var expected = Names.Method(
                "[System.Void, mscorlib, 4.0.0.0] [N.C2, TestProject].M([System.Int32, mscorlib, 4.0.0.0] i)");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AbstractClassWithImpl_Super()
        {
            WhenCodeCompletionIsInvokedInFile("AbstractClassWithImpl");

            var actual = FindEnclosingMethodHierarchy(ResultContext).Super;
            var expected = Names.Method(
                "[System.Void, mscorlib, 4.0.0.0] [N.C1, TestProject].M([System.Int32, mscorlib, 4.0.0.0] i)");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AbstractClassWithImpl_First()
        {
            WhenCodeCompletionIsInvokedInFile("AbstractClassWithImpl");

            var actual = FindEnclosingMethodHierarchy(ResultContext).First;
            var expected = Names.Method(
                "[System.Void, mscorlib, 4.0.0.0] [i:N.I, TestProject].M([System.Int32, mscorlib, 4.0.0.0] i)");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CollisionIsResolvedByDeclarationOrder_Enc()
        {
            WhenCodeCompletionIsInvokedInFile("CollisionIsResolvedByDeclarationOrder");

            var actual = FindEnclosingMethodHierarchy(ResultContext).Element;
            var expected = Names.Method(
                "[System.Void, mscorlib, 4.0.0.0] [N.C, TestProject].M([System.Int32, mscorlib, 4.0.0.0] i)");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CollisionIsResolvedByDeclarationOrder_Super()
        {
            WhenCodeCompletionIsInvokedInFile("CollisionIsResolvedByDeclarationOrder");
            Assert.Null(FindEnclosingMethodHierarchy(ResultContext).Super);
        }

        [Test]
        public void CollisionIsResolvedByDeclarationOrder_First()
        {
            WhenCodeCompletionIsInvokedInFile("CollisionIsResolvedByDeclarationOrder");

            var actual = FindEnclosingMethodHierarchy(ResultContext).First;
            var expected = Names.Method(
                "[System.Void, mscorlib, 4.0.0.0] [i:N.I2, TestProject].M([System.Int32, mscorlib, 4.0.0.0] i)");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DepthFirstSearchForFirstMethod_Enc()
        {
            WhenCodeCompletionIsInvokedInFile("DepthFirstSearchForFirstMethod");

            var actual = FindEnclosingMethodHierarchy(ResultContext).Element;
            var expected = Names.Method(
                "[System.Void, mscorlib, 4.0.0.0] [N.C2, TestProject].M([System.Int32, mscorlib, 4.0.0.0] i)");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DepthFirstSearchForFirstMethod_Super()
        {
            WhenCodeCompletionIsInvokedInFile("DepthFirstSearchForFirstMethod");

            var actual = FindEnclosingMethodHierarchy(ResultContext).Super;
            var expected = Names.Method(
                "[System.Void, mscorlib, 4.0.0.0] [N.C1, TestProject].M([System.Int32, mscorlib, 4.0.0.0] i)");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DepthFirstSearchForFirstMethod_First()
        {
            WhenCodeCompletionIsInvokedInFile("DepthFirstSearchForFirstMethod");

            var actual = FindEnclosingMethodHierarchy(ResultContext).First;
            var expected = Names.Method(
                "[System.Void, mscorlib, 4.0.0.0] [i:N.I1, TestProject].M([System.Int32, mscorlib, 4.0.0.0] i)");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ExternalDependencyAndGenerics_Enc()
        {
            WhenCodeCompletionIsInvokedInFile("ExternalDependencyAndGenerics");

            var actual = FindEnclosingMethodHierarchy(ResultContext).Element;
            var expected = Names.Method(
                "[System.Void, mscorlib, 4.0.0.0] [N.C, TestProject].Add([System.String, mscorlib, 4.0.0.0] s)");
            Assert.AreEqual(expected, actual);
        }

        [Test, Ignore]
        public void ExternalDependencyAndGenerics_Super()
        {
            WhenCodeCompletionIsInvokedInFile("ExternalDependencyAndGenerics");

            var actual = FindEnclosingMethodHierarchy(ResultContext).Super;
            var expected = Names.Method(
                "[System.Void, mscorlib, 4.0.0.0] [System.List, mscorlib, 4.0.0.0].Add([System.String, mscorlib, 4.0.0.0] s)");
            Assert.AreEqual(expected, actual);
        }

        [Test, Ignore]
        public void ExternalDependencyAndGenerics_First()
        {
            WhenCodeCompletionIsInvokedInFile("ExternalDependencyAndGenerics");

            var actual = FindEnclosingMethodHierarchy(ResultContext).First;
            var expected = Names.Method(
                "[System.Void, mscorlib, 4.0.0.0] [System.IList, mscorlib, 4.0.0.0].Add([System.String, mscorlib, 4.0.0.0] s)");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FirstDeclarationInAbstractClass_Enc()
        {
            WhenCodeCompletionIsInvokedInFile("FirstDeclarationInAbstractClass");

            var actual = FindEnclosingMethodHierarchy(ResultContext).Element;
            var expected = Names.Method(
                "[System.Void, mscorlib, 4.0.0.0] [N.C2, TestProject].M([System.Int32, mscorlib, 4.0.0.0] i)");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FirstDeclarationInAbstractClass_Super()
        {
            WhenCodeCompletionIsInvokedInFile("FirstDeclarationInAbstractClass");

            var actual = FindEnclosingMethodHierarchy(ResultContext).Super;
            var expected = Names.Method(
                "[System.Void, mscorlib, 4.0.0.0] [N.C1, TestProject].M([System.Int32, mscorlib, 4.0.0.0] i)");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FirstDeclarationInAbstractClass_First()
        {
            WhenCodeCompletionIsInvokedInFile("FirstDeclarationInAbstractClass");

            var actual = FindEnclosingMethodHierarchy(ResultContext).First;
            var expected = Names.Method(
                "[System.Void, mscorlib, 4.0.0.0] [N.A, TestProject].M([System.Int32, mscorlib, 4.0.0.0] i)");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FirstIsNotImplementedAtDeepestLevel_Enc()
        {
            WhenCodeCompletionIsInvokedInFile("FirstIsNotImplementedAtDeepestLevel");

            var actual = FindEnclosingMethodHierarchy(ResultContext).Element;
            var expected = Names.Method(
                "[System.Void, mscorlib, 4.0.0.0] [N.C3, TestProject].M([System.Int32, mscorlib, 4.0.0.0] i)");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FirstIsNotImplementedAtDeepestLevel_Super()
        {
            WhenCodeCompletionIsInvokedInFile("FirstIsNotImplementedAtDeepestLevel");

            var actual = FindEnclosingMethodHierarchy(ResultContext).Super;
            var expected = Names.Method(
                "[System.Void, mscorlib, 4.0.0.0] [N.C2, TestProject].M([System.Int32, mscorlib, 4.0.0.0] i)");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FirstIsNotImplementedAtDeepestLevel_First()
        {
            WhenCodeCompletionIsInvokedInFile("FirstIsNotImplementedAtDeepestLevel");

            Assert.Null(FindEnclosingMethodHierarchy(ResultContext).First);
        }

        [Test]
        public void OnlyInterface_Enc()
        {
            WhenCodeCompletionIsInvokedInFile("OnlyInterface");

            var actual = FindEnclosingMethodHierarchy(ResultContext).Element;
            var expected = Names.Method(
                "[System.Void, mscorlib, 4.0.0.0] [N.C1, TestProject].M([System.Int32, mscorlib, 4.0.0.0] i)");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void OnlyInterface_Super()
        {
            WhenCodeCompletionIsInvokedInFile("OnlyInterface");

            Assert.Null(FindEnclosingMethodHierarchy(ResultContext).Super);
        }

        [Test]
        public void OnlyInterface_First()
        {
            WhenCodeCompletionIsInvokedInFile("OnlyInterface");

            var actual = FindEnclosingMethodHierarchy(ResultContext).First;
            var expected = Names.Method(
                "[System.Void, mscorlib, 4.0.0.0] [i:N.I, TestProject].M([System.Int32, mscorlib, 4.0.0.0] i)");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void OnlySuperclass_Enc()
        {
            WhenCodeCompletionIsInvokedInFile("OnlySuperclass");

            var actual = FindEnclosingMethodHierarchy(ResultContext).Element;
            var expected = Names.Method(
                "[System.Void, mscorlib, 4.0.0.0] [N.C2, TestProject].M([System.Int32, mscorlib, 4.0.0.0] i)");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void OnlySuperclass_Super()
        {
            WhenCodeCompletionIsInvokedInFile("OnlySuperclass");

            var actual = FindEnclosingMethodHierarchy(ResultContext).Super;
            var expected = Names.Method(
                "[System.Void, mscorlib, 4.0.0.0] [N.C1, TestProject].M([System.Int32, mscorlib, 4.0.0.0] i)");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void OnlySuperclass_First()
        {
            WhenCodeCompletionIsInvokedInFile("OnlySuperclass");

            Assert.Null(FindEnclosingMethodHierarchy(ResultContext).First);
        }

        [Test]
        public void SuperMethod_AbstractDeclaration_Enc()
        {
            WhenCodeCompletionIsInvokedInFile("SuperMethod_AbstractDeclaration");

            var actual = FindEnclosingMethodHierarchy(ResultContext).Element;
            var expected = Names.Method(
                "[System.Void, mscorlib, 4.0.0.0] [N.C2, TestProject].M([System.Int32, mscorlib, 4.0.0.0] i)");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SuperMethod_AbstractDeclaration_Super()
        {
            WhenCodeCompletionIsInvokedInFile("SuperMethod_AbstractDeclaration");

            Assert.Null(FindEnclosingMethodHierarchy(ResultContext).Super);
        }

        [Test]
        public void SuperMethod_AbstractDeclaration_First()
        {
            WhenCodeCompletionIsInvokedInFile("SuperMethod_AbstractDeclaration");

            var actual = FindEnclosingMethodHierarchy(ResultContext).First;
            var expected = Names.Method(
                "[System.Void, mscorlib, 4.0.0.0] [i:N.I, TestProject].M([System.Int32, mscorlib, 4.0.0.0] i)");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void IncompleteParameterDeclaration()
        {
            CompleteInCSharpFile(@"
                class C
                {
                    public void M(int i, )
                    {
                        $
                    }
                }");
            var actual = FindEnclosingMethodHierarchy(ResultContext).Element.Identifier;
            Assert.AreEqual(
                "[System.Void, mscorlib, 4.0.0.0] [C, TestProject].M([System.Int32, mscorlib, 4.0.0.0] i, [?] ???)",
                actual);
        }
    }
}