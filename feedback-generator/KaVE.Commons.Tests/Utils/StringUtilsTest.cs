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
using System.Globalization;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Assertion;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils
{
    internal class StringUtilsTest
    {
        [Test]
        public void SimpleRoundTrip()
        {
            const string expected = "asd";
            var actual = expected.AsBytes().AsString();
            Assert.AreEqual(expected, actual);
        }

        [TestCase("myfoobar", true),
         TestCase("myFoobar", true),
         TestCase("myFOObar", true),
         TestCase("myfOObar", true),
         TestCase("myf00bar", false),
         TestCase("myfobar", false)]
        public void ContainsIgnoreCase(string value, bool expected)
        {
            Assert.AreEqual(expected, value.Contains("foo", CompareOptions.IgnoreCase));
        }

        [TestCase(new[] {"foo"}, true),
         TestCase(new[] {"wut"}, false),
         TestCase(new[] {"some", "other", "foo"}, true),
         TestCase(new[] {"but", "not", "this", "time"}, false)]
        public void ContainsAny(string[] needles, bool expected)
        {
            Assert.AreEqual(expected, "myfoobar".ContainsAny(needles));
        }

        [Test]
        public void Format()
        {
            var actual = "a{0}c".FormatEx("b");
            var expected = "abc";
            Assert.AreEqual(expected, actual);
        }

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
            var actual = "ccccba".FindNext(1, 'b', 'a');
            Assert.AreEqual(4, actual);
        }

        [Test]
        public void FindNext_NotFound()
        {
            var actual = "abbb".FindNext(1, 'a');
            Assert.AreEqual(-1, actual);
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void FindNext_IndexTooLow()
        {
            "abc".FindNext(-1, 'a');
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void FindNext_IndexTooHigh()
        {
            "abc".FindNext(3, 'a');
        }

        [Test]
        public void FindPrevious()
        {
            var actual = "abcabcabc".FindPrevious(5, 'a');
            Assert.AreEqual(3, actual);
        }

        [Test]
        public void FindPrevious_arr()
        {
            var actual = "abcabcabc".FindPrevious(5, 'a', 'b');
            Assert.AreEqual(4, actual);
        }

        [Test]
        public void FindPrevious_arr2()
        {
            var actual = "abcabcabc".FindPrevious(5, 'b', 'a');
            Assert.AreEqual(4, actual);
        }

        [Test]
        public void FindPrevious_NotFound()
        {
            var actual = "abcabcabc".FindPrevious(5, 'x');
            Assert.AreEqual(-1, actual);
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void FindPrevious_IndexTooLow()
        {
            "abc".FindPrevious(-1, 'a');
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void FindPrevious_IndexTooHigh()
        {
            "abc".FindPrevious(3, 'a');
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

        [Test, ExpectedException(typeof(AssertException))]
        public void FindCorrespondingCloseBracket_null()
        {
            ((string) null).FindCorrespondingCloseBracket(0);
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void FindCorrespondingCloseBracket_indexTooLow()
        {
            "x".FindCorrespondingCloseBracket(-1);
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void FindCorrespondingCloseBracket_indexTooHigh()
        {
            "x".FindCorrespondingCloseBracket(1);
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void FindCorrespondingCloseBracket_noBracket()
        {
            "x".FindCorrespondingCloseBracket(0);
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

        [Test, ExpectedException(typeof(AssertException))]
        public void FindCorrespondingOpenBracket_null()
        {
            ((string) null).FindCorrespondingOpenBracket(0);
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void FindCorrespondingOpenBracket_indexTooLow()
        {
            "x".FindCorrespondingOpenBracket(-1);
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void FindCorrespondingOpenBracket_indexTooHigh()
        {
            "x".FindCorrespondingOpenBracket(1);
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void FindCorrespondingOpenBracket_noBracket()
        {
            "x".FindCorrespondingOpenBracket(0);
        }

        [Test]
        public void TakeUntil_noChar()
        {
            const string strIn = "abcd";
            var actual = strIn.TakeUntil();
            const string expected = "abcd";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TakeUntil_singleChar()
        {
            const string strIn = "abcd";
            var actual = strIn.TakeUntil('b');
            const string expected = "a";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TakeUntil_multiChar()
        {
            const string strIn = "abcd";
            var actual = strIn.TakeUntil('b', 'c');
            const string expected = "a";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TakeUntil_notFound()
        {
            const string strIn = "abcd";
            var actual = strIn.TakeUntil('x');
            const string expected = "abcd";
            Assert.AreEqual(expected, actual);
        }
    }
}