using KaVE.VsFeedbackGenerator.Utils.Json;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils.Json
{
    static class JsonAssert
    {
        private delegate void Assertion(object expected, object actual);

        public static void SerializationPreservesReferenceIdentity<T>(T obj) where T : class
        {
            SerializationPreserves(obj, Assert.AreSame);
        }

        public static void SerializationPreservesData<T>(T obj) where T : class
        {
            SerializationPreserves(obj, Assert.AreEqual);
        }

        private static void SerializationPreserves<T>(T original, Assertion assertion) where T : class
        {
            var clone = SerializeAndDeserialize(original);
            assertion.Invoke(original, clone);
        }

        private static T SerializeAndDeserialize<T>(T model) where T : class
        {
            var json = model.ToCompactJson();
            var modelCopy = json.ParseJsonTo<T>();
            return modelCopy;
        }
    }
}