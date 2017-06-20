/*
 * Copyright 2014 Technische Universität Darmstadt
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        #region setup

        protected override FileSystemPath ExistingSolutionFilePath
        {
            get
            {
                return BaseTestDataPath.Combine(
                    Path.Combine(
                        "..",
                        "..",
                        "..",
                        "KaVE.RS.SolutionAnalysis.Tests",
                        "test",
                        "data",
                        "TestSolution",
                        "TestSolution.sln"));
            }
        }

        private IList<ITypeShape> _typeShapes;

        public List<IAssemblyName> AssemblyNames
        {
            get { return _typeShapes.Select(ts => ts.TypeHierarchy.Element.Assembly).Distinct().AsList(); }
        }

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            _typeShapes = Lists.NewList<ITypeShape>();

            DoTestSolution(
                (lifetime, solution) =>
                {
                    var logger = new TestLogger(false);
                    new TypeShapeSolutionAnalysis(solution, logger, ts => _typeShapes.Add(ts)).AnalyzeAllProjects();
                });
        }

        #endregion

        [Test]
        public void IncludesAllDependencies()
        {
            CollectionAssert.IsSubsetOf(
                new[]
                {
                    Names.Assembly("System, 4.0.0.0"),
                    Names.Assembly("System.Core, 4.0.0.0"),
                    Names.Assembly("System.Xml.Linq, 4.0.0.0"),
                    Names.Assembly("System.Data.DataSetExtensions, 4.0.0.0"),
                    Names.Assembly("Microsoft.CSharp, 4.0.0.0"),
                    Names.Assembly("System.Data, 4.0.0.0"),
                    Names.Assembly("System.Xml, 4.0.0.0"),
                    Names.Assembly("mscorlib, 4.0.0.0"),
                    Names.Assembly("Newtonsoft.Json, 6.0.0.0")
                },
                AssemblyNames);
        }

        [Test]
        public void DoesNotIncludeLocalProjects()
        {
            CollectionAssert.DoesNotContain(AssemblyNames, Names.Assembly("Project1"));
        }

        [Test]
        public void TypesAreNotReportedMultipleTimes()
        {
            var a = _typeShapes.Count;
            var b = _typeShapes.Distinct().Count();
            Assert.AreEqual(a, b);
        }

        [Test]
        public void Example_RootType()
        {
            // root types (i.e., Object, ValueType, Enum) should not occur in hierarchies, but we want to include them as type shapes
            var expected = new TypeShape
            {
                TypeHierarchy = new TypeHierarchy("p:object"),
                MethodHierarchies =
                {
                    new MethodHierarchy
                    {
                        Element = Names.Method("[p:void] [p:object]..ctor()")
                    },
                    new MethodHierarchy
                    {
                        Element = Names.Method("[p:string] [p:object].ToString()")
                    },
                    new MethodHierarchy
                    {
                        Element = Names.Method("[p:bool] [p:object].Equals([p:object] obj)")
                    },
                    new MethodHierarchy
                    {
                        Element = Names.Method("static [p:bool] [p:object].Equals([p:object] objA, [p:object] objB)")
                    },
                    new MethodHierarchy
                    {
                        Element =
                            Names.Method("static [p:bool] [p:object].ReferenceEquals([p:object] objA, [p:object] objB)")
                    },
                    new MethodHierarchy
                    {
                        Element = Names.Method("[p:int] [p:object].GetHashCode()")
                    },
                    new MethodHierarchy
                    {
                        Element = Names.Method("[System.Type, mscorlib, 4.0.0.0] [p:object].GetType()")
                    },
                    new MethodHierarchy
                    {
                        Element = Names.Method("[p:void] [p:object].Finalize()")
                    },
                    new MethodHierarchy
                    {
                        Element = Names.Method("[p:object] [p:object].MemberwiseClone()")
                    }
                }
            };
            var filter = _typeShapes.Where(ts => ts.TypeHierarchy.Element.Identifier.Contains("p:"));
            CollectionAssert.Contains(_typeShapes, expected);
        }

        [Test]
        public void Example_FromStandardLibrary()
        {
            var expected = new TypeShape
            {
                TypeHierarchy = new TypeHierarchy
                {
                    Element = Names.Type("System.UriTypeConverter, System, 4.0.0.0"),
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

            CollectionAssert.Contains(_typeShapes, expected);
        }

        [Test]
        public void Example_FromNugetDependency()
        {
            var expected = new TypeShape
            {
                TypeHierarchy = new TypeHierarchy
                {
                    Element = Names.Type("Newtonsoft.Json.Converters.BinaryConverter, Newtonsoft.Json, 6.0.0.0"),
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
            CollectionAssert.Contains(_typeShapes, expected);
        }

        [Test]
        public void FindsNestedTypes()
        {
            const string nested = "e:Newtonsoft.Json.JsonReader+State, Newtonsoft.Json, 6.0.0.0";
            var expected = new TypeShape
            {
                TypeHierarchy = new TypeHierarchy(nested),
                Fields =
                {
                    Enum(nested, "Start"),
                    Enum(nested, "Complete"),
                    Enum(nested, "Property"),
                    Enum(nested, "ObjectStart"),
                    Enum(nested, "Object"),
                    Enum(nested, "ArrayStart"),
                    Enum(nested, "Array"),
                    Enum(nested, "Closed"),
                    Enum(nested, "PostValue"),
                    Enum(nested, "ConstructorStart"),
                    Enum(nested, "Constructor"),
                    Enum(nested, "Error"),
                    Enum(nested, "Finished")
                }
            };
            CollectionAssert.Contains(_typeShapes, expected);
        }

        private static IFieldName Enum(string declaringType, string shortName)
        {
            return Names.Field("[{0}] [{0}].{1}", declaringType, shortName);
        }
    }
}