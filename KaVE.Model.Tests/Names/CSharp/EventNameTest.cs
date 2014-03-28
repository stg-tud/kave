using KaVE.Model.Names.CSharp;
using NUnit.Framework;

namespace KaVE.Model.Tests.Names.CSharp
{
    [TestFixture]
    class EventNameTest
    {
        [Test]
        public void ShouldBeSimpleEvent()
        {
            var eventName = EventName.Get("[ChangedEventHandler, IO, 1.2.3.4] [TextBox, GUI, 0.8.7.6].Changed");

            Assert.AreEqual("ChangedEventHandler", eventName.HandlerType.FullName);
            Assert.AreEqual("TextBox", eventName.DeclaringType.FullName);
            Assert.AreEqual("Changed", eventName.Name);
        }
    }
}
