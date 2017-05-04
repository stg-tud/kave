/*
 * Copyright 2017 Sebastian Proksch
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
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.FeedbackProcessor.StatisticsUltimate;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.StatisticsUltimate
{
    internal class ContextFilterTest
    {
        private ContextFilter _sut;

        [TestCase(GeneratedCode.Include, Duplication.Include, true),
         TestCase(GeneratedCode.Exclude, Duplication.Include, false),
         TestCase(GeneratedCode.Include, Duplication.Exclude, true),
         TestCase(GeneratedCode.Exclude, Duplication.Exclude, false)]
        public void ShouldProcessGeneratedCore(GeneratedCode g, Duplication d, bool shouldProcess)
        {
            _sut = new ContextFilter(g, d);
            Assert.AreEqual(shouldProcess, _sut.ShouldProcessOrRegister(Gen(1)));
        }

        [TestCase(GeneratedCode.Include, Duplication.Include, true),
         TestCase(GeneratedCode.Exclude, Duplication.Include, true),
         TestCase(GeneratedCode.Include, Duplication.Exclude, false),
         TestCase(GeneratedCode.Exclude, Duplication.Exclude, false)]
        public void ShouldProcessSecondTimeTheSameType(GeneratedCode g, Duplication d, bool shouldProcess)
        {
            _sut = new ContextFilter(g, d);
            Assert.True(_sut.ShouldProcessOrRegister(T(1)));
            Assert.AreEqual(shouldProcess, _sut.ShouldProcessOrRegister(T(1)));
        }

        [TestCase(GeneratedCode.Include, Duplication.Include),
         TestCase(GeneratedCode.Exclude, Duplication.Include),
         TestCase(GeneratedCode.Include, Duplication.Exclude),
         TestCase(GeneratedCode.Exclude, Duplication.Exclude)]
        public void ShouldProcessSecondTimePartialClasses(GeneratedCode g, Duplication d)
        {
            _sut = new ContextFilter(g, d);
            Assert.True(_sut.ShouldProcessOrRegister(Part(1)));
            Assert.True(_sut.ShouldProcessOrRegister(Part(1)));
        }

        [TestCase(GeneratedCode.Include, Duplication.Include, true),
         TestCase(GeneratedCode.Exclude, Duplication.Include, true),
         TestCase(GeneratedCode.Include, Duplication.Exclude, false),
         TestCase(GeneratedCode.Exclude, Duplication.Exclude, false)]
        public void ShouldProcessMethodSecondTime(GeneratedCode g, Duplication d, bool shouldProcess)
        {
            _sut = new ContextFilter(g, d);
            Assert.True(_sut.ShouldProcessOrRegister(M(1)));
            Assert.AreEqual(shouldProcess, _sut.ShouldProcessOrRegister(M(1)));
        }

        [Test]
        public void ToStringInformsAboutConfiguration_a()
        {
            var sut = new ContextFilter(GeneratedCode.Include, Duplication.Exclude);
            Assert.AreEqual("ContextFilter(GeneratedCode.Include, Duplication.Exclude)", sut.ToString());
        }

        [Test]
        public void ToStringInformsAboutConfiguration_b()
        {
            var sut = new ContextFilter(GeneratedCode.Exclude, Duplication.Include);
            Assert.AreEqual("ContextFilter(GeneratedCode.Exclude, Duplication.Include)", sut.ToString());
        }

        private static ISST Gen(int classNum)
        {
            return new SST
            {
                EnclosingType = Names.Type("C{0}, P", classNum),
                PartialClassIdentifier = "C.Designer.cs"
            };
        }

        private static ISST T(int classNum)
        {
            return new SST
            {
                EnclosingType = Names.Type("C{0}, P", classNum)
            };
        }

        private static ISST Part(int classNum)
        {
            return new SST
            {
                EnclosingType = Names.Type("C{0}, P", classNum),
                PartialClassIdentifier = "xy"
            };
        }

        private static IMethodName M(int methodNum)
        {
            return Names.Method("[p:void] [C,P].M{0}()", methodNum);
        }
    }
}