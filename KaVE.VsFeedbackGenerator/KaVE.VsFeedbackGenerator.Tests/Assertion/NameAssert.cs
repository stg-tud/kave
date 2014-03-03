using KaVE.Model.Names;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Assertion
{
    static class NameAssert
    {
        public static void AreEqual(string identifier, IName name)
        {
            Assert.NotNull(name);
            Assert.AreEqual(identifier, name.Identifier);
        }
    }
}
