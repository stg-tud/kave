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
using NUnit.Framework;
using NameUtils = KaVE.Commons.Model.Naming.Impl.v0.NameUtils;

namespace KaVE.Commons.Tests.Model.Naming.CSharp
{
    internal class NameUtilsTest
    {
        [Test]
        public void HasParameters()
        {
            Assert.IsTrue(NameUtils.HasParameters("M([C,P] p)"));
            Assert.IsTrue(NameUtils.HasParameters("M([[DR, P] [D, P].()] p)"));
        }

        [Test]
        public void HasNoParameters()
        {
            Assert.IsFalse(NameUtils.HasParameters("M()"));
        }

        [Test]
        public void ParsesParametersWithParameterizedType()
        {
            var parameterNames = NameUtils.GetParameterNames("M([A`1[[B, P]], P] p)");
            Assert.AreEqual(Names.Parameter("[A`1[[B, P]], P] p"), parameterNames[0]);
        }

        [Test]
        public void ParsesParametersWithDelegateType()
        {
            var parameterNames = NameUtils.GetParameterNames("M([[DR, P] [D, P].()] p)");
            Assert.AreEqual(Names.Parameter("[[DR, P] [D, P].()] p"), parameterNames[0]);
        }

        [Test]
        public void ParsesMultipleParameters()
        {
            var parameterNames = NameUtils.GetParameterNames("M([T1,P] a, [T2,P] b)");
            Assert.AreEqual(2, parameterNames.Count);
            Assert.AreEqual(Names.Parameter("[T1,P] a"), parameterNames[0]);
            Assert.AreEqual(Names.Parameter("[T2,P] b"), parameterNames[1]);
        }

        [Test]
        public void ParsesUnknownNames()
        {
            var parameterNames = NameUtils.GetParameterNames("???");
            Assert.AreEqual(0, parameterNames.Count);
        }

        [Test]
        public void ParsesEmptyParameters()
        {
            var parameterNames = NameUtils.GetParameterNames("M()");
            Assert.AreEqual(0, parameterNames.Count);
        }

        [Test]
        public void ParsesParametersWithModifiers()
        {
            var parameterNames = NameUtils.GetParameterNames("M(out [T,P] p, out [T,P] q)");
            Assert.AreEqual(2, parameterNames.Count);
            Assert.AreEqual(Names.Parameter("out [T,P] p"), parameterNames[0]);
            Assert.AreEqual(Names.Parameter("out [T,P] q"), parameterNames[1]);
        }

        [Test]
        public void ParsesParametersWithAdditionalWhitespace()
        {
            var parameterNames = NameUtils.GetParameterNames("M(  out [T,P] p   )");
            Assert.AreEqual(1, parameterNames.Count);
            Assert.AreEqual(Names.Parameter("out [T,P] p"), parameterNames[0]);
        }

        [Test]
        public void DoesNotHurtNonGenericNAmes()
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
            var actual = NameUtils.FindNext("abcabcabc", 1, 'a');
            Assert.AreEqual(3, actual);
        }

        [Test]
        public void FindNext_array()
        {
            var actual = NameUtils.FindNext("ccccab", 1, 'a', 'b');
            Assert.AreEqual(4, actual);
        }

        [Test]
        public void FindNext_array2()
        {
            var actual = NameUtils.FindNext("ccccba", 1, 'a', 'b');
            Assert.AreEqual(4, actual);
        }

        [Test]
        public void FindNext_NotFound()
        {
            var actual = NameUtils.FindNext("abbb", 1, 'a');
            Assert.AreEqual(-1, actual);
        }

        [Test]
        public void FindPrevious()
        {
            var actual = NameUtils.FindPrevious("abcabcabc", 5, 'a');
            Assert.AreEqual(3, actual);
        }

        [Test]
        public void GetCorresponding_Round_Open()
        {
            var actual = NameUtils.GetCorresponding('(');
            Assert.AreEqual(')', actual);
        }

        [Test]
        public void GetCorresponding_Round_Close()
        {
            var actual = NameUtils.GetCorresponding(')');
            Assert.AreEqual('(', actual);
        }

        [Test]
        public void GetCorresponding_Curly_Open()
        {
            var actual = NameUtils.GetCorresponding('{');
            Assert.AreEqual('}', actual);
        }

        [Test]
        public void GetCorresponding_Curly_Close()
        {
            var actual = NameUtils.GetCorresponding('}');
            Assert.AreEqual('{', actual);
        }

        [Test]
        public void GetCorresponding_Array_Open()
        {
            var actual = NameUtils.GetCorresponding('[');
            Assert.AreEqual(']', actual);
        }

        [Test]
        public void GetCorresponding_Array_Close()
        {
            var actual = NameUtils.GetCorresponding(']');
            Assert.AreEqual('[', actual);
        }

        [Test]
        public void GetCorresponding_Pointy_Open()
        {
            var actual = NameUtils.GetCorresponding('<');
            Assert.AreEqual('>', actual);
        }

        [Test]
        public void GetCorresponding_Pointy_Close()
        {
            var actual = NameUtils.GetCorresponding('>');
            Assert.AreEqual('<', actual);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void GetCorresponding_EverythingElse()
        {
            NameUtils.GetCorresponding('x');
        }

        [Test]
        public void FindPrevious_NotFound()
        {
            var actual = NameUtils.FindPrevious("bbb", 1, 'a');
            Assert.AreEqual(-1, actual);
        }

        [Test]
        public void FindCorrespondingCloseBracket_Round()
        {
            var actual = NameUtils.FindCorrespondingCloseBracket("((()))", 1);
            Assert.AreEqual(4, actual);
        }

        [Test]
        public void FindCorrespondingCloseBracket_Courly()
        {
            var actual = NameUtils.FindCorrespondingCloseBracket("{{{}}}", 1);
            Assert.AreEqual(4, actual);
        }

        [Test]
        public void FindCorrespondingCloseBracket_Array()
        {
            var actual = NameUtils.FindCorrespondingCloseBracket("[[[]]]", 1);
            Assert.AreEqual(4, actual);
        }

        [Test]
        public void FindCorrespondingCloseBracket_Pointy()
        {
            var actual = NameUtils.FindCorrespondingCloseBracket("<<<>>>", 1);
            Assert.AreEqual(4, actual);
        }

        [Test]
        public void FindCorrespondingOpenBracket_Round()
        {
            var actual = NameUtils.FindCorrespondingOpenBracket("((()))", 4);
            Assert.AreEqual(1, actual);
        }

        [Test]
        public void FindCorrespondingOpenBracket_Courly()
        {
            var actual = NameUtils.FindCorrespondingOpenBracket("{{{}}}", 4);
            Assert.AreEqual(1, actual);
        }

        [Test]
        public void FindCorrespondingOpenBracket_Array()
        {
            var actual = NameUtils.FindCorrespondingOpenBracket("[[[]]]", 4);
            Assert.AreEqual(1, actual);
        }

        [Test]
        public void FindCorrespondingOpenBracket_Pointy()
        {
            var actual = NameUtils.FindCorrespondingOpenBracket("<<<>>>", 4);
            Assert.AreEqual(1, actual);
        }

        #endregion
    }
}