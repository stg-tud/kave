using System;
using System.Collections.Generic;
using System.IO;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Model.TypeShapes;
using KaVE.Commons.TestUtils;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.TypeShapeIndex;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.TypeShapeIndex
{
    public class TypeShapeIndexReaderTest : FileBasedTestBase
    {
        private TypeShapeIndexReader _uut;

        [SetUp]
        public void Setup()
        {
            _uut = new TypeShapeIndexReader(DirTestRoot);
            PrepareAssemblyZips();
        }

        [Test]
        public void ReturnsAllAssemblies()
        {
            var allAssemblies = _uut.GetAllAssemblies();
            var expected = Sets.NewHashSet(
                Names.Assembly("Assembly1, 0.0.0.0"),
                Names.Assembly("Assembly2, 0.0.0.0"),
                Names.Assembly("Assembly3, 0.0.0.0")
            );

            Assert.AreEqual(expected, allAssemblies);
        }

        [Test]
        public void ReturnsAllTypesInAssembly()
        {
            var assemblyName = Names.Assembly("Assembly1, 0.0.0.0");
            var typeNames = _uut.OpenAssembly(assemblyName);
            var expected = Sets.NewHashSet(
                GetTypeName("T1", "Assembly1"),
                GetTypeName("T2", "Assembly1"),
                GetTypeName("T3", "Assembly1")
            );
            Assert.AreEqual(expected, typeNames);
        }

        [Test]
        public void ReturnsEmptyTypeListIfAssemblyDoesNotExist()
        {
            var assemblyName = Names.Assembly("SomeOtherAssembly, 0.0.0.0");
            var typeNames = _uut.OpenAssembly(assemblyName);
            var expected = Sets.NewHashSet<ITypeName>();
            Assert.AreEqual(expected, typeNames);
        }

        [Test]
        public void ReturnsTypeShapeForType()
        {
            var typeName = GetTypeName("T1", "Assembly1");
            var typeShape = _uut.OpenTypeShape(typeName);
            var expected = GetTypeShape("T1", "Assembly1");
            Assert.AreEqual(expected, typeShape);
        }

        [Test]
        public void ReturnsEmptyTypeShapeIfTypeIsNotInAssembly()
        {
            var typeName = GetTypeName("SomeOtherType", "Assembly1");
            var typeShape = _uut.OpenTypeShape(typeName);
            var expected = new TypeShape();
            Assert.AreEqual(expected, typeShape);
        }

        private static TypeShape GetTypeShape(string type, string assembly1)
        {
            return new TypeShape
            {
                TypeHierarchy = new TypeHierarchy
                {
                    Element = GetTypeName(type, assembly1)
                }
            };
        }

        private void PrepareAssemblyZips()
        {
            string assembly1 = "Assembly1";
            string assembly2 = "Assembly2";
            string assembly3 = "Assembly3";
            WriteZip(
                $"{DirTestRoot}\\{assembly1}-0.0.0.0.zip",
                GetTypeShape("T1", assembly1),
                GetTypeShape("T2", assembly1),
                GetTypeShape("T3", assembly1)
            );
            WriteZip(
                $"{DirTestRoot}\\{assembly2}-0.0.0.0.zip",
                GetTypeShape("T4", assembly2)
            );
            WriteZip(
                $"{DirTestRoot}\\{assembly3}-0.0.0.0.zip",
                GetTypeShape("T5", assembly3)
            );
        }

        private static ITypeName GetTypeName(string name, string assembly)
        {
            return Names.Type("{0}, {1}, 0.0.0.0", name, assembly);
        }
    }
}