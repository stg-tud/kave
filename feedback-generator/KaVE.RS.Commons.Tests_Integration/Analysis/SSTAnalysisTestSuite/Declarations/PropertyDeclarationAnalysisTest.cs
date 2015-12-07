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

using System;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs.Declarations;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.Utils.Collections;
using NUnit.Framework;
using Fix = KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.Declarations
{
    internal class PropertyDeclarationAnalysisTest : BaseSSTAnalysisTest
    {
        [Test]
        public void SimpleCase()
        {
            CompleteInClass(@"
                public int P {get; set;}
                $
            ");

            AssertProperties(
                new PropertyDeclaration
                {
                    Name = P("set get [{0}] [{1}].P()", Fix.Int, Fix.TestClass)
                });
        }

        [Test]
        public void ImplementedGetterAndSetter()
        {
            CompleteInClass(@"
                public int P {
                    get { return 1; }
                    set { int i = value; }
                }
                $
            ");

            AssertProperties(
                new PropertyDeclaration
                {
                    Name = P("set get [{0}] [{1}].P()", Fix.Int, Fix.TestClass),
                    Get =
                    {
                        new ReturnStatement {Expression = Const("1")}
                    },
                    Set =
                    {
                        VarDecl("i", Fix.Int),
                        Assign("i", RefExpr("value"))
                    }
                });
        }

        [Test]
        public void CompletionInEmptyGetter()
        {
            CompleteInClass(@"
                public int P {
                    get { $ }
                    set;
                }
            ");

            AssertProperties(
                new PropertyDeclaration
                {
                    Name = P("set get [{0}] [{1}].P()", Fix.Int, Fix.TestClass),
                    Get =
                    {
                        ExprStmt(new CompletionExpression())
                    }
                });
        }

        [Test]
        public void CompletionInEmptySetter()
        {
            CompleteInClass(@"
                public int P {
                    get;
                    set { $ }
                }
            ");

            AssertProperties(
                new PropertyDeclaration
                {
                    Name = P("set get [{0}] [{1}].P()", Fix.Int, Fix.TestClass),
                    Set =
                    {
                        ExprStmt(new CompletionExpression())
                    }
                });
        }

        private void AssertProperties(params IPropertyDeclaration[] props)
        {
            var expected = Sets.NewHashSetFrom(props);
            var actual = ResultSST.Properties;
            if (!expected.Equals(actual))
            {
                Console.WriteLine("expected");
                Console.WriteLine(expected);
                Console.WriteLine("but was:");
                Console.WriteLine(actual);
                Assert.Fail();
            }
        }

        private PropertyName P(string name, params object[] args)
        {
            return PropertyName.Get(name, args);
        }
    }
}