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
 *    - Uli Fahrer
 */

using KaVE.Model.ObjectUsage;
using KaVE.Utils.Assertion;
using NUnit.Framework;

namespace KaVE.Model.Tests.ObjectUsage
{
    [TestFixture]
    internal class CallSitesTest
    {
        [Test, ExpectedException(typeof (AssertException))]
        public void NullValueReceiverCallSite()
        {
            CallSites.CreateReceiverCallSite(null);
        }

        [Test, ExpectedException(typeof (AssertException))]
        public void NullValueParameterCallSite()
        {
            CallSites.CreateParameterCallSite(null, 0);
        }

        [Test]
        public void ReceiverCallSiteIsCorrectInitialized()
        {
            var actual = CallSites.CreateReceiverCallSite("LType.method(LArgument;)LReturn;");
            var expected = new CallSite
            {
                kind = CallSiteKind.RECEIVER,
                method = new CoReMethodName("LType.method(LArgument;)LReturn;")
            };

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ParameterCallSiteIsCorrectInitialized()
        {
            var actual = CallSites.CreateParameterCallSite("LType.method(LArgument;)LReturn;", 0);
            var expected = new CallSite
            {
                kind = CallSiteKind.PARAMETER,
                method = new CoReMethodName("LType.method(LArgument;)LReturn;"),
                argIndex = 0
            };

            Assert.AreEqual(expected, actual);
        }
    }
}