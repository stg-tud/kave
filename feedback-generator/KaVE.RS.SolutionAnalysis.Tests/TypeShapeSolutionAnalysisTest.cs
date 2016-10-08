using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.ReSharper.TestFramework;
using JetBrains.Util;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Types.Organization;
using KaVE.Commons.Model.TypeShapes;
using KaVE.Commons.TestUtils.Utils.Exceptions;
using KaVE.Commons.Utils.Collections;
using NUnit.Framework;

namespace KaVE.RS.SolutionAnalysis.Tests
{
    internal class TypeShapeSolutionAnalysisTest : BaseTestWithExistingSolution
    {
        protected override FileSystemPath ExistingSolutionFilePath
        {
            get { return GetTestDataFilePath2(Path.Combine("..", "TestSolution", "TestSolution.sln")); }
        }

        [Test]
        public void DoesNotAnalyzeProjectDependencies()
        {
            var results = RunAnalysis();

            var assemblyNames = results.AssemblyNames;
            CollectionAssert.DoesNotContain(assemblyNames, Names.Assembly("Project1"));
        }

        [Test]
        public void AnalyzesAllStandardReferences()
        {
            var results = RunAnalysis();

            var assemblyNames = results.AssemblyNames;
            CollectionAssert.IsSubsetOf(
                new List<IAssemblyName>
                {
                    Names.Assembly("System, 4.0.0.0"),
                    Names.Assembly("System.Core, 4.0.0.0"),
                    Names.Assembly("System.Xml.Linq, 4.0.0.0"),
                    Names.Assembly("System.Data.DataSetExtensions, 4.0.0.0"),
                    Names.Assembly("Microsoft.CSharp, 4.0.0.0"),
                    Names.Assembly("System.Data, 4.0.0.0"),
                    Names.Assembly("System.Xml, 4.0.0.0"),
                    Names.Assembly("mscorlib, 4.0.0.0")
                },
                assemblyNames);
        }

        [Test]
        public void AnalyzesNugetDependency()
        {
            var results = RunAnalysis();

            var assemblyNames = results.AssemblyNames;
            CollectionAssert.Contains(assemblyNames, Names.Assembly("Newtonsoft.Json, 6.0.0.0"));
        }

        [Test]
        public void ContainsOneExampleTypeFromStandardDependency()
        {
            var results = RunAnalysis();

            var typeShapeIndex = results.TypeShapeIndex;
            var assemblyName = Names.Assembly("System, 4.0.0.0");
            CollectionAssert.Contains(results.AssemblyNames, assemblyName);
            var expectedType = Names.Type("System.UriTypeConverter, System, 4.0.0.0");
            var expectedTypeShape = new TypeShape
            {
                TypeHierarchy = new TypeHierarchy
                {
                    Element = expectedType,
                    Extends = new TypeHierarchy("System.ComponentModel.TypeConverter, System, 4.0.0.0")
                },
                MethodHierarchies =
                {
                    new MethodHierarchy(Names.Method("[p:void] [System.UriTypeConverter, System, 4.0.0.0]..ctor()")),
                    new MethodHierarchy
                    {
                        Element =
                            Names.Method(
                                "[p:bool] [System.UriTypeConverter, System, 4.0.0.0].CanConvertFrom([i:System.ComponentModel.ITypeDescriptorContext, System, 4.0.0.0] context, [System.Type, mscorlib, 4.0.0.0] sourceType)"),
                        Super =
                            Names.Method(
                                "[p:bool] [System.ComponentModel.TypeConverter, System, 4.0.0.0].CanConvertFrom([i:System.ComponentModel.ITypeDescriptorContext, System, 4.0.0.0] context, [System.Type, mscorlib, 4.0.0.0] sourceType)")
                    },
                    new MethodHierarchy
                    {
                        Element =
                            Names.Method(
                                "[p:bool] [System.UriTypeConverter, System, 4.0.0.0].CanConvertTo([i:System.ComponentModel.ITypeDescriptorContext, System, 4.0.0.0] context, [System.Type, mscorlib, 4.0.0.0] destinationType)"),
                        Super =
                            Names.Method(
                                "[p:bool] [System.ComponentModel.TypeConverter, System, 4.0.0.0].CanConvertTo([i:System.ComponentModel.ITypeDescriptorContext, System, 4.0.0.0] context, [System.Type, mscorlib, 4.0.0.0] destinationType)")
                    },
                    new MethodHierarchy
                    {
                        Element =
                            Names.Method(
                                "[p:object] [System.UriTypeConverter, System, 4.0.0.0].ConvertFrom([i:System.ComponentModel.ITypeDescriptorContext, System, 4.0.0.0] context, [System.Globalization.CultureInfo, mscorlib, 4.0.0.0] culture, [p:object] value)"),
                        Super =
                            Names.Method(
                                "[p:object] [System.ComponentModel.TypeConverter, System, 4.0.0.0].ConvertFrom([i:System.ComponentModel.ITypeDescriptorContext, System, 4.0.0.0] context, [System.Globalization.CultureInfo, mscorlib, 4.0.0.0] culture, [p:object] value)")
                    },
                    new MethodHierarchy
                    {
                        Element =
                            Names.Method(
                                "[p:object] [System.UriTypeConverter, System, 4.0.0.0].ConvertTo([i:System.ComponentModel.ITypeDescriptorContext, System, 4.0.0.0] context, [System.Globalization.CultureInfo, mscorlib, 4.0.0.0] culture, [p:object] value, [System.Type, mscorlib, 4.0.0.0] destinationType)"),
                        Super =
                            Names.Method(
                                "[p:object] [System.ComponentModel.TypeConverter, System, 4.0.0.0].ConvertTo([i:System.ComponentModel.ITypeDescriptorContext, System, 4.0.0.0] context, [System.Globalization.CultureInfo, mscorlib, 4.0.0.0] culture, [p:object] value, [System.Type, mscorlib, 4.0.0.0] destinationType)")
                    },
                    new MethodHierarchy
                    {
                        Element =
                            Names.Method(
                                "[p:bool] [System.UriTypeConverter, System, 4.0.0.0].IsValid([i:System.ComponentModel.ITypeDescriptorContext, System, 4.0.0.0] context, [p:object] value)"),
                        Super =
                            Names.Method(
                                "[p:bool] [System.ComponentModel.TypeConverter, System, 4.0.0.0].IsValid([i:System.ComponentModel.ITypeDescriptorContext, System, 4.0.0.0] context, [p:object] value)")
                    }
                }
            };

            var typeShapes = typeShapeIndex[assemblyName];
            CollectionAssert.Contains(typeShapes, expectedTypeShape);
        }

        [Test]
        public void ContainsOneExampleTypeFromNugetDependency()
        {
            var results = RunAnalysis();

            var typeShapeIndex = results.TypeShapeIndex;
            var assemblyName = Names.Assembly("Newtonsoft.Json, 6.0.0.0");
            CollectionAssert.Contains(results.AssemblyNames, assemblyName);

            var expectedType = Names.Type("Newtonsoft.Json.Converters.BinaryConverter, Newtonsoft.Json, 6.0.0.0");
            var expectedTypeShape = new TypeShape
            {
                TypeHierarchy = new TypeHierarchy
                {
                    Element = expectedType,
                    Extends = new TypeHierarchy("Newtonsoft.Json.JsonConverter, Newtonsoft.Json, 6.0.0.0")
                },
                Fields =
                {
                    Names.Field(
                        "static [p:string] [Newtonsoft.Json.Converters.BinaryConverter, Newtonsoft.Json, 6.0.0.0].BinaryTypeName"),
                    Names.Field(
                        "static [p:string] [Newtonsoft.Json.Converters.BinaryConverter, Newtonsoft.Json, 6.0.0.0].BinaryToArrayName"),
                    Names.Field(
                        "[Newtonsoft.Json.Utilities.ReflectionObject, Newtonsoft.Json, 6.0.0.0] [Newtonsoft.Json.Converters.BinaryConverter, Newtonsoft.Json, 6.0.0.0]._reflectionObject")
                },
                MethodHierarchies =
                {
                    new MethodHierarchy(
                        Names.Method(
                            "[p:void] [Newtonsoft.Json.Converters.BinaryConverter, Newtonsoft.Json, 6.0.0.0]..ctor()")),
                    new MethodHierarchy
                    {
                        Element =
                            Names.Method(
                                "[p:void] [Newtonsoft.Json.Converters.BinaryConverter, Newtonsoft.Json, 6.0.0.0].WriteJson([Newtonsoft.Json.JsonWriter, Newtonsoft.Json, 6.0.0.0] writer, [p:object] value, [Newtonsoft.Json.JsonSerializer, Newtonsoft.Json, 6.0.0.0] serializer)"),
                        Super =
                            Names.Method(
                                "[p:void] [Newtonsoft.Json.JsonConverter, Newtonsoft.Json, 6.0.0.0].WriteJson([Newtonsoft.Json.JsonWriter, Newtonsoft.Json, 6.0.0.0] writer, [p:object] value, [Newtonsoft.Json.JsonSerializer, Newtonsoft.Json, 6.0.0.0] serializer)")
                    },
                    new MethodHierarchy(
                        Names.Method(
                            "[p:byte[]] [Newtonsoft.Json.Converters.BinaryConverter, Newtonsoft.Json, 6.0.0.0].GetByteArray([p:object] value)")),
                    new MethodHierarchy(
                        Names.Method(
                            "[p:void] [Newtonsoft.Json.Converters.BinaryConverter, Newtonsoft.Json, 6.0.0.0].EnsureReflectionObject([System.Type, mscorlib, 4.0.0.0] t)")),
                    new MethodHierarchy
                    {
                        Element =
                            Names.Method(
                                "[p:object] [Newtonsoft.Json.Converters.BinaryConverter, Newtonsoft.Json, 6.0.0.0].ReadJson([Newtonsoft.Json.JsonReader, Newtonsoft.Json, 6.0.0.0] reader, [System.Type, mscorlib, 4.0.0.0] objectType, [p:object] existingValue, [Newtonsoft.Json.JsonSerializer, Newtonsoft.Json, 6.0.0.0] serializer)"),
                        Super =
                            Names.Method(
                                "[p:object] [Newtonsoft.Json.JsonConverter, Newtonsoft.Json, 6.0.0.0].ReadJson([Newtonsoft.Json.JsonReader, Newtonsoft.Json, 6.0.0.0] reader, [System.Type, mscorlib, 4.0.0.0] objectType, [p:object] existingValue, [Newtonsoft.Json.JsonSerializer, Newtonsoft.Json, 6.0.0.0] serializer)")
                    },
                    new MethodHierarchy(
                        Names.Method(
                            "[p:byte[]] [Newtonsoft.Json.Converters.BinaryConverter, Newtonsoft.Json, 6.0.0.0].ReadByteArray([Newtonsoft.Json.JsonReader, Newtonsoft.Json, 6.0.0.0] reader)")),
                    new MethodHierarchy
                    {
                        Element =
                            Names.Method(
                                "[p:bool] [Newtonsoft.Json.Converters.BinaryConverter, Newtonsoft.Json, 6.0.0.0].CanConvert([System.Type, mscorlib, 4.0.0.0] objectType)"),
                        Super =
                            Names.Method(
                                "[p:bool] [Newtonsoft.Json.JsonConverter, Newtonsoft.Json, 6.0.0.0].CanConvert([System.Type, mscorlib, 4.0.0.0] objectType)")
                    }
                }
            };
            var typeShapes = typeShapeIndex[assemblyName];
            CollectionAssert.Contains(typeShapes, expectedTypeShape);
        }

        [Test]
        public void AnalyzesNestedTypes()
        {
            var results = RunAnalysis();

            var typeShapeIndex = results.TypeShapeIndex;
            var assemblyName = Names.Assembly("Newtonsoft.Json, 6.0.0.0");
            CollectionAssert.Contains(results.AssemblyNames, assemblyName);
            var nestedTypeName = "e:Newtonsoft.Json.JsonReader+State, Newtonsoft.Json, 6.0.0.0";
            var expectedTypeShape = new TypeShape
            {
                TypeHierarchy =
                    new TypeHierarchy(nestedTypeName),
                Fields =
                {
                    EnumField(nestedTypeName, "Start"),
                    EnumField(nestedTypeName, "Complete"),
                    EnumField(nestedTypeName, "Property"),
                    EnumField(nestedTypeName, "ObjectStart"),
                    EnumField(nestedTypeName, "Object"),
                    EnumField(nestedTypeName, "ArrayStart"),
                    EnumField(nestedTypeName, "Array"),
                    EnumField(nestedTypeName, "Closed"),
                    EnumField(nestedTypeName, "PostValue"),
                    EnumField(nestedTypeName, "ConstructorStart"),
                    EnumField(nestedTypeName, "Constructor"),
                    EnumField(nestedTypeName, "Error"),
                    EnumField(nestedTypeName, "Finished")
                }
            };
            var typeShapes = typeShapeIndex[assemblyName];
            CollectionAssert.Contains(typeShapes, expectedTypeShape);
        }

        private IFieldName EnumField(string declaringType, string shortName)
        {
            return Names.Field("[{0}] [{0}].{1}", declaringType, shortName);
        }

        private Results RunAnalysis()
        {
            IList<string> infos = new List<string>();
            var testLogger = new TestLogger(false);
            testLogger.InfoLogged += infos.Add;

            var results = new Results();
            Func<TypeShape, bool> cbTypeShape = tS =>
            {
                var typeShapeIndex = results.TypeShapeIndex;
                var assembly = tS.TypeHierarchy.Element.Assembly;
                if (typeShapeIndex.ContainsKey(assembly))
                {
                    typeShapeIndex[assembly].Add(tS);
                }
                else
                {
                    typeShapeIndex.Add(assembly, Sets.NewHashSet(tS));
                }
                return false;
            };
            DoTestSolution(
                (lifetime, solution) =>
                {
                    new TypeShapeSolutionAnalysis(solution, testLogger, cbTypeShape).AnalyzeAllProjects();
                });

            testLogger.InfoLogged -= infos.Add;
            return results;
        }


        private class Results
        {
            public Results()
            {
                TypeShapeIndex = new Dictionary<IAssemblyName, ISet<TypeShape>>();
            }

            public Dictionary<IAssemblyName, ISet<TypeShape>> TypeShapeIndex { get; private set; }

            public List<IAssemblyName> AssemblyNames
            {
                get { return TypeShapeIndex.Keys.AsList(); }
            }

            public IKaVEList<ITypeShape> AllTypeShapes
            {
                get
                {
                    var typeShapes = Lists.NewList<ITypeShape>();
                    foreach (var typeshapeSet in TypeShapeIndex.Values)
                    {
                        typeShapes.AddAll(typeshapeSet);
                    }
                    return typeShapes;
                }
            }
        }
    }
}