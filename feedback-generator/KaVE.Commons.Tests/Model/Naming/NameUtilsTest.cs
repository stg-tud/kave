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
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Impl.v0.CodeElements;
using KaVE.Commons.Utils.Collections;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Naming
{
    internal class NameUtilsTest
    {
        [Test]
        public void HasParameters()
        {
            Assert.IsTrue("M([C,P] p)".HasParameters());
            Assert.IsTrue("M([[DR, P] [D, P].()] p)".HasParameters());
        }

        [Test]
        public void HasNoParameters()
        {
            Assert.IsFalse("M()".HasParameters());
        }

        [Test]
        public void ParsesEmptyParameters()
        {
            var methodName = new MethodName("[?] [?].M()");
            var lambdaName = new LambdaName("[?] ()");
            var expected = Lists.NewList<IParameterName>();

            Assert.AreEqual(expected, methodName.GetParameterNamesFromMethod());
            Assert.AreEqual(expected, lambdaName.GetParameterNamesFromLambda());
        }

        [TestCase("([A`1[[B, P]], P] p)", "[A`1[[B, P]], P] p"),
         TestCase("([d:[?] [?].()] p)", "[d:[?] [?].()] p"),
         TestCase("(  out [T,P] p   )", "out [T,P] p")]
        public void ParsesParameters(string parameters, string parameterId)
        {
            var methodName = new MethodName("[?] [?].M" + parameters);
            var lambdaName = new LambdaName("[?] " + parameters);
            var expected = Lists.NewList(new ParameterName(parameterId));

            Assert.AreEqual(expected, methodName.GetParameterNamesFromMethod());
            Assert.AreEqual(expected, lambdaName.GetParameterNamesFromLambda());
        }

        [TestCase("([T1,P] a, [T2,P] b)", "[T1,P] a", "[T2,P] b)"),
         TestCase("(out [T,P] p, ref [T,P] q)", "out [T,P] p", "ref [T,P] q")]
        public void ParsesMultipleParameters(string parameters, string paramId1, string paramId2)
        {
            var methodName = new MethodName("[?] [?].M" + parameters);
            var lambdaName = new LambdaName("[?] " + parameters);
            var expected = Lists.NewList(new ParameterName(paramId1), new ParameterName(paramId2));

            Assert.AreEqual(expected, methodName.GetParameterNamesFromMethod());
            Assert.AreEqual(expected, lambdaName.GetParameterNamesFromLambda());
        }

        [Test]
        public void ParsesUnknownNames()
        {
            var expected = Lists.NewList<IParameterName>();

            Assert.AreEqual(expected, new MethodName().GetParameterNamesFromMethod());
            Assert.AreEqual(expected, new LambdaName().GetParameterNamesFromLambda());
        }

        [Test]
        public void DoesNotBreakNonGenericNames()
        {
            var actual = Names.Method("[T,P] [T,P].Add()").RemoveGenerics();
            var expected = Names.Method("[T,P] [T,P].Add()");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CanRemoveGenericInformationFromResolvedTypes()
        {
            var actual = Names.Method("[T,P] [G`1[[T -> U,P]], P].Add([T] item)").RemoveGenerics();
            var expected = Names.Method("[T,P] [G`1[[T]], P].Add([T] item)");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void StillWorksWithUnresolvedParameters()
        {
            var actual = Names.Method("[T,P] [G`1[[T]], P].Add([T] item)").RemoveGenerics();
            var expected = Names.Method("[T,P] [G`1[[T]], P].Add([T] item)");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void WorksWithNestedParameters()
        {
            var actual = Names.Method("[T,P] [G`1[[T -> G2`2[[A -> T,P],[B]]]], P].Add([T] item)").RemoveGenerics();
            var expected = Names.Method("[T,P] [G`1[[T]], P].Add([T] item)");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void WorksWithMultipleParameters_2()
        {
            var actual = Names.Method("[T,P] [G`2[[T->T2,P],[U->U2,P]], P].Add([T] item)").RemoveGenerics();
            var expected = Names.Method("[T,P] [G`2[[T],[U]], P].Add([T] item)");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void WorksWithMultipleParameters_3()
        {
            var actual = Names.Method("[T,P] [G`3[[T->T2,P],[U->U2,P],[V->V2,P]], P].Add([T] item)").RemoveGenerics();
            var expected = Names.Method("[T,P] [G`3[[T],[U],[V]], P].Add([T] item)");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MultipleGenericTypes()
        {
            var actual = Names.Method("[T`1[[A->A2,P]],P] [T2`1[[B->B2,P]], P].Add([T] item)").RemoveGenerics();
            var expected = Names.Method("[T`1[[A]],P] [T2`1[[B]], P].Add([T] item)");
            Assert.AreEqual(expected, actual);
        }

        // TODO more tests with delegates (in method name test)

        #region tests for helper methods

        [Test]
        public void FindNext()
        {
            var actual = "abcabcabc".FindNext(1, 'a');
            Assert.AreEqual(3, actual);
        }

        [Test]
        public void FindNext_array()
        {
            var actual = "ccccab".FindNext(1, 'a', 'b');
            Assert.AreEqual(4, actual);
        }

        [Test]
        public void FindNext_array2()
        {
            var actual = "ccccba".FindNext(1, 'a', 'b');
            Assert.AreEqual(4, actual);
        }

        [Test]
        public void FindNext_NotFound()
        {
            var actual = "abbb".FindNext(1, 'a');
            Assert.AreEqual(-1, actual);
        }

        [Test]
        public void FindPrevious()
        {
            var actual = "abcabcabc".FindPrevious(5, 'a');
            Assert.AreEqual(3, actual);
        }

        [Test]
        public void GetCorresponding_Round_Open()
        {
            var actual = '('.GetCorresponding();
            Assert.AreEqual(')', actual);
        }

        [Test]
        public void GetCorresponding_Round_Close()
        {
            var actual = ')'.GetCorresponding();
            Assert.AreEqual('(', actual);
        }

        [Test]
        public void GetCorresponding_Curly_Open()
        {
            var actual = '{'.GetCorresponding();
            Assert.AreEqual('}', actual);
        }

        [Test]
        public void GetCorresponding_Curly_Close()
        {
            var actual = '}'.GetCorresponding();
            Assert.AreEqual('{', actual);
        }

        [Test]
        public void GetCorresponding_Array_Open()
        {
            var actual = '['.GetCorresponding();
            Assert.AreEqual(']', actual);
        }

        [Test]
        public void GetCorresponding_Array_Close()
        {
            var actual = ']'.GetCorresponding();
            Assert.AreEqual('[', actual);
        }

        [Test]
        public void GetCorresponding_Pointy_Open()
        {
            var actual = '<'.GetCorresponding();
            Assert.AreEqual('>', actual);
        }

        [Test]
        public void GetCorresponding_Pointy_Close()
        {
            var actual = '>'.GetCorresponding();
            Assert.AreEqual('<', actual);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void GetCorresponding_EverythingElse()
        {
            'x'.GetCorresponding();
        }

        [Test]
        public void FindPrevious_NotFound()
        {
            var actual = "bbb".FindPrevious(1, 'a');
            Assert.AreEqual(-1, actual);
        }

        [Test]
        public void FindCorrespondingCloseBracket_Round()
        {
            var actual = "((()))".FindCorrespondingCloseBracket(1);
            Assert.AreEqual(4, actual);
        }

        [Test]
        public void FindCorrespondingCloseBracket_Courly()
        {
            var actual = "{{{}}}".FindCorrespondingCloseBracket(1);
            Assert.AreEqual(4, actual);
        }

        [Test]
        public void FindCorrespondingCloseBracket_Array()
        {
            var actual = "[[[]]]".FindCorrespondingCloseBracket(1);
            Assert.AreEqual(4, actual);
        }

        [Test]
        public void FindCorrespondingCloseBracket_Pointy()
        {
            var actual = "<<<>>>".FindCorrespondingCloseBracket(1);
            Assert.AreEqual(4, actual);
        }

        [Test]
        public void FindCorrespondingOpenBracket_Round()
        {
            var actual = "((()))".FindCorrespondingOpenBracket(4);
            Assert.AreEqual(1, actual);
        }

        [Test]
        public void FindCorrespondingOpenBracket_Courly()
        {
            var actual = "{{{}}}".FindCorrespondingOpenBracket(4);
            Assert.AreEqual(1, actual);
        }

        [Test]
        public void FindCorrespondingOpenBracket_Array()
        {
            var actual = "[[[]]]".FindCorrespondingOpenBracket(4);
            Assert.AreEqual(1, actual);
        }

        [Test]
        public void FindCorrespondingOpenBracket_Pointy()
        {
            var actual = "<<<>>>".FindCorrespondingOpenBracket(4);
            Assert.AreEqual(1, actual);
        }

        #endregion
    }
}