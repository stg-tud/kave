using System.Collections.Generic;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names;
using KaVE.VsFeedbackGenerator.Tests.TestFactories;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils.Json.Model
{
    [TestFixture]
    class ContextSerializationTest
    {
        [Test]
        public void ShouldSerializeEmptyContext()
        {
            var context = new Context();
            JsonAssert.SerializationPreservesData(context);
        }

        [Test]
        public void ShouldSerializeAllFieldsOfContext()
        {
            var context = new Context
            {
                EnclosingClassHierarchy = GetAnonymousTypeHierarchy(),
                EnclosingMethod = TestNameFactory.GetAnonymousMethodName(),
                EnclosingMethodFirst = TestNameFactory.GetAnonymousMethodName(),
                EnclosingMethodSuper = TestNameFactory.GetAnonymousMethodName(),
                CalledMethods = new HashSet<IMethodName>
                {
                    TestNameFactory.GetAnonymousMethodName(),
                    TestNameFactory.GetAnonymousMethodName(),
                    TestNameFactory.GetAnonymousMethodName()
                },
            };
            JsonAssert.SerializationPreservesData(context);
        }

        private static TypeHierarchy GetAnonymousTypeHierarchy()
        {
            return new TypeHierarchy(TestNameFactory.GetAnonymousTypeName().Identifier);
        }
    }
}
