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

using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Utils.Collections;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite
{
    internal class ReferencesTest : BaseSSTAnalysisTest
    {
        // variables, members, etc.

        [Test, Ignore]
        public void NullReference()
        {
            CompleteInMethod(@"
                object o = null;
                $
            ");

            var body = Lists.NewList<IStatement>();
            body.Add(SSTUtil.Declare("o", SSTAnalysisFixture.Object));
            body.Add(SSTUtil.AssignmentToLocal("o", new NullExpression()));

            AssertBody(body);
        }

        [Test, Ignore]
        public void AReferenceExpression()
        {
            CompleteInMethod(@"
                object o = null;
                object o2 = o;
                $
            ");

            var body = Lists.NewList<IStatement>();
            body.Add(SSTUtil.Declare("o", SSTAnalysisFixture.Object));
            body.Add(SSTUtil.AssignmentToLocal("o", new NullExpression()));
            body.Add(SSTUtil.Declare("o2", SSTAnalysisFixture.Object));
            body.Add(SSTUtil.AssignmentToLocal("o2", SSTUtil.ReferenceExprToVariable("o")));

            AssertBody(body);
        }
    }
}