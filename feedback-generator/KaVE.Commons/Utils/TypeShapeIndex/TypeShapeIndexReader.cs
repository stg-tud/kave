using System.Collections.Generic;
using System.IO;
using System.Linq;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Model.Naming.Types.Organization;
using KaVE.Commons.Model.TypeShapes;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.IO.Archives;

namespace KaVE.Commons.Utils.TypeShapeIndex
{
    public class TypeShapeIndexReader
    {
        private readonly string _path;

        public TypeShapeIndexReader(string path)
        {
            _path = path;
        }

        public ISet<IAssemblyName> GetAllAssemblies()
        {
            var files = Directory.EnumerateFiles(_path);
            var zipFiles = files.Where(s => s.EndsWith(".zip"));
            var result = Sets.NewHashSet<IAssemblyName>();
            foreach (var zipFile in zipFiles)
            {
                var assemblyFileName = Path.GetFileNameWithoutExtension(zipFile);
                if (assemblyFileName != null)
                {
                    var assemblyNameStrings = assemblyFileName.Split('-');
                    var assembly = Names.Assembly($"{assemblyNameStrings[0]}, {assemblyNameStrings[1]}");
                    result.Add(assembly);
                }
            }
            return result;
        }

        public ISet<ITypeName> OpenAssembly(IAssemblyName assemblyName)
        {
            var result = Sets.NewHashSet<ITypeName>();
            var ra = Open(assemblyName);
            if (ra != null)
            {
                var typeShapes = ra.GetAll<ITypeShape>();
                foreach (var typeShape in typeShapes)
                {
                    result.Add(typeShape.TypeHierarchy.Element);
                }
                ra.Dispose();
            }
            return result;
        }

        public ITypeShape OpenTypeShape(ITypeName t)
        {
            var ra = Open(t.Assembly);
            if (ra != null)
            {
                while (ra.HasNext())
                {
                    var typeShape = ra.GetNext<ITypeShape>();
                    if (typeShape.TypeHierarchy.Element.Equals(t))
                    {
                        ra.Dispose();
                        return typeShape;
                    }
                }
                ra.Dispose();
            }
            return new TypeShape();
        }

        private ReadingArchive Open(IAssemblyName asm)
        {
            var fileName = TypeShapeIndexUtil.GetAssemblyFileName(asm);
            var zipPath = GetZipPath(fileName);
            return File.Exists(zipPath) ? new ReadingArchive(zipPath) : null;
        }

        private string GetZipPath(string assemblyFileName)
        {
            return $"{_path}\\{assemblyFileName}.zip";
        }
    }
}