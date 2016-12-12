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

        public Dictionary<IAssemblyName, ISet<ITypeShape>> TypeShapeIndex;

        public List<IAssemblyName> AssemblyNames
        {
            get { return TypeShapeIndex.Keys.AsList(); }
        }

        private void AddResult(ITypeShape ts)
        {
            try
            {
                var assembly = ts.TypeHierarchy.Element.Assembly;
                if (TypeShapeIndex.ContainsKey(assembly))
                {
                    TypeShapeIndex[assembly].Add(ts);
                }
                else
                {
                    TypeShapeIndex.Add(assembly, Sets.NewHashSet(ts));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("### error: " + ts);
            }
        }

        [Test]
        public void RunAllAnalyses()
        {
            RunAnalysis();

            DoesNotAnalyzeProjectDependencies();
            AnalyzesAllStandardReferences();
            AnalyzesNugetDependency();
            ContainsOneExampleTypeFromStandardDependency();
            ContainsOneExampleTypeFromNugetDependency();
            AnalyzesNestedTypes();
        }

        private void RunAnalysis()
        {
            TypeShapeIndex = new Dictionary<IAssemblyName, ISet<ITypeShape>>();

            DoTestSolution(
                (lifetime, solution) =>
                {
                    var logger = new TestLogger(false);
                    new TypeShapeSolutionAnalysis(solution, logger, AddResult).AnalyzeAllProjects();
                });
        }

        private void DoesNotAnalyzeProjectDependencies()
        {
            var assemblyNames = AssemblyNames;
            CollectionAssert.DoesNotContain(assemblyNames, Names.Assembly("Project1"));
        }

        private void AnalyzesAllStandardReferences()
        {
            var assemblyNames = AssemblyNames;
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
                    Names.Assembly("mscorlib, 4.0.0.0")
                },
                assemblyNames);
        }

        private void AnalyzesNugetDependency()
        {
            var assemblyNames = AssemblyNames;
            CollectionAssert.Contains(assemblyNames, Names.Assembly("Newtonsoft.Json, 6.0.0.0"));
        }

        private void ContainsOneExampleTypeFromStandardDependency()
        {
            var assemblyName = Names.Assembly("System, 4.0.0.0");
            CollectionAssert.Contains(AssemblyNames, assemblyName);

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

            var typeShapes = TypeShapeIndex[assemblyName];
            CollectionAssert.Contains(typeShapes, expectedTypeShape);
        }

        private void ContainsOneExampleTypeFromNugetDependency()
        {
            var assemblyName = Names.Assembly("Newtonsoft.Json, 6.0.0.0");
            CollectionAssert.Contains(AssemblyNames, assemblyName);

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
            var typeShapes = TypeShapeIndex[assemblyName];
            CollectionAssert.Contains(typeShapes, expectedTypeShape);
        }

        private void AnalyzesNestedTypes()
        {
            var assemblyName = Names.Assembly("Newtonsoft.Json, 6.0.0.0");
            CollectionAssert.Contains(AssemblyNames, assemblyName);

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
            var typeShapes = TypeShapeIndex[assemblyName];
            CollectionAssert.Contains(typeShapes, expectedTypeShape);
        }

        private static IFieldName EnumField(string declaringType, string shortName)
        {
            return Names.Field("[{0}] [{0}].{1}", declaringType, shortName);
        }
    }
}