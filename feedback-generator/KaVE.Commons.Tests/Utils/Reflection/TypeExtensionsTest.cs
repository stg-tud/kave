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

using System;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Reflection;
using KaVE.JetBrains.Annotations;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.Reflection
{
    [TestFixture]
    internal class TypeExtensionsTest
    {
        public int MyTestProperty { get; set; }

        private bool MyPrivateProperty { get; set; }

        private int _myField;

        public event EventHandler MyEvent;

        [NotNull, UsedImplicitly]
        public string AttributedMember;

        [Test]
        public void ShouldGetPropertyName()
        {
            var actual = TypeExtensions<TypeExtensionsTest>.GetPropertyName(o => o.MyTestProperty);

            Assert.AreEqual("MyTestProperty", actual);
        }

        [Test(Description = "In some cases an implicit cast to object is introduced; this is tested explicitly here.")]
        public void ShouldGetPropertyWithCast()
        {
            var actual = TypeExtensions<TypeExtensionsTest>.GetPropertyName(o => (object) o.MyTestProperty);

            Assert.AreEqual("MyTestProperty", actual);
        }

        [Test,
         ExpectedException(
             ExpectedMessage =
                 "Invalid expression type: Expected ExpressionType.MemberAccess, found ExpressionType.Call"
             )]
        public void ShouldFailToGetPropertyNameFromMethod()
        {
            TypeExtensions<TypeExtensionsTest>.GetPropertyName(o => o.Equals(null));
        }

        [Test]
        public void ShouldGetAttributedMember()
        {
            var expected = new[] {typeof (TypeExtensionsTest).GetField("AttributedMember")};
            var actual = typeof (TypeExtensionsTest).GetMembersWithCustomAttributeNoInherit<NotNullAttribute>();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldGetMethodName()
        {
            const string expected = "ShouldGetMethodName";
            var actual = TypeExtensions<TypeExtensionsTest>.GetMethodName(o => o.ShouldGetMethodName());

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldGetPublicPropertyValue()
        {
            MyTestProperty = 42;

            var actual = this.GetPublicPropertyValue<int>("MyTestProperty");

            Assert.AreEqual(MyTestProperty, actual);
        }

        [Test,
         ExpectedException(typeof (AssertException),
             ExpectedMessage =
                 "Property 'NonExistentProperty' doesn't exist on 'KaVE.Commons.Tests.Utils.Reflection.TypeExtensionsTest'.")]
        public void ShouldThrowIfPublicPropertyDoesNotExist()
        {
            this.GetPublicPropertyValue<int>("NonExistentProperty");
        }

        [Test]
        public void ShouldGetPrivatePropertyValue()
        {
            MyPrivateProperty = true;

            var actual = this.GetPrivatePropertyValue<bool>("MyPrivateProperty");

            Assert.AreEqual(MyPrivateProperty, actual);
        }

        [Test,
         ExpectedException(typeof (AssertException),
             ExpectedMessage =
                 "Property 'NonExistentProperty' doesn't exist on 'KaVE.Commons.Tests.Utils.Reflection.TypeExtensionsTest'.")]
        public void ShouldThrowIfPrivatePropertyDoesNotExist()
        {
            this.GetPrivatePropertyValue<int>("NonExistentProperty");
        }

        [Test]
        public void ShouldGetPrivateFieldValue()
        {
            _myField = 23;

            var actual = this.GetPrivateFieldValue<int>("_myField");

            Assert.AreEqual(_myField, actual);
        }

        [Test,
         ExpectedException(typeof (AssertException),
             ExpectedMessage =
                 "Field '_myNonExistentField' doesn't exist on 'KaVE.Commons.Tests.Utils.Reflection.TypeExtensionsTest'.")]
        public void ShouldThrowIfPrivateFieldDoesNotExist()
        {
            this.GetPrivateFieldValue<int>("_myNonExistentField");
        }

        [Test]
        public void ShouldRegisterToEvent()
        {
            var invoked = false;

            this.RegisterToEvent("MyEvent", (EventHandler) delegate { invoked = true; });
            MyEvent(this, new EventArgs());

            Assert.IsTrue(invoked);
        }

        [Test,
         ExpectedException(typeof(AssertException),
             ExpectedMessage =
                 "Event 'MyNonExistentEvent' doesn't exist on 'KaVE.Commons.Tests.Utils.Reflection.TypeExtensionsTest'.")]
        public void ShouldThrowIfEventDoesNotExist()
        {
            this.RegisterToEvent("MyNonExistentEvent", null);
        }
    }
}