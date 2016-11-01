using KaVE.Commons.Model.Naming.Types.Organization;

namespace KaVE.Commons.Utils.TypeShapeIndex
{
    public static class TypeShapeIndexUtil
    {
        public static string GetAssemblyFileName(IAssemblyName asm)
        {
            var assemblyVersion = asm.Version;
            string versionString;
            if (assemblyVersion.IsUnknown)
            {
                versionString = "0.0.0.0";
            }
            else
            {
                versionString = assemblyVersion.Identifier;
            }
            return asm.Name + "-" + versionString;
        }
    }
}