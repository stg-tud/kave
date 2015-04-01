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

using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite
{
    internal class CastingAndTypeCheckingTest : BaseSSTAnalysisTest
    {
        [Test, Ignore]
        public void TypeOf()
        {
            CompleteInClass(@"
                public void A()
                {
                    var t = typeof(int);
                    $
                }
            ");

            var mA = NewMethodDeclaration(SSTAnalysisFixture.Void, "A");
            mA.Body.Add(SSTUtil.Declare("t", TypeName.Get("System.Type, mscorlib, 4.0.0.0")));
            mA.Body.Add(SSTUtil.AssignmentToLocal("t", new ConstantValueExpression()));

            AssertAllMethods(mA);
        }

        [Test, Ignore]
        public void CompositionOfTypeOf()
        {
            CompleteInClass(@"
                public void A()
                {
                    var t = typeof(int) == typeof(string);
                    $
                }
            ");

            var mA = NewMethodDeclaration(SSTAnalysisFixture.Void, "A");
            mA.Body.Add(SSTUtil.Declare("t", SSTAnalysisFixture.Bool));
            mA.Body.Add(SSTUtil.AssignmentToLocal("t", new ConstantValueExpression()));

            AssertAllMethods(mA);
        }

        [Test, Ignore]
        public void Is_Reference()
        {
            CompleteInClass(@"
                public void A(object o)
                {
                    var isInstanceOf = o is string;
                    $
                }
            ");

            var mA = NewMethodDeclaration(
                SSTAnalysisFixture.Void,
                "A",
                string.Format("[{0}] o", SSTAnalysisFixture.Object));
            mA.Body.Add(SSTUtil.Declare("isInstanceOf", SSTAnalysisFixture.Bool));
            mA.Body.Add(SSTUtil.AssignmentToLocal("isInstanceOf", SSTUtil.ComposedExpression("o")));

            AssertAllMethods(mA);
        }

        [Test, Ignore]
        public void Is_Const()
        {
            CompleteInClass(@"
                public void A()
                {
                    var isInstanceOf = 1 is double;
                    $
                }
            ");

            var mA = NewMethodDeclaration(SSTAnalysisFixture.Void, "A");
            mA.Body.Add(SSTUtil.Declare("isInstanceOf", SSTAnalysisFixture.Bool));
            mA.Body.Add(SSTUtil.AssignmentToLocal("isInstanceOf", new ConstantValueExpression()));

            AssertAllMethods(mA);
        }

        [Test, Ignore]
        public void As_Reference()
        {
            CompleteInClass(@"
                public void A(object o)
                {
                    var cast = o as string;
                    $
                }
            ");

            var mA = NewMethodDeclaration(
                SSTAnalysisFixture.Void,
                "A",
                string.Format("[{0}] o", SSTAnalysisFixture.Object));
            mA.Body.Add(SSTUtil.Declare("cast", SSTAnalysisFixture.String));
            mA.Body.Add(SSTUtil.AssignmentToLocal("cast", SSTUtil.ComposedExpression("o")));

            AssertAllMethods(mA);
        }

        [Test, Ignore]
        public void As_Const()
        {
            CompleteInClass(@"
                public void A()
                {
                    var cast = 1.0 as object;
                    $
                }
            ");

            var mA = NewMethodDeclaration(SSTAnalysisFixture.Void, "A");
            mA.Body.Add(SSTUtil.Declare("cast", SSTAnalysisFixture.Object));
            mA.Body.Add(SSTUtil.AssignmentToLocal("cast", new ConstantValueExpression()));

            AssertAllMethods(mA);
        }

        [Test, Ignore]
        public void Cast_Const()
        {
            CompleteInClass(@"
                public void A()
                {
                    var i = (int) 1.0;
                    $
                }
            ");

            var mA = NewMethodDeclaration(SSTAnalysisFixture.Void, "A");
            mA.Body.Add(SSTUtil.Declare("i", SSTAnalysisFixture.Int));
            mA.Body.Add(SSTUtil.AssignmentToLocal("i", new ConstantValueExpression()));

            AssertAllMethods(mA);
        }

        [Test, Ignore]
        public void Cast_Reference()
        {
            CompleteInClass(@"
                public void A(object o)
                {
                    var s = (string) o;
                    $
                }
            ");

            var mA = NewMethodDeclaration(
                SSTAnalysisFixture.Void,
                "A",
                string.Format("[{0}] o", SSTAnalysisFixture.Object));
            mA.Body.Add(SSTUtil.Declare("s", SSTAnalysisFixture.String));
            mA.Body.Add(SSTUtil.AssignmentToLocal("s", SSTUtil.ComposedExpression("o")));

            AssertAllMethods(mA);
        }
    }
}