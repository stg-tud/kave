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

using KaVE.Commons.Utils.Assertion;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.Assertion
{
    internal class AssertsTest
    {
        // NotNull

        [Test, ExpectedException(typeof(AssertException), ExpectedMessage = "is null")]
        public void NotNull_Violation()
        {
            Asserts.NotNull(null);
        }

        [Test]
        public void NotNull_Ok()
        {
            Asserts.NotNull(new object());
        }

        [Test, ExpectedException(typeof(AssertException), ExpectedMessage = "ab")]
        public void NotNull_Violation_Formatted()
        {
            Asserts.NotNull(null, "a{0}", "b");
        }

        [Test]
        public void NotNull_Ok_Formatted()
        {
            Asserts.NotNull(new object(), "a{0}", "b");
        }

        // Null

        [Test, ExpectedException(typeof(AssertException), ExpectedMessage = "is not null")]
        public void Null_Violation()
        {
            Asserts.Null(new object());
        }

        [Test]
        public void Null_Ok()
        {
            Asserts.Null(null);
        }

        [Test, ExpectedException(typeof(AssertException), ExpectedMessage = "ab")]
        public void Null_Violation_Formatted()
        {
            Asserts.Null(new object(), "a{0}", "b");
        }

        [Test]
        public void Null_Ok_Formatted()
        {
            Asserts.Null(null, "a{0}", "b");
        }

        // NotSame

        [Test, ExpectedException(typeof(AssertException), ExpectedMessage = "is the same")]
        public void NotSame_Violation()
        {
            var a = new object();
            Asserts.NotSame(a, a);
        }

        [Test]
        public void NotSame_Ok()
        {
            var a = new object();
            var b = new object();
            Asserts.NotSame(a, b);
        }

        [Test, ExpectedException(typeof(AssertException), ExpectedMessage = "ab")]
        public void NotSame_Violation_Formatted()
        {
            var a = new object();
            Asserts.NotSame(a, a, "a{0}", "b");
        }

        [Test]
        public void NotSame_Ok_Formatted()
        {
            var a = new object();
            var b = new object();
            Asserts.NotSame(a, b, "a{0}", "b");
        }

        // Same

        [Test, ExpectedException(typeof(AssertException), ExpectedMessage = "is not the same")]
        public void Same_Violation()
        {
            var a = new object();
            var b = new object();
            Asserts.Same(a, b);
        }

        [Test]
        public void Same_Ok()
        {
            var a = new object();
            Asserts.Same(a, a);
        }

        [Test, ExpectedException(typeof(AssertException), ExpectedMessage = "ab")]
        public void Same_Violation_Formatted()
        {
            var a = new object();
            var b = new object();
            Asserts.Same(a, b, "a{0}", "b");
        }

        [Test]
        public void Same_Ok_Formatted()
        {
            var a = new object();
            Asserts.Same(a, a, "a{0}", "b");
        }

        // Not

        [Test, ExpectedException(typeof(AssertException), ExpectedMessage = "is true")]
        public void Not_Violation()
        {
            Asserts.Not(true);
        }

        [Test]
        public void Not_Ok()
        {
            Asserts.Not(false);
        }

        [Test, ExpectedException(typeof(AssertException), ExpectedMessage = "ab")]
        public void Not_Violation_Formatted()
        {
            Asserts.Not(true, "a{0}", "b");
        }

        [Test]
        public void Not_Ok_Formatted()
        {
            Asserts.Not(false, "a{0}", "b");
        }

        // That

        [Test, ExpectedException(typeof(AssertException), ExpectedMessage = "is false")]
        public void That_Violation()
        {
            Asserts.That(false);
        }

        [Test]
        public void That_Ok()
        {
            Asserts.That(true);
        }

        [Test, ExpectedException(typeof(AssertException), ExpectedMessage = "ab")]
        public void That_Violation_Formatted()
        {
            Asserts.That(false, "a{0}", "b");
        }

        [Test]
        public void That_Ok_Formatted()
        {
            Asserts.That(true, "a{0}", "b");
        }

        // AreNotEqual

        [Test, ExpectedException(typeof(AssertException), ExpectedMessage = "are equal: a and a")]
        public void AreNotEqual_Violation()
        {
            Asserts.AreNotEqual("a", "a");
        }

        [Test]
        public void AreNotEqual_Ok()
        {
            Asserts.AreNotEqual("a", "b");
        }

        // AreEqual

        [Test, ExpectedException(typeof(AssertException), ExpectedMessage = "are not equal: a and b")]
        public void AreEqual_Violation()
        {
            Asserts.AreEqual("a", "b");
        }

        [Test]
        public void AreEqual_Ok()
        {
            Asserts.AreEqual("a", "a");
        }

        // IsLess

        [Test, ExpectedException(typeof(AssertException), ExpectedMessage = "1 >= 1")]
        public void IsLess_Violation()
        {
            Asserts.IsLess(1, 1);
        }

        [Test, ExpectedException(typeof(AssertException), ExpectedMessage = "2 >= 1")]
        public void IsLess_Violation2()
        {
            Asserts.IsLess(2, 1);
        }

        [Test]
        public void IsLess_Ok()
        {
            Asserts.IsLess(1, 2);
        }

        // IsLessOrEqual

        [Test, ExpectedException(typeof(AssertException), ExpectedMessage = "2 > 1")]
        public void IsLessOrEqual_Violation()
        {
            Asserts.IsLessOrEqual(2, 1);
        }

        [Test]
        public void IsLessOrEqual_Ok()
        {
            Asserts.IsLessOrEqual(1, 1);
        }

        [Test]
        public void IsLessOrEqual_Ok2()
        {
            Asserts.IsLessOrEqual(1, 2);
        }

        // Fail

        [Test, ExpectedException(typeof(AssertException), ExpectedMessage = "unexpected condition")]
        public void Fail()
        {
            Asserts.Fail();
        }

        [Test, ExpectedException(typeof(AssertException), ExpectedMessage = "ab")]
        public void Fail_Formatted()
        {
            Asserts.Fail("a{0}", "b");
        }

        [Test, ExpectedException(typeof(AssertException), ExpectedMessage = "ab")]
        public void Fail_Return_string()
        {
            // ReSharper disable once SuggestVarOrType_BuiltInTypes
            // ReSharper disable once UnusedVariable
            string s = Asserts.Fail<string>("a{0}", "b");
        }

        [Test, ExpectedException(typeof(AssertException), ExpectedMessage = "ab")]
        public void Fail_Return_int()
        {
            // ReSharper disable once SuggestVarOrType_BuiltInTypes
            // ReSharper disable once UnusedVariable
            int i = Asserts.Fail<int>("a{0}", "b");
        }

        [Test, ExpectedException(typeof(AssertException), ExpectedMessage = "ab")]
        public void Fail_Return_float()
        {
            // ReSharper disable once SuggestVarOrType_BuiltInTypes
            // ReSharper disable once UnusedVariable
            float i = Asserts.Fail<float>("a{0}", "b");
        }

        [Test, ExpectedException(typeof(AssertException), ExpectedMessage = "{x}")]
        public void ShouldIgnoreInvalidFormattingWithoutArguments()
        {
            // ReSharper disable once FormatStringProblem
            Asserts.Fail("{x}");
        }
    }
}