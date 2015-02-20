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

using KaVE.Model.SSTs.Statements.Wrapped;
using NUnit.Framework;

namespace KaVE.Model.Tests.SSTs.Statements.Wrapped
{
    public class MemberAccessStatementTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new MemberAccessStatement();
            Assert.IsNull(sut.Target);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues_Helper1()
        {
            var sut = MemberAccessStatement.Create("i", "m");
            Assert.NotNull(sut);
            // TODO adapt test
        }

        [Test]
        public void TargetDoesNotChange()
        {
            var sut = new MemberAccessStatement();
            Assert.AreSame(sut.Target, sut.MemberAccess);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new MemberAccessStatement();
            var b = new MemberAccessStatement();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = MemberAccessStatement.Create("i", "m");
            var b = MemberAccessStatement.Create("i", "m");
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentTarget()
        {
            var a = MemberAccessStatement.Create("i", "m");
            var b = new MemberAccessStatement();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }
    }
}