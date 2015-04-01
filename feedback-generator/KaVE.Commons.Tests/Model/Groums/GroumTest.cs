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

using KaVE.Commons.Model.Groums;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Groums
{
    [TestFixture]
    public class GroumTest
    {
        [Test]
        public void EqualsAndHashCode()
        {
            var a = G(1);
            var b = G(1);
            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(a.GetHashCode() == b.GetHashCode());
        }

        [Test]
        public void EqualsAndHashCode2()
        {
            var a = G(1);
            var b = G(2);
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a.GetHashCode() == b.GetHashCode());
        }

        private static Groum G(int i)
        {
            return new Groum {Name = i};
        }
    }
}