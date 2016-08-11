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

using KaVE.Commons.Model.Naming.Impl.v0;
using KaVE.Commons.Model.Naming.Impl.v0.Types;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Utils.Exceptions;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Naming.Impl.v0
{
    internal class GeneralNameTest
    {
        [Test]
        public void ShouldRecognizeUnknownName()
        {
            Assert.True(new GeneralName().IsUnknown);
            Assert.True(new GeneralName("???").IsUnknown);
            Assert.False(new GeneralName("x").IsUnknown);
        }

        [Test]
        public void ShouldDifferentiateEqualityOnRuntimeTypeToo()
        {
            var a = new PredefinedTypeName("p:int");
            var b = new GeneralName("p:int");
            Assert.AreNotEqual(a, b);
        }

        [Test, Ignore("not sure about the original motivation behind this test")]
        public void AllUnknownTypesAreEqual()
        {
            var unknowns = new ITypeName[] {new TypeName(), new DelegateTypeName()};

            foreach (var u1 in unknowns)
            {
                foreach (var u2 in unknowns)
                {
                    Assert.AreEqual(u1, u2);
                    Assert.AreEqual(u2, u1);
                }
            }
        }

        [Test]
        public void UnknownTypesAreNotEqualToOtherNames()
        {
            var unknowns = new ITypeName[] {new TypeName(), new DelegateTypeName()};

            foreach (var u in unknowns)
            {
                Assert.AreNotEqual(u, new GeneralName());
                Assert.AreNotEqual(new GeneralName(), u);
            }
        }

        [Test, ExpectedException(typeof(ValidationException))]
        public void ShouldAvoidNullParameters()
        {
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            new GeneralName(null);
        }

        [Test]
        public void ShouldImplementIsHashed()
        {
            Assert.IsTrue(new GeneralName("72launbJW34oSO9wR5XBdw==").IsHashed);
            Assert.IsFalse(new GeneralName("x").IsHashed);
        }

        [Test]
        public void ShouldNotChangeOrCloneIdentfier()
        {
            var expected = "x";
            var actual = new GeneralName(expected).Identifier;
            Assert.AreSame(expected, actual);
        }
    }
}