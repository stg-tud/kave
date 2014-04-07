using System.Collections.Generic;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names;
using KaVE.VsFeedbackGenerator.Tests.TestFactories;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils.Json.Model
{
    [TestFixture]
    internal class ContextSerializationTest
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
                TypeShape = new TypeShape
                {
                    TypeHierarchy = GetAnonymousTypeHierarchy(),
                    MethodHierarchies = new HashSet<MethodHierarchy>
                    {
                        new MethodHierarchy(TestNameFactory.GetAnonymousMethodName())
                        {
                            First = TestNameFactory.GetAnonymousMethodName(),
                            Super = TestNameFactory.GetAnonymousMethodName(),
                        }
                    }
                },
                EnclosingMethod = TestNameFactory.GetAnonymousMethodName(),
                CalledMethods = new HashSet<IMethodName>
                {
                    TestNameFactory.GetAnonymousMethodName(),
                    TestNameFactory.GetAnonymousMethodName(),
                    TestNameFactory.GetAnonymousMethodName()
                },
                TriggerTarget = TestNameFactory.GetAnonymousTypeName()
            };
            JsonAssert.SerializationPreservesData(context);
        }

        private static TypeHierarchy GetAnonymousTypeHierarchy()
        {
            return new TypeHierarchy(TestNameFactory.GetAnonymousTypeName().Identifier);
        }
    }
}