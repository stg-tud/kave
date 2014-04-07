using KaVE.Model.Events.CompletionEvent;
using KaVE.VsFeedbackGenerator.Tests.TestFactories;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils.Json.Model
{
    [TestFixture]
    class MethodHierarchySerializationTest
    {
        [Test]
        public void ShouldSerializeMethodHierarchy()
        {
            var uut = new MethodHierarchy(TestNameFactory.GetAnonymousMethodName())
            {
                First = TestNameFactory.GetAnonymousMethodName(),
                Super = TestNameFactory.GetAnonymousMethodName()
            };
            JsonAssert.SerializationPreservesData(uut);
        }
    }
}
