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

using KaVE.Commons.Model.Naming;
using KaVE.Commons.Utils.Collections;
using NUnit.Framework;
using Fix = KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.TypeShapeAnalysisTestSuite
{
    internal class DelegateTest : BaseCSharpCodeCompletionTest
    {
        [Test]
        public void NoDelegates()
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

            Assert.IsEmpty(ResultContext.TypeShape.Delegates);
        }

        [Test]
        public void ShouldRetrieveDelegates()
        {
            CompleteInClass(@"
                public delegate void D1(object o);
                public delegate void D2(int o);
                
                $
            ");

            var expected = Sets.NewHashSet(
                Names.Type(
                    "d:[{0}] [N.C+D1, TestProject].([{1}] o)",
                    Fix.Void,
                    Fix.Object)
                     .AsDelegateTypeName,
                Names.Type(
                    "d:[{0}] [N.C+D2, TestProject].([{1}] o)",
                    Fix.Void,
                    Fix.Int)
                     .AsDelegateTypeName);

            Assert.AreEqual(expected, ResultContext.TypeShape.Delegates);
        }
    }
}