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
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Naming.Impl.v0
{
    internal class LegacyFixesTest
    {
        [Test]
        public void FixesLegacyDelegateTypeNameFormat()
        {
            var actual = new DelegateTypeName("d:Some.DelegateType, A, 1.0.0.0");
            var expected = new DelegateTypeName("d:[?] [Some.DelegateType, A, 1.0.0.0].()");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MissingGenericTicksAreAdded()
        {
            var actual = new TestName("n.C1`1[[T1]]+C2[[T2]]+C3[[T3]], P");
            var expected = new TestName("n.C1`1[[T1]]+C2`0[[T2]]+C3`0[[T3]], P");
            Assert.AreEqual(expected, actual);
        }

        // some fixes are implemented in the base class to make it availabel to both TypeName and TypeParameterName
        // thistestclass only exists to test the base class
        internal class TestName : BaseName
        {
            public TestName(string id) : base(id) {}

            public override bool IsUnknown
            {
                get { return false; }
            }
        }
    }
}