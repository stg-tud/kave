/*
 * Copyright 2017 Sebastian Proksch
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

using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Model.Naming.Types.Organization;
using KaVE.Commons.TestUtils;
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.StatisticsUltimate;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.StatisticsUltimate
{
    internal class ContextStatisticsTest
    {
        [Test]
        public void Default()
        {
            var sut = new ContextStatistics();

            Assert.AreEqual(0, sut.NumRepositories);
            Assert.AreEqual(0, sut.NumUsers);
            Assert.AreEqual(0, sut.NumSolutions);
            Assert.AreEqual(0, sut.EstimatedLinesOfCode);

            Assert.AreEqual(0, sut.NumTypeDeclTotal);
            Assert.AreEqual(0, sut.NumTypeDeclTopLevel);
            Assert.AreEqual(0, sut.NumTypeDeclNested);
            Assert.AreEqual(0, sut.NumPartial);
            Assert.AreEqual(Sets.NewHashSet<ITypeName>(), sut.UniqueTypeDecl);

            Assert.AreEqual(0, sut.NumClasses);
            Assert.AreEqual(0, sut.NumInterfaces);
            Assert.AreEqual(0, sut.NumDelegates);
            Assert.AreEqual(0, sut.NumStructs);
            Assert.AreEqual(0, sut.NumEnums);
            Assert.AreEqual(0, sut.NumUnusualType);

            Assert.AreEqual(0, sut.NumTypeDeclExtendsOrImplements);
            Assert.AreEqual(0, sut.NumMethodDeclsTotal);
            Assert.AreEqual(0, sut.NumMethodDeclsOverrideOrImplement);
            Assert.AreEqual(0, sut.NumMethodDeclsOverrideOrImplementAsm);
            Assert.AreEqual(Sets.NewHashSet<IMethodName>(), sut.UniqueMethodDeclsOverrideOrImplementAsm);

            Assert.AreEqual(0, sut.NumValidInvocations);
            Assert.AreEqual(0, sut.NumUnknownInvocations);

            Assert.AreEqual(Sets.NewHashSet<IAssemblyName>(), sut.UniqueAssemblies);

            Assert.AreEqual(0, sut.NumAsmDelegateCalls);
            Assert.AreEqual(0, sut.NumAsmCalls);
            Assert.AreEqual(Sets.NewHashSet<IMethodName>(), sut.UniqueAsmMethods);

            Assert.AreEqual(0, sut.NumAsmFieldRead);
            Assert.AreEqual(Sets.NewHashSet<IFieldName>(), sut.UniqueAsmFields);

            Assert.AreEqual(0, sut.NumAsmPropertyRead);
            Assert.AreEqual(Sets.NewHashSet<IPropertyName>(), sut.UniqueAsmProperties);

            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SetValues()
        {
            var sut = new ContextStatistics
            {
                NumRepositories = 1,
                NumUsers = 2,
                NumSolutions = 3,
                EstimatedLinesOfCode = 31,
                NumTypeDeclTopLevel = 4,
                NumTypeDeclNested = 5,
                UniqueTypeDecl = {Names.Type("T,P")},
                NumPartial = 20,
                NumClasses = 6,
                NumInterfaces = 7,
                NumDelegates = 8,
                NumStructs = 9,
                NumEnums = 10,
                NumUnusualType = 111,
                NumTypeDeclExtendsOrImplements = 11,
                NumMethodDeclsTotal = 12,
                NumMethodDeclsOverrideOrImplement = 13,
                NumMethodDeclsOverrideOrImplementAsm = 21,
                UniqueMethodDeclsOverrideOrImplementAsm = {Names.Method("[p:bool] [T,P].O1()")},
                NumValidInvocations = 1311,
                NumUnknownInvocations = 131,
                UniqueAssemblies = {Names.Assembly("A,1.2.3.4")},
                NumAsmDelegateCalls = 141,
                NumAsmCalls = 14,
                UniqueAsmMethods = {Names.Method("[p:bool] [T,P].M()")},
                NumAsmFieldRead = 15,
                UniqueAsmFields = {Names.Field("[p:int] [T,P]._f")},
                NumAsmPropertyRead = 16,
                UniqueAsmProperties = {Names.Property("get set [p:double] [T,P].P()")}
            };

            Assert.AreEqual(1, sut.NumRepositories);
            Assert.AreEqual(2, sut.NumUsers);
            Assert.AreEqual(3, sut.NumSolutions);
            Assert.AreEqual(31, sut.EstimatedLinesOfCode);

            Assert.AreEqual(9, sut.NumTypeDeclTotal);
            Assert.AreEqual(4, sut.NumTypeDeclTopLevel);
            Assert.AreEqual(5, sut.NumTypeDeclNested);
            Assert.AreEqual(20, sut.NumPartial);
            Assert.AreEqual(Sets.NewHashSet(Names.Type("T,P")), sut.UniqueTypeDecl);

            Assert.AreEqual(6, sut.NumClasses);
            Assert.AreEqual(7, sut.NumInterfaces);
            Assert.AreEqual(8, sut.NumDelegates);
            Assert.AreEqual(9, sut.NumStructs);
            Assert.AreEqual(10, sut.NumEnums);
            Assert.AreEqual(111, sut.NumUnusualType);

            Assert.AreEqual(11, sut.NumTypeDeclExtendsOrImplements);
            Assert.AreEqual(12, sut.NumMethodDeclsTotal);
            Assert.AreEqual(13, sut.NumMethodDeclsOverrideOrImplement);
            Assert.AreEqual(21, sut.NumMethodDeclsOverrideOrImplementAsm);
            Assert.AreEqual(
                Sets.NewHashSet(Names.Method("[p:bool] [T,P].O1()")),
                sut.UniqueMethodDeclsOverrideOrImplementAsm);

            Assert.AreEqual(1311, sut.NumValidInvocations);
            Assert.AreEqual(131, sut.NumUnknownInvocations);

            Assert.AreEqual(Sets.NewHashSet(Names.Assembly("A,1.2.3.4")), sut.UniqueAssemblies);

            Assert.AreEqual(141, sut.NumAsmDelegateCalls);
            Assert.AreEqual(14, sut.NumAsmCalls);
            Assert.AreEqual(Sets.NewHashSet(Names.Method("[p:bool] [T,P].M()")), sut.UniqueAsmMethods);

            Assert.AreEqual(15, sut.NumAsmFieldRead);
            Assert.AreEqual(Sets.NewHashSet(Names.Field("[p:int] [T,P]._f")), sut.UniqueAsmFields);

            Assert.AreEqual(16, sut.NumAsmPropertyRead);
            Assert.AreEqual(Sets.NewHashSet(Names.Property("get set [p:double] [T,P].P()")), sut.UniqueAsmProperties);

            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(new ContextStatistics());
        }

        [Test]
        public void Equality_Default()
        {
            var a = new ContextStatistics();
            var b = new ContextStatistics();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_SetValues()
        {
            var a = new ContextStatistics
            {
                NumRepositories = 1,
                NumUsers = 2,
                NumSolutions = 3,
                EstimatedLinesOfCode = 31,
                NumTypeDeclTopLevel = 4,
                NumTypeDeclNested = 5,
                UniqueTypeDecl = {Names.Type("T,P")},
                NumPartial = 20,
                NumClasses = 6,
                NumInterfaces = 7,
                NumDelegates = 8,
                NumStructs = 9,
                NumEnums = 10,
                NumUnusualType = 111,
                NumTypeDeclExtendsOrImplements = 11,
                NumMethodDeclsTotal = 12,
                NumMethodDeclsOverrideOrImplement = 13,
                NumMethodDeclsOverrideOrImplementAsm = 21,
                UniqueMethodDeclsOverrideOrImplementAsm = {Names.Method("[p:bool] [T,P].O1()")},
                NumValidInvocations = 1311,
                NumUnknownInvocations = 131,
                UniqueAssemblies = {Names.Assembly("A,1.2.3.4")},
                NumAsmCalls = 14,
                UniqueAsmMethods = {Names.Method("[p:bool] [T,P].M()")},
                NumAsmFieldRead = 15,
                UniqueAsmFields = {Names.Field("[p:int] [T,P]._f")},
                NumAsmPropertyRead = 16,
                UniqueAsmProperties = {Names.Property("get set [p:double] [T,P].P()")}
            };
            var b = new ContextStatistics
            {
                NumRepositories = 1,
                NumUsers = 2,
                NumSolutions = 3,
                EstimatedLinesOfCode = 31,
                NumTypeDeclTopLevel = 4,
                NumTypeDeclNested = 5,
                UniqueTypeDecl = {Names.Type("T,P")},
                NumPartial = 20,
                NumClasses = 6,
                NumInterfaces = 7,
                NumDelegates = 8,
                NumStructs = 9,
                NumEnums = 10,
                NumUnusualType = 111,
                NumTypeDeclExtendsOrImplements = 11,
                NumMethodDeclsTotal = 12,
                NumMethodDeclsOverrideOrImplement = 13,
                NumMethodDeclsOverrideOrImplementAsm = 21,
                UniqueMethodDeclsOverrideOrImplementAsm = {Names.Method("[p:bool] [T,P].O1()")},
                NumValidInvocations = 1311,
                NumUnknownInvocations = 131,
                UniqueAssemblies = {Names.Assembly("A,1.2.3.4")},
                NumAsmCalls = 14,
                UniqueAsmMethods = {Names.Method("[p:bool] [T,P].M()")},
                NumAsmFieldRead = 15,
                UniqueAsmFields = {Names.Field("[p:int] [T,P]._f")},
                NumAsmPropertyRead = 16,
                UniqueAsmProperties = {Names.Property("get set [p:double] [T,P].P()")}
            };
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentNumRepositories()
        {
            var a = new ContextStatistics
            {
                NumRepositories = 1
            };
            var b = new ContextStatistics();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentNumUsers()
        {
            var a = new ContextStatistics
            {
                NumUsers = 2
            };
            var b = new ContextStatistics();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentNumSolutions()
        {
            var a = new ContextStatistics
            {
                NumSolutions = 3
            };
            var b = new ContextStatistics();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentEstimatedLinesOfCode()
        {
            var a = new ContextStatistics
            {
                EstimatedLinesOfCode = 31
            };
            var b = new ContextStatistics();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentNumTopLevelType()
        {
            var a = new ContextStatistics
            {
                NumTypeDeclTopLevel = 4
            };
            var b = new ContextStatistics();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentNumNestedType()
        {
            var a = new ContextStatistics
            {
                NumTypeDeclNested = 5
            };
            var b = new ContextStatistics();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentUniqueTypeDecl()
        {
            var a = new ContextStatistics
            {
                UniqueTypeDecl = {Names.Type("T,P")}
            };
            var b = new ContextStatistics();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentNumPartial()
        {
            var a = new ContextStatistics
            {
                NumPartial = 20
            };
            var b = new ContextStatistics();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentNumClasses()
        {
            var a = new ContextStatistics
            {
                NumClasses = 6
            };
            var b = new ContextStatistics();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentNumInterfaces()
        {
            var a = new ContextStatistics
            {
                NumInterfaces = 7
            };
            var b = new ContextStatistics();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentNumDelegates()
        {
            var a = new ContextStatistics
            {
                NumDelegates = 8
            };
            var b = new ContextStatistics();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentNumStructs()
        {
            var a = new ContextStatistics
            {
                NumStructs = 9
            };
            var b = new ContextStatistics();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentNumEnums()
        {
            var a = new ContextStatistics
            {
                NumEnums = 10
            };
            var b = new ContextStatistics();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentNumUnusualType()
        {
            var a = new ContextStatistics
            {
                NumUnusualType = 111
            };
            var b = new ContextStatistics();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentNumClassExtendOrImplement()
        {
            var a = new ContextStatistics
            {
                NumTypeDeclExtendsOrImplements = 11
            };
            var b = new ContextStatistics();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentNumMethodDecls()
        {
            var a = new ContextStatistics
            {
                NumMethodDeclsTotal = 12
            };
            var b = new ContextStatistics();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentNumMethodOverridesOrImplements()
        {
            var a = new ContextStatistics
            {
                NumMethodDeclsOverrideOrImplement = 13
            };
            var b = new ContextStatistics();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentNumMethodDeclsOverrideOrImplementAsm()
        {
            var a = new ContextStatistics
            {
                NumMethodDeclsOverrideOrImplementAsm = 21
            };
            var b = new ContextStatistics();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentUniqueMethodDeclsOverrideOrImplementAsm()
        {
            var a = new ContextStatistics
            {
                UniqueMethodDeclsOverrideOrImplementAsm = {Names.Method("[p:bool] [T,P].O1()")}
            };
            var b = new ContextStatistics();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentNumTotalRegularInvocations()
        {
            var a = new ContextStatistics
            {
                NumValidInvocations = 1311
            };
            var b = new ContextStatistics();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentNumUnknownInvocations()
        {
            var a = new ContextStatistics
            {
                NumUnknownInvocations = 131
            };
            var b = new ContextStatistics();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentUniqueAssemblies()
        {
            var a = new ContextStatistics
            {
                UniqueAssemblies = {Names.Assembly("A,1.2.3.4")}
            };
            var b = new ContextStatistics();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentNumAsmDelegateCalls()
        {
            var a = new ContextStatistics
            {
                NumAsmDelegateCalls = 141
            };
            var b = new ContextStatistics();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentNumCalls()
        {
            var a = new ContextStatistics
            {
                NumAsmCalls = 14
            };
            var b = new ContextStatistics();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentUniqueMethods()
        {
            var a = new ContextStatistics
            {
                UniqueAsmMethods = {Names.Method("[p:bool] [T,P].M()")}
            };
            var b = new ContextStatistics();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentNumFieldRead()
        {
            var a = new ContextStatistics
            {
                NumAsmFieldRead = 15
            };
            var b = new ContextStatistics();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentUniqueFields()
        {
            var a = new ContextStatistics
            {
                UniqueAsmFields = {Names.Field("[p:int] [T,P]._f")}
            };
            var b = new ContextStatistics();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentNumPropertyRead()
        {
            var a = new ContextStatistics
            {
                NumAsmPropertyRead = 16
            };
            var b = new ContextStatistics();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentUniqueProperties()
        {
            var a = new ContextStatistics
            {
                UniqueAsmProperties = {Names.Property("get set [p:double] [T,P].P()")}
            };
            var b = new ContextStatistics();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Add()
        {
            var a = new ContextStatistics
            {
                NumRepositories = 1,
                NumUsers = 2,
                NumSolutions = 3,
                EstimatedLinesOfCode = 31,
                NumTypeDeclTopLevel = 4,
                NumTypeDeclNested = 5,
                UniqueTypeDecl = {Names.Type("T,P")},
                NumPartial = 20,
                NumClasses = 6,
                NumInterfaces = 7,
                NumDelegates = 8,
                NumStructs = 9,
                NumEnums = 10,
                NumUnusualType = 111,
                NumTypeDeclExtendsOrImplements = 11,
                NumMethodDeclsTotal = 12,
                NumMethodDeclsOverrideOrImplement = 13,
                NumMethodDeclsOverrideOrImplementAsm = 121,
                UniqueMethodDeclsOverrideOrImplementAsm = {Names.Method("[p:bool] [T,P].O1()")},
                NumValidInvocations = 1311,
                NumUnknownInvocations = 131,
                UniqueAssemblies = {Names.Assembly("A,1.2.3.4")},
                NumAsmDelegateCalls = 141,
                NumAsmCalls = 14,
                UniqueAsmMethods = {Names.Method("[p:bool] [T,P].MA()")},
                NumAsmFieldRead = 15,
                UniqueAsmFields = {Names.Field("[p:int] [T,P]._fA")},
                NumAsmPropertyRead = 16,
                UniqueAsmProperties = {Names.Property("get set [p:double] [T,P].PA()")}
            };
            a.Add(
                new ContextStatistics
                {
                    NumRepositories = 1 + 1,
                    NumUsers = 2 + 1,
                    NumSolutions = 3 + 1,
                    EstimatedLinesOfCode = 31 + 1,
                    NumTypeDeclTopLevel = 4 + 1,
                    NumTypeDeclNested = 5 + 1,
                    UniqueTypeDecl = {Names.Type("T2,P")},
                    NumPartial = 20 + 1,
                    NumClasses = 6 + 1,
                    NumInterfaces = 7 + 1,
                    NumDelegates = 8 + 1,
                    NumStructs = 9 + 1,
                    NumEnums = 10 + 1,
                    NumUnusualType = 111 + 1,
                    NumTypeDeclExtendsOrImplements = 11 + 1,
                    NumMethodDeclsTotal = 12 + 1,
                    NumMethodDeclsOverrideOrImplement = 13 + 1,
                    NumMethodDeclsOverrideOrImplementAsm = 122,
                    UniqueMethodDeclsOverrideOrImplementAsm = {Names.Method("[p:bool] [T,P].O2()")},
                    NumValidInvocations = 1311 + 1,
                    NumUnknownInvocations = 131 + 1,
                    UniqueAssemblies = {Names.Assembly("B,1.2.3.4")},
                    NumAsmDelegateCalls = 141 + 1,
                    NumAsmCalls = 14 + 1,
                    UniqueAsmMethods = {Names.Method("[p:bool] [T,P].MB()")},
                    NumAsmFieldRead = 15 + 1,
                    UniqueAsmFields = {Names.Field("[p:int] [T,P]._fB")},
                    NumAsmPropertyRead = 16 + 1,
                    UniqueAsmProperties = {Names.Property("get set [p:double] [T,P].PB()")}
                });

            var c = new ContextStatistics
            {
                NumRepositories = 1 * 2 + 1,
                NumUsers = 2 * 2 + 1,
                NumSolutions = 3 * 2 + 1,
                EstimatedLinesOfCode = 31 * 2 + 1,
                NumTypeDeclTopLevel = 4 * 2 + 1,
                NumTypeDeclNested = 5 * 2 + 1,
                UniqueTypeDecl = {Names.Type("T,P"), Names.Type("T2,P")},
                NumPartial = 2 * 20 + 1,
                NumClasses = 6 * 2 + 1,
                NumInterfaces = 7 * 2 + 1,
                NumDelegates = 8 * 2 + 1,
                NumStructs = 9 * 2 + 1,
                NumEnums = 10 * 2 + 1,
                NumUnusualType = 111 * 2 + 1,
                NumTypeDeclExtendsOrImplements = 11 * 2 + 1,
                NumMethodDeclsTotal = 12 * 2 + 1,
                NumMethodDeclsOverrideOrImplement = 13 * 2 + 1,
                NumMethodDeclsOverrideOrImplementAsm = 121 * 2 + 1,
                UniqueMethodDeclsOverrideOrImplementAsm =
                {
                    Names.Method("[p:bool] [T,P].O1()"),
                    Names.Method("[p:bool] [T,P].O2()")
                },
                NumValidInvocations = 2 * 1311 + 1,
                NumUnknownInvocations = 2 * 131 + 1,
                UniqueAssemblies = {Names.Assembly("A,1.2.3.4"), Names.Assembly("B,1.2.3.4")},
                NumAsmDelegateCalls = 2 * 141 + 1,
                NumAsmCalls = 14 * 2 + 1,
                UniqueAsmMethods = {Names.Method("[p:bool] [T,P].MA()"), Names.Method("[p:bool] [T,P].MB()")},
                NumAsmFieldRead = 15 * 2 + 1,
                UniqueAsmFields = {Names.Field("[p:int] [T,P]._fA"), Names.Field("[p:int] [T,P]._fB")},
                NumAsmPropertyRead = 16 * 2 + 1,
                UniqueAsmProperties =
                {
                    Names.Property("get set [p:double] [T,P].PA()"),
                    Names.Property("get set [p:double] [T,P].PB()")
                }
            };
            Assert.AreEqual(a, c);
        }
    }
}