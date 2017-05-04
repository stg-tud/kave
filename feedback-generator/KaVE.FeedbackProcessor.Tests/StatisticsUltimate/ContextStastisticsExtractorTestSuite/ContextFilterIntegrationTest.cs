/*
 * Copyright 2017 Sebastian Proksch
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

using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.FeedbackProcessor.StatisticsUltimate;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.StatisticsUltimate.ContextStastisticsExtractorTestSuite
{
    internal class ContextFilterIntegrationTest : ContextStatisticsExtractorTestBase
    {
        [Test]
        public void SameTypeTwice()
        {
            SetUp(GeneratedCode.Include, Duplication.Include);

            var stats = Extract(
                new Context
                {
                    SST = new SST
                    {
                        EnclosingType = Names.Type("C,P")
                    }
                },
                new Context
                {
                    SST = new SST
                    {
                        EnclosingType = Names.Type("C,P")
                    }
                });

            Assert.AreEqual(2, stats.NumClasses);
        }

        [Test]
        public void SameTypeTwice_Exclude()
        {
            SetUp(GeneratedCode.Include, Duplication.Exclude);

            var stats = Extract(
                new Context
                {
                    SST = new SST
                    {
                        EnclosingType = Names.Type("C,P")
                    }
                },
                new Context
                {
                    SST = new SST
                    {
                        EnclosingType = Names.Type("C,P")
                    }
                });

            Assert.AreEqual(1, stats.NumClasses);
        }

        [Test]
        public void SameTypeTwice_ExcludePartial()
        {
            SetUp(GeneratedCode.Include, Duplication.Exclude);

            var stats = Extract(
                new Context
                {
                    SST = new SST
                    {
                        EnclosingType = Names.Type("C,P"),
                        PartialClassIdentifier = "a"
                    }
                },
                new Context
                {
                    SST = new SST
                    {
                        EnclosingType = Names.Type("C,P"),
                        PartialClassIdentifier = "a"
                    }
                });
            // real duplicate partial classes cannot be filtered to "1", but we also capture unique typeDecls
            Assert.AreEqual(2, stats.NumClasses);
        }

        [Test]
        public void SameMethodInPartialTypeTwice()
        {
            SetUp(GeneratedCode.Include, Duplication.Include);

            var stats = Extract(
                new Context
                {
                    SST = new SST
                    {
                        EnclosingType = Names.Type("C,P"),
                        PartialClassIdentifier = "a",
                        Methods =
                        {
                            new MethodDeclaration {Name = Names.Method("[p:void] [C,P].M()")}
                        }
                    }
                },
                new Context
                {
                    SST = new SST
                    {
                        EnclosingType = Names.Type("C,P"),
                        PartialClassIdentifier = "a",
                        Methods =
                        {
                            new MethodDeclaration {Name = Names.Method("[p:void] [C,P].M()")}
                        }
                    }
                });

            Assert.AreEqual(2, stats.NumClasses);
            Assert.AreEqual(2, stats.NumMethodDeclsTotal);
        }

        [Test]
        public void SameMethodInPartialTypeTwice_Exclude()
        {
            SetUp(GeneratedCode.Include, Duplication.Exclude);

            var stats = Extract(
                new Context
                {
                    SST = new SST
                    {
                        EnclosingType = Names.Type("C,P"),
                        PartialClassIdentifier = "a",
                        Methods =
                        {
                            new MethodDeclaration {Name = Names.Method("[p:void] [C,P].M()")}
                        }
                    }
                },
                new Context
                {
                    SST = new SST
                    {
                        EnclosingType = Names.Type("C,P"), // real duplicate
                        PartialClassIdentifier = "a",
                        Methods =
                        {
                            new MethodDeclaration {Name = Names.Method("[p:void] [C,P].M()")}
                        }
                    }
                });

            Assert.AreEqual(2, stats.NumClasses); // see above
            Assert.AreEqual(1, stats.NumMethodDeclsTotal);
        }

        protected void SetUp(GeneratedCode genCode, Duplication dupe)
        {
            var uc = new ContextFilter(genCode, dupe);
            Sut = new ContextStatisticsExtractor(uc);
        }
    }
}