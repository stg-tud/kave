using KaVE.Utils.Serialization;
using Newtonsoft.Json.Converters;
using NUnit.Framework;

namespace KaVE.Utils.Tests.Serialization
{
    [TestFixture]
    public class JsonExtensionsTest
    {
        [Test]
        public void ShouldSerializeToJson()
        {
            var json = (1).ToJson();

            Assert.AreEqual("1", json);
        }

        [Test]
        public void ShouldSerializeWithConverter()
        {
            var json = (new System.DateTime(2013, 11, 26, 11, 38, 00)).ToJson(new IsoDateTimeConverter());

            Assert.AreEqual("\"2013-11-26T11:38:00\"", json);
        }
    }
}
