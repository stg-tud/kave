using System.Collections.Generic;
using KaVE.Model.Names;
using KaVE.Model.Names.CSharp;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis
{
    [TestFixture]
    internal class ContextAnalysisEnclosingMethodTest : KaVEBaseTest
    {
        [Test]
        public void ShouldResolveEnclosingMethod()
        {
            WhenCodeCompletionIsInvokedInFile("SimpleHierarchy");

            var actual = ResultContext.EnclosingMethod;
            var expected =
                MethodName.Get(
                    "[System.Void, mscorlib, Version=4.0.0.0] [N.C6, TestProject].M([System.Int32, mscorlib, Version=4.0.0.0] i)");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldResolveSuperEnclosingMethod()
        {
            WhenCodeCompletionIsInvokedInFile("SimpleHierarchy");

            var actual = ResultContext.EnclosingMethodSuper;
            var expected =
                MethodName.Get(
                    "[System.Void, mscorlib, Version=4.0.0.0] [N.C4, TestProject].M([System.Int32, mscorlib, Version=4.0.0.0] i)");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldResolveFirstEnclosingMethod()
        {
            WhenCodeCompletionIsInvokedInFile("SimpleHierarchy");

            var actual = ResultContext.EnclosingMethodFirst;
            var expected =
                MethodName.Get(
                    "[System.Void, mscorlib, Version=4.0.0.0] [N.I, TestProject].M([System.Int32, mscorlib, Version=4.0.0.0] i)");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AbstractClassWithImpl_Enc()
        {
            WhenCodeCompletionIsInvokedInFile("AbstractClassWithImpl");

            var actual = ResultContext.EnclosingMethod;
            var expected = MethodName.Get(
                    "[System.Void, mscorlib, Version=4.0.0.0] [N.C2, TestProject].M([System.Int32, mscorlib, Version=4.0.0.0] i)"); ;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AbstractClassWithImpl_Super()
        {
            WhenCodeCompletionIsInvokedInFile("AbstractClassWithImpl");

            var actual = ResultContext.EnclosingMethodSuper;
            var expected = MethodName.Get(
                    "[System.Void, mscorlib, Version=4.0.0.0] [N.C1, TestProject].M([System.Int32, mscorlib, Version=4.0.0.0] i)"); ;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AbstractClassWithImpl_First()
        {
            WhenCodeCompletionIsInvokedInFile("AbstractClassWithImpl");

            var actual = ResultContext.EnclosingMethodFirst;
            var expected = MethodName.Get(
                    "[System.Void, mscorlib, Version=4.0.0.0] [N.I, TestProject].M([System.Int32, mscorlib, Version=4.0.0.0] i)"); ;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CollisionIsResolvedByDeclarationOrder_Enc()
        {
            WhenCodeCompletionIsInvokedInFile("CollisionIsResolvedByDeclarationOrder");

            var actual = ResultContext.EnclosingMethod;
            var expected = MethodName.Get(
                   "[System.Void, mscorlib, Version=4.0.0.0] [N.C, TestProject].M([System.Int32, mscorlib, Version=4.0.0.0] i)"); ;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CollisionIsResolvedByDeclarationOrder_Super()
        {
            WhenCodeCompletionIsInvokedInFile("CollisionIsResolvedByDeclarationOrder");
            Assert.Null(ResultContext.EnclosingMethodSuper);
        }

        [Test]
        public void CollisionIsResolvedByDeclarationOrder_First()
        {
            WhenCodeCompletionIsInvokedInFile("CollisionIsResolvedByDeclarationOrder");

            var actual = ResultContext.EnclosingMethodFirst;
            var expected = MethodName.Get(
                   "[System.Void, mscorlib, Version=4.0.0.0] [N.I2, TestProject].M([System.Int32, mscorlib, Version=4.0.0.0] i)"); ;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DepthFirstSearchForFirstMethod_Enc()
        {
            WhenCodeCompletionIsInvokedInFile("DepthFirstSearchForFirstMethod");

            var actual = ResultContext.EnclosingMethod;
            var expected = MethodName.Get(
                   "[System.Void, mscorlib, Version=4.0.0.0] [N.C2, TestProject].M([System.Int32, mscorlib, Version=4.0.0.0] i)"); ;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DepthFirstSearchForFirstMethod_Super()
        {
            WhenCodeCompletionIsInvokedInFile("DepthFirstSearchForFirstMethod");

            var actual = ResultContext.EnclosingMethodSuper;
            var expected = MethodName.Get(
                   "[System.Void, mscorlib, Version=4.0.0.0] [N.C1, TestProject].M([System.Int32, mscorlib, Version=4.0.0.0] i)"); ;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DepthFirstSearchForFirstMethod_First()
        {
            WhenCodeCompletionIsInvokedInFile("DepthFirstSearchForFirstMethod");

            var actual = ResultContext.EnclosingMethodFirst;
            var expected = MethodName.Get(
                   "[System.Void, mscorlib, Version=4.0.0.0] [N.I1, TestProject].M([System.Int32, mscorlib, Version=4.0.0.0] i)"); ;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ExternalDependencyAndGenerics_Enc()
        {
            WhenCodeCompletionIsInvokedInFile("ExternalDependencyAndGenerics");

            var actual = ResultContext.EnclosingMethod;
            var expected = MethodName.Get(
                  "[System.Void, mscorlib, Version=4.0.0.0] [N.C, TestProject].Add([System.String, mscorlib, Version=4.0.0.0] s)"); ;
            Assert.AreEqual(expected, actual);
        }

        [Test, Ignore]
        public void ExternalDependencyAndGenerics_Super()
        {
            WhenCodeCompletionIsInvokedInFile("ExternalDependencyAndGenerics");

            var actual = ResultContext.EnclosingMethodSuper;
            var expected = MethodName.Get(
                  "[System.Void, mscorlib, Version=4.0.0.0] [System.List, mscorlib, Version=4.0.0.0].Add([System.String, mscorlib, Version=4.0.0.0] s)"); ;
            Assert.AreEqual(expected, actual);
        }

        [Test, Ignore]
        public void ExternalDependencyAndGenerics_First()
        {
            WhenCodeCompletionIsInvokedInFile("ExternalDependencyAndGenerics");

            var actual = ResultContext.EnclosingMethodFirst;
            var expected = MethodName.Get(
                   "[System.Void, mscorlib, Version=4.0.0.0] [System.IList, mscorlib, Version=4.0.0.0].Add([System.String, mscorlib, Version=4.0.0.0] s)"); ;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FirstDeclarationInAbstractClass_Enc()
        {
            WhenCodeCompletionIsInvokedInFile("FirstDeclarationInAbstractClass");

            var actual = ResultContext.EnclosingMethod;
            var expected = MethodName.Get(
                     "[System.Void, mscorlib, Version=4.0.0.0] [N.C2, TestProject].M([System.Int32, mscorlib, Version=4.0.0.0] i)"); ;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FirstDeclarationInAbstractClass_Super()
        {
            WhenCodeCompletionIsInvokedInFile("FirstDeclarationInAbstractClass");

            var actual = ResultContext.EnclosingMethodSuper;
            var expected = MethodName.Get(
                     "[System.Void, mscorlib, Version=4.0.0.0] [N.C1, TestProject].M([System.Int32, mscorlib, Version=4.0.0.0] i)"); ;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FirstDeclarationInAbstractClass_First()
        {
            WhenCodeCompletionIsInvokedInFile("FirstDeclarationInAbstractClass");

            var actual = ResultContext.EnclosingMethodFirst;
            var expected = MethodName.Get(
                     "[System.Void, mscorlib, Version=4.0.0.0] [N.A, TestProject].M([System.Int32, mscorlib, Version=4.0.0.0] i)"); ;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FirstIsNotImplementedAtDeepestLevel_Enc()
        {
            WhenCodeCompletionIsInvokedInFile("FirstIsNotImplementedAtDeepestLevel");

            var actual = ResultContext.EnclosingMethod;
            var expected = MethodName.Get(
                    "[System.Void, mscorlib, Version=4.0.0.0] [N.C3, TestProject].M([System.Int32, mscorlib, Version=4.0.0.0] i)"); ;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FirstIsNotImplementedAtDeepestLevel_Super()
        {
            WhenCodeCompletionIsInvokedInFile("FirstIsNotImplementedAtDeepestLevel");

            var actual = ResultContext.EnclosingMethodSuper;
            var expected = MethodName.Get(
                    "[System.Void, mscorlib, Version=4.0.0.0] [N.C2, TestProject].M([System.Int32, mscorlib, Version=4.0.0.0] i)"); ;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FirstIsNotImplementedAtDeepestLevel_First()
        {
            WhenCodeCompletionIsInvokedInFile("FirstIsNotImplementedAtDeepestLevel");

            Assert.Null(ResultContext.EnclosingMethodFirst);
        }

        [Test]
        public void OnlyInterface_Enc()
        {
            WhenCodeCompletionIsInvokedInFile("OnlyInterface");

            var actual = ResultContext.EnclosingMethod;
            var expected = MethodName.Get(
                    "[System.Void, mscorlib, Version=4.0.0.0] [N.C1, TestProject].M([System.Int32, mscorlib, Version=4.0.0.0] i)"); ;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void OnlyInterface_Super()
        {
            WhenCodeCompletionIsInvokedInFile("OnlyInterface");

            Assert.Null(ResultContext.EnclosingMethodSuper);
        }

        [Test]
        public void OnlyInterface_First()
        {
            WhenCodeCompletionIsInvokedInFile("OnlyInterface");

            var actual = ResultContext.EnclosingMethodFirst;
            var expected = MethodName.Get(
                    "[System.Void, mscorlib, Version=4.0.0.0] [N.I, TestProject].M([System.Int32, mscorlib, Version=4.0.0.0] i)"); ;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void OnlySuperclass_Enc()
        {
            WhenCodeCompletionIsInvokedInFile("OnlySuperclass");

            var actual = ResultContext.EnclosingMethod;
            var expected = MethodName.Get(
                    "[System.Void, mscorlib, Version=4.0.0.0] [N.C2, TestProject].M([System.Int32, mscorlib, Version=4.0.0.0] i)"); ;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void OnlySuperclass_Super()
        {
            WhenCodeCompletionIsInvokedInFile("OnlySuperclass");

            var actual = ResultContext.EnclosingMethodSuper;
            var expected = MethodName.Get(
                    "[System.Void, mscorlib, Version=4.0.0.0] [N.C1, TestProject].M([System.Int32, mscorlib, Version=4.0.0.0] i)"); ;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void OnlySuperclass_First()
        {
            WhenCodeCompletionIsInvokedInFile("OnlySuperclass");
            
            Assert.Null(ResultContext.EnclosingMethodFirst);
        }

        [Test]
        public void SuperMethod_AbstractDeclaration_Enc()
        {
            WhenCodeCompletionIsInvokedInFile("SuperMethod_AbstractDeclaration");

            var actual = ResultContext.EnclosingMethod;
            var expected = MethodName.Get(
                    "[System.Void, mscorlib, Version=4.0.0.0] [N.C2, TestProject].M([System.Int32, mscorlib, Version=4.0.0.0] i)"); ;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SuperMethod_AbstractDeclaration_Super()
        {
            WhenCodeCompletionIsInvokedInFile("SuperMethod_AbstractDeclaration");

            Assert.Null(ResultContext.EnclosingMethodSuper);
        }

        [Test]
        public void SuperMethod_AbstractDeclaration_First()
        {
            WhenCodeCompletionIsInvokedInFile("SuperMethod_AbstractDeclaration");

            var actual = ResultContext.EnclosingMethodFirst;
            var expected = MethodName.Get(
                    "[System.Void, mscorlib, Version=4.0.0.0] [N.I, TestProject].M([System.Int32, mscorlib, Version=4.0.0.0] i)"); ;
            Assert.AreEqual(expected, actual);
        }
    }
}