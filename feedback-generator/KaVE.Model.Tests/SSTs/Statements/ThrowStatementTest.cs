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

using KaVE.Model.Names.CSharp;
using KaVE.Model.SSTs.Expressions;
using KaVE.Model.SSTs.Statements;
using NUnit.Framework;

namespace KaVE.Model.Tests.SSTs.Statements
{
    public class ThrowStatementTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new ThrowStatement();
            Assert.IsNull(sut.Exception);
        }

        [Test]
        public void SettingValues()
        {
            var sut = new ThrowStatement { Exception = TypeName.UnknownName };
            Assert.AreEqual(TypeName.UnknownName, sut.Exception);
        }

        [Test]
        public void Equality_default()
        {
            var a = new ThrowStatement();
            var b = new ThrowStatement();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_reallyTheSame()
        {
            var a = new ThrowStatement { Exception = TypeName.UnknownName };
            var b = new ThrowStatement { Exception = TypeName.UnknownName };
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_differentException()
        {
            var a = new ThrowStatement { Exception = TypeName.UnknownName };
            var b = new ThrowStatement { Exception = TypeName.Get("System.Int32, mscore, 4.0.0.0") };
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }
    }
}