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

using NUnit.Framework;

namespace KaVE.Model.Tests.SSTs.SSTTestSuite
{
    [TestFixture, Ignore]
    internal class EntrypointsTest : AbstractSSTTest
    {
        [Test]
        public void Aaaaaa()
        {
            Analyze(@"
                private void PrivateA() {}
                public void PublicA() {}
            ");

            var mA = NewMethodDeclaration("PublicA");

            AssertEntryPoints(mA);
        }

        [Test]
        public void Bbbbbbb()
        {
            Analyze(@"
                private void PrivateA() {}
                public void PublicA() {}
            ");

            var mA = NewMethodDeclaration("PublicA");

            AssertEntryPoints(mA);
        }

        [Test]
        public void Cccccc()
        {
            Analyze(@"
                public void A() { B();}
                public void B() {}
                public void C() {}
            ");

            var mA = NewMethodDeclaration("A");
            var mB = NewMethodDeclaration("B"); // ...
            var mC = NewMethodDeclaration("C");

            AssertEntryPoints(mA, mC);
            AssertMethodDeclaration(mB);
        }

        // ... adapt tests from EntryPointSelectorTest
    }
}