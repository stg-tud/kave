using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Model.Naming.Types.Organization;
using KaVE.Commons.Utils.TypeShapeIndex;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.TypeShapeIndex
{
    public class TypeShapeIndexUtilTest
    {
        [Test]
        public void ReturnsAssemblyNameAsFileName()
        {
            ITypeName type = Names.Type("System.IDisposable, mscorlib, 4.0.0.0");
            var assemblyFileName = TypeShapeIndexUtil.GetAssemblyFileName(type.Assembly);
            Assert.AreEqual("mscorlib-4.0.0.0", assemblyFileName);
        }
    }
}