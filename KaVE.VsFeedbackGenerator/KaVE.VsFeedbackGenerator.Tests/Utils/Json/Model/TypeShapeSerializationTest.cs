using System.Collections.Generic;
using KaVE.Model.Events.CompletionEvent;
using KaVE.VsFeedbackGenerator.Tests.TestFactories;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils.Json.Model
{
    [TestFixture]
    internal class TypeShapeSerializationTest
    {
        [Test]
        public void ShouldSerializeTypeShape()
        {
            var uut = new TypeShape
            {
                MethodHierarchies =
                    new HashSet<MethodHierarchy> {GetAnonymousMethodHierarchy(), GetAnonymousMethodHierarchy()},
                TypeHierarchy = new TypeHierarchy("TestClass")
            };
            JsonAssert.SerializationPreservesData(uut);
        }

        private static MethodHierarchy GetAnonymousMethodHierarchy()
        {
            return new MethodHierarchy(TestNameFactory.GetAnonymousMethodName());
        }
    }
}