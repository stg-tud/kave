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
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.SSTs.Declarations;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using NUnit.Framework;
using Fix = KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.Declarations
{
    class SampleCollection
    {
        public int this[int i, int j]
        {
            get { return -1; }
            set { }
        }
    }

    internal class IndexerDeclarationAnalysisTest : BaseSSTAnalysisTest
    {
        [Test]
        public void Indexer_HappyPath()
        {
            CompleteInClass(@"
                public int this[int i]
                {
                    get
                    {
                        return -1;
                    }
                    set
                    {
                        var x = value;
                    }
                }
                $
            ");
            var expected = new PropertyDeclaration
            {
                Name = Names.Property("set get [p:int] [N.C, TestProject].Item([p:int] i)"),
                Get =
                {
                    new ReturnStatement
                    {
                        Expression = Const("-1")
                    }
                },
                Set =
                {
                    VarDecl("x", Fix.Int),
                    Assign("x", RefExpr(VarRef("value")))
                }
            };
            var actual = AssertSingleProperty();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Indexer_TwoParameters()
        {
            CompleteInClass(@"
                public int this[int i, int j]
                {
                    get { return -1; }
                    set { }
                }
                $
            ");
            var expected = new PropertyDeclaration
            {
                Name = Names.Property("set get [p:int] [N.C, TestProject].Item([p:int] i, [p:int] j)"),
                Get =
                {
                    new ReturnStatement
                    {
                        Expression = Const("-1")
                    }
                }
            };
            var actual = AssertSingleProperty();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Indexer_OnlyGet()
        {
            CompleteInClass(@"
                public int this[int i]
                {
                    get
                    {
                        return -1;
                    }
                }
                $
            ");
            var expected = new PropertyDeclaration
            {
                Name = Names.Property("get [p:int] [N.C, TestProject].Item([p:int] i)"),
                Get =
                {
                    new ReturnStatement
                    {
                        Expression = Const("-1")
                    }
                }
            };
            var actual = AssertSingleProperty();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Indexer_OnlySet()
        {
            CompleteInClass(@"
                public int this[int i]
                {
                    set
                    {
                        var x = value;
                    }
                }
                $
            ");
            var expected = new PropertyDeclaration
            {
                Name = Names.Property("set [p:int] [N.C, TestProject].Item([p:int] i)"),
                Set =
                {
                    VarDecl("x", Fix.Int),
                    Assign("x", RefExpr(VarRef("value")))
                }
            };
            var actual = AssertSingleProperty();
            Assert.AreEqual(expected, actual);
        }

        private IPropertyDeclaration AssertSingleProperty()
        {
            Assert.AreEqual(1, ResultSST.Properties.Count);
            return ResultSST.Properties.First();
        }
    }
}