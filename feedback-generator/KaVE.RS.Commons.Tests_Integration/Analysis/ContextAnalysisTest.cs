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

using NUnit.Framework;

namespace KaVE.RS.Commons.Tests_Integration.Analysis
{
    internal class ContextAnalysisTest : BaseCSharpCodeCompletionTest
    {
        [Test]
        public void ShouldRetrieveContext()
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
            Assert.IsNotNull(ResultContext);
        }

        [Test]
        public void ShouldIdentifNonPartialClasses()
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
            Assert.IsFalse(ResultSST.IsPartialClass);
            Assert.Null(ResultSST.PartialClassIdentifier);
        }

        [Test]
        public void ShouldIdentifyClassNameOfPartialClasses()
        {
            CompleteInCSharpFile(@"
                namespace N
                {
                    public partial class C
                    {
                        public void M()
                        {
                            $
                        }
                    }
                }
            ");
            Assert.That(ResultSST.IsPartialClass);
            var pcid = ResultSST.PartialClassIdentifier;
            Assert.NotNull(pcid);
            var isValid = pcid.EndsWith(".cs");
            Assert.IsTrue(isValid, "unexpected identifier for partial class '{0}'", pcid);
        }
    }
}