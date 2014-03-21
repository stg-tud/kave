using KaVE.Utils.Reflection;
using NUnit.Framework;

namespace KaVE.Utils.Tests.Reflection
{
    [TestFixture]
    internal class TypeExceptionsTest
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private int MyTestProperty { get; set; }

        [Test]
        public void ShouldGetPropertyName()
        {
            var actual = TypeExtensions<TypeExceptionsTest>.GetPropertyName(o => o.MyTestProperty);

            Assert.AreEqual("MyTestProperty", actual);
        }

        [Test(Description = "In some cases an implicit cast to object is introduced; this is tested explicitly here.")]
        public void ShouldGetPropertyWithCast()
        {
            var actual = TypeExtensions<TypeExceptionsTest>.GetPropertyName(o => (object) o.MyTestProperty);

            Assert.AreEqual("MyTestProperty", actual);
        }

        [Test,
         ExpectedException(ExpectedMessage = "Invalid expression type: Expected ExpressionType.MemberAccess, Found Call"
             )]
        public void ShouldFailToGet()
        {
            TypeExtensions<TypeExceptionsTest>.GetPropertyName(o => o.Equals(null));
        }
    }
}