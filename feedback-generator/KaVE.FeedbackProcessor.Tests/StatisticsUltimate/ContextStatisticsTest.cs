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

            Assert.AreEqual(0, sut.NumTopLevelType);
            Assert.AreEqual(0, sut.NumNestedType);

            Assert.AreEqual(0, sut.NumClasses);
            Assert.AreEqual(0, sut.NumInterfaces);
            Assert.AreEqual(0, sut.NumDelegates);
            Assert.AreEqual(0, sut.NumStructs);
            Assert.AreEqual(0, sut.NumEnums);
            Assert.AreEqual(0, sut.NumUnusualType);

            Assert.AreEqual(0, sut.NumTypeExtendsOrImplements);
            Assert.AreEqual(0, sut.NumMethodDecls);
            Assert.AreEqual(0, sut.NumMethodOverridesOrImplements);

            Assert.AreEqual(Sets.NewHashSet<IAssemblyName>(), sut.UniqueAssemblies);

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
                NumTopLevelType = 4,
                NumNestedType = 5,
                NumClasses = 6,
                NumInterfaces = 7,
                NumDelegates = 8,
                NumStructs = 9,
                NumEnums = 10,
                NumUnusualType = 111,
                NumTypeExtendsOrImplements = 11,
                NumMethodDecls = 12,
                NumMethodOverridesOrImplements = 13,
                UniqueAssemblies = {Names.Assembly("A,1.2.3.4")},
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

            Assert.AreEqual(4, sut.NumTopLevelType);
            Assert.AreEqual(5, sut.NumNestedType);

            Assert.AreEqual(6, sut.NumClasses);
            Assert.AreEqual(7, sut.NumInterfaces);
            Assert.AreEqual(8, sut.NumDelegates);
            Assert.AreEqual(9, sut.NumStructs);
            Assert.AreEqual(10, sut.NumEnums);
            Assert.AreEqual(111, sut.NumUnusualType);

            Assert.AreEqual(11, sut.NumTypeExtendsOrImplements);
            Assert.AreEqual(12, sut.NumMethodDecls);
            Assert.AreEqual(13, sut.NumMethodOverridesOrImplements);

            Assert.AreEqual(Sets.NewHashSet(Names.Assembly("A,1.2.3.4")), sut.UniqueAssemblies);

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
                NumTopLevelType = 4,
                NumNestedType = 5,
                NumClasses = 6,
                NumInterfaces = 7,
                NumDelegates = 8,
                NumStructs = 9,
                NumEnums = 10,
                NumUnusualType = 111,
                NumTypeExtendsOrImplements = 11,
                NumMethodDecls = 12,
                NumMethodOverridesOrImplements = 13,
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
                NumTopLevelType = 4,
                NumNestedType = 5,
                NumClasses = 6,
                NumInterfaces = 7,
                NumDelegates = 8,
                NumStructs = 9,
                NumEnums = 10,
                NumUnusualType = 111,
                NumTypeExtendsOrImplements = 11,
                NumMethodDecls = 12,
                NumMethodOverridesOrImplements = 13,
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
                NumTopLevelType = 4
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
                NumNestedType = 5
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
                NumTypeExtendsOrImplements = 11
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
                NumMethodDecls = 12
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
                NumMethodOverridesOrImplements = 13
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
                NumTopLevelType = 4,
                NumNestedType = 5,
                NumClasses = 6,
                NumInterfaces = 7,
                NumDelegates = 8,
                NumStructs = 9,
                NumEnums = 10,
                NumUnusualType = 111,
                NumTypeExtendsOrImplements = 11,
                NumMethodDecls = 12,
                NumMethodOverridesOrImplements = 13,
                UniqueAssemblies = {Names.Assembly("A,1.2.3.4")},
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
                    NumTopLevelType = 4 + 1,
                    NumNestedType = 5 + 1,
                    NumClasses = 6 + 1,
                    NumInterfaces = 7 + 1,
                    NumDelegates = 8 + 1,
                    NumStructs = 9 + 1,
                    NumEnums = 10 + 1,
                    NumUnusualType = 111 + 1,
                    NumTypeExtendsOrImplements = 11 + 1,
                    NumMethodDecls = 12 + 1,
                    NumMethodOverridesOrImplements = 13 + 1,
                    UniqueAssemblies = {Names.Assembly("B,1.2.3.4")},
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
                NumTopLevelType = 4 * 2 + 1,
                NumNestedType = 5 * 2 + 1,
                NumClasses = 6 * 2 + 1,
                NumInterfaces = 7 * 2 + 1,
                NumDelegates = 8 * 2 + 1,
                NumStructs = 9 * 2 + 1,
                NumEnums = 10 * 2 + 1,
                NumUnusualType = 111 * 2 + 1,
                NumTypeExtendsOrImplements = 11 * 2 + 1,
                NumMethodDecls = 12 * 2 + 1,
                NumMethodOverridesOrImplements = 13 * 2 + 1,
                UniqueAssemblies = {Names.Assembly("A,1.2.3.4"), Names.Assembly("B,1.2.3.4")},
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