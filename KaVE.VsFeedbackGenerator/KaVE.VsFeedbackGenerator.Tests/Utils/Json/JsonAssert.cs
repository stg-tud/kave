using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils.Json
{
    static class JsonAssert
    {
        private delegate void Assertion(object expected, object actual);

        public static void SerializationPreservesReferenceIdentity<T>(T obj)
        {
            SerializationPreserves(obj, Assert.AreSame);
        }

        public static void SerializationPreservesData<T>(T obj)
        {
            SerializationPreserves(obj, Assert.AreEqual);
        }

        private static void SerializationPreserves<T>(T original, Assertion assertion)
        {
            var clone = SerializeAndDeserialize(original);
            assertion.Invoke(original, clone);
        }

        private static TModel SerializeAndDeserialize<TModel>(TModel model)
        {
            var serialization = model.Serialize();
            var modelCopy = serialization.Deserialize<TModel>();
            return modelCopy;
        }
    }
}