using KaVE.Model.Names;
using KaVE.Model.Names.CSharp;

namespace KaVE.VsFeedbackGenerator.Tests.TestFactories
{
    internal static class TestNameFactory
    {
        private static int _counter;

        private static int NextCounter()
        {
            return ++_counter;
        }

        public static IMethodName GetAnonymousMethodName()
        {
            return
                MethodName.Get(
                    string.Format(
                        "[{0}] [{1}].Method{2}()",
                        GetAnonymousTypeName(),
                        GetAnonymousTypeName(),
                        NextCounter()));
        }

        public static ITypeName GetAnonymousTypeName()
        {
            return
                TypeName.Get(
                    string.Format("SomeType{0}, SomeAssembly{1}, Version=9.8.7.6", NextCounter(), NextCounter()));
        }

        public static INamespaceName GetAnonymousNamespace()
        {
            return NamespaceName.Get(string.Format("A.N{0}", NextCounter()));
        }
    }
}