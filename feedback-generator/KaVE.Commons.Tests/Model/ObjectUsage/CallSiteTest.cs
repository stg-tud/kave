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

using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.TestUtils;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.ObjectUsage
{
    internal class CallSiteTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new CallSite();
            Assert.AreEqual(CallSiteKind.RECEIVER, sut.kind);
            Assert.AreEqual(0, sut.argIndex);
            Assert.AreEqual(Names.UnknownMethod.ToCoReName(), sut.method);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new CallSite
            {
                kind = CallSiteKind.PARAMETER,
                argIndex = 3,
                method = Names.Method("[T1,P] [T2,P].M()").ToCoReName()
            };
            Assert.AreEqual(CallSiteKind.PARAMETER, sut.kind);
            Assert.AreEqual(3, sut.argIndex);
            Assert.AreEqual(Names.Method("[T1,P] [T2,P].M()").ToCoReName(), sut.method);
        }

        [Test]
        public void ShouldRecognizeEqualCallSites()
        {
            Assert.AreEqual(
                CallSites.CreateParameterCallSite("LReceiver.method(LArgument;)LReturn;", 2),
                CallSites.CreateParameterCallSite("LReceiver.method(LArgument;)LReturn;", 2));
        }

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(new CallSite());
        }
    }
}