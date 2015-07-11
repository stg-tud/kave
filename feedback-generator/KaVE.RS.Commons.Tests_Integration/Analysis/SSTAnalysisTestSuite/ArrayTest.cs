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

using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using NUnit.Framework;
using Fix = KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite
{
    internal class ArrayTest : BaseSSTAnalysisTest
    {
        [Test, Ignore]
        public void ArrayInit_Constant()
        {
            CompleteInClass(@"
                public void A()
                {
                    var arr = new[] {1,2,3};
                    $
                }
            ");

            var mA = NewMethodDeclaration(SSTAnalysisFixture.Void, "A");
            mA.Body.Add(SSTUtil.Declare("arr", SSTAnalysisFixture.IntArray));
            mA.Body.Add(SSTUtil.AssignmentToLocal("arr", new ConstantValueExpression()));

            AssertAllMethods(mA);
        }

        [Test, Ignore]
        public void ArrayInit_Composed()
        {
            CompleteInClass(@"
                public void A()
                {
                    var n = 1;
                    var arr = new[] {1,2,n};
                    $
                }
            ");

            var mA = NewMethodDeclaration(SSTAnalysisFixture.Void, "A");
            mA.Body.Add(SSTUtil.Declare("n", SSTAnalysisFixture.Int));
            mA.Body.Add(SSTUtil.AssignmentToLocal("n", new ConstantValueExpression()));
            mA.Body.Add(SSTUtil.Declare("arr", SSTAnalysisFixture.IntArray));
            mA.Body.Add(SSTUtil.AssignmentToLocal("arr", SSTUtil.ComposedExpression("n")));

            AssertAllMethods(mA);
        }

        [Test, Ignore]
        public void ArrayInit_WithArrays()
        {
            CompleteInClass(@"
                public void A()
                {
                    var arr = new[] {new[] {1, 2}, new[] {3, 4}};
                    $
                }
            ");

            var mA = NewMethodDeclaration(SSTAnalysisFixture.Void, "A");
            mA.Body.Add(SSTUtil.Declare("arr", TypeName.Get("System.Int32[][], mscorlib, 4.0.0.0")));
            mA.Body.Add(SSTUtil.Declare("$0", SSTAnalysisFixture.IntArray));
            mA.Body.Add(SSTUtil.AssignmentToLocal("$0", new ConstantValueExpression()));
            mA.Body.Add(SSTUtil.Declare("$1", SSTAnalysisFixture.IntArray));
            mA.Body.Add(SSTUtil.AssignmentToLocal("$1", new ConstantValueExpression()));
            mA.Body.Add(SSTUtil.AssignmentToLocal("arr", SSTUtil.ComposedExpression("$0", "$1")));

            AssertAllMethods(mA);
        }

        [Test, Ignore]
        public void ArrayInit_WithCalls()
        {
            CompleteInClass(@"
                public void A(object o)
                {
                    var arr = new[] {1,2,o.GetHashCode()};
                    $
                }
            ");

            var mA = NewMethodDeclaration(
                SSTAnalysisFixture.Void,
                "A",
                string.Format("[{0}] o", SSTAnalysisFixture.Object));
            mA.Body.Add(SSTUtil.Declare("arr", SSTAnalysisFixture.IntArray));
            mA.Body.Add(SSTUtil.Declare("$0", SSTAnalysisFixture.Int));
            mA.Body.Add(SSTUtil.AssignmentToLocal("$0", SSTUtil.InvocationExpression("o", Fix.Object_GetHashCode)));
            mA.Body.Add(SSTUtil.AssignmentToLocal("arr", SSTUtil.ComposedExpression("$0")));

            AssertAllMethods(mA);
        }
    }
}