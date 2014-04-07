using System.Collections.Generic;
using KaVE.Model.Events.CompletionEvent;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils.Json.Model
{
    [TestFixture]
    class TypeHierarchySerializationTest
    {
        [Test]
        public void ShouldSerializeTypeHierarchy()
        {
            var uut = new TypeHierarchy("Test.Class")
            {
                Extends = new TypeHierarchy("Another.Test.Class"),
                Implements = new HashSet<ITypeHierarchy>
                {
                    new TypeHierarchy("Some.Interface")
                }
            };
            JsonAssert.SerializationPreservesData(uut);
        }
    }
}
