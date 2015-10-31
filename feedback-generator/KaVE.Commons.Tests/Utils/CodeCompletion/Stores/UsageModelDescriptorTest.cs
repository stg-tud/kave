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

using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.TestUtils;
using KaVE.Commons.Utils.CodeCompletion.Stores;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.CodeCompletion.Stores
{
    internal class UsageModelDescriptorTest
    {
        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new UsageModelDescriptor(new CoReTypeName("LTest"), 1);
            var b = new UsageModelDescriptor(new CoReTypeName("LTest"), 1);
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentTypeNames()
        {
            var a = new UsageModelDescriptor(new CoReTypeName("LA"), 0);
            var b = new UsageModelDescriptor(new CoReTypeName("LB"), 0);
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentVersions()
        {
            var a = new UsageModelDescriptor(new CoReTypeName("LTest"), 1);
            var b = new UsageModelDescriptor(new CoReTypeName("LTest"), 2);
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(new UsageModelDescriptor(new CoReTypeName("LTest"), 0));
        }
    }
}