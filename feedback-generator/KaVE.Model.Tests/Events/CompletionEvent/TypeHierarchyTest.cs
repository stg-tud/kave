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
using KaVE.Model.Events.CompletionEvent;
using NUnit.Framework;

namespace KaVE.Model.Tests.Events.CompletionEvent
{
    [TestFixture]
    internal class TypeHierarchyTest
    {
        private TypeHierarchy _uut;

        [SetUp]
        public void SetUpHierarchyUnderTest()
        {
            _uut = new TypeHierarchy("MyType, MyAssembly")
            {
                Extends = new TypeHierarchy("System.Object, mscorlib, 4.0.0.0")
            };
            _uut.Implements.Add(new TypeHierarchy("ISomeinterface, MyAssembly"));
        }

        [Test]
        public void ShouldEqualACloneOfItself()
        {
            var clone = new TypeHierarchy("MyType, MyAssembly")
            {
                Extends = new TypeHierarchy("System.Object, mscorlib, 4.0.0.0")
            };
            clone.Implements.Add(new TypeHierarchy("ISomeinterface, MyAssembly"));

            Assert.AreEqual(_uut, clone);
            Assert.AreEqual(_uut.GetHashCode(), clone.GetHashCode());
        }

        [Test]
        public void ShouldNotEqualOtherType()
        {
            var other = new TypeHierarchy("OtherType, MyAssembly")
            {
                Extends = new TypeHierarchy("System.Object, mscorlib, 4.0.0.0")
            };

            Assert.AreNotEqual(_uut, other);
            Assert.AreNotEqual(_uut.GetHashCode(), other.GetHashCode());
        }
    }
}