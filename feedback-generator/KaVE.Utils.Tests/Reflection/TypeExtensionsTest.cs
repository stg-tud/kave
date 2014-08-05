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
 *    - Sven Amann
 */

using KaVE.JetBrains.Annotations;
using KaVE.Utils.Reflection;
using NUnit.Framework;

namespace KaVE.Utils.Tests.Reflection
{
    [TestFixture]
    internal class TypeExtensionsTest
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private int MyTestProperty { get; set; }

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
         ExpectedException(ExpectedMessage = "Invalid expression type: Expected ExpressionType.MemberAccess, found ExpressionType.Call"
             )]
        public void ShouldFailToGetPropertyNameFromMethod()
        {
            TypeExtensions<TypeExtensionsTest>.GetPropertyName(o => o.Equals(null));
        }

        [Test]
        public void ShouldGetAttributedMember()
        {
            var expected = new[] {typeof (TypeExtensionsTest).GetField("AttributedMember")};
            var actual = typeof(TypeExtensionsTest).GetMembersWithCustomAttributeNoInherit<NotNullAttribute>();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldGetMethodName()
        {
            const string expected = "ShouldGetMethodName";
            var actual = TypeExtensions<TypeExtensionsTest>.GetMethodName(o => o.ShouldGetMethodName());

            Assert.AreEqual(expected, actual);
        }
    }
}