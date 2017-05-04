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

using System.Collections.Generic;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.Types.Organization;
using KaVE.FeedbackProcessor.StatisticsUltimate;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.StatisticsUltimate
{
    internal class ContextStatisticsLoggerTest
    {
        [Test]
        public void Integration()
        {
            var sut = new ContextStatisticsLogger();

            sut.StartUp(new ContextFilter(GeneratedCode.Include, Duplication.Exclude));

            sut.SearchingZips("C:\\a\\b\\");
            sut.FoundZips(123);

            sut.StartingStatCreation(1);
            sut.StartingStatCreation(2);
            sut.CreatingStats(1, "a/b.zip");
            sut.CreatingStats(1, "c.zip");
            sut.CreatingStats(1, "d.zip");
            sut.FinishedStatCreation(1);
            sut.FinishedStatCreation(2);


            var counts = new Dictionary<IAssemblyName, int>();
            counts[Names.Assembly("A,1.2.3.4")] = 1;
            counts[Names.Assembly("B,1.2.3.4")] = 4;
            counts[Names.Assembly("D,1.2.3.4")] = 3;
            counts[Names.Assembly("C,1.2.3.4")] = 2;

            sut.Results(
                new ContextStatistics
                {
                    NumRepositories = 1,
                    NumUsers = 2,
                    NumSolutions = 3,
                    EstimatedLinesOfCode = 31,
                    NumTypeDeclTopLevel = 4,
                    NumTypeDeclNested = 5,
                    UniqueTypeDecl =
                    {
                        Names.Type("T1,P"),
                        Names.Type("T2,P"),
                        Names.Type("T3,P"),
                        Names.Type("T4,P"),
                        Names.Type("T5,P")
                    },
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
                    UniqueMethodDeclsOverrideOrImplementAsm =
                    {
                        Names.Method("[p:bool] [T,P].M1()"),
                        Names.Method("[p:bool] [T,P].M2()"),
                        Names.Method("[p:bool] [T,P].M3()"),
                        Names.Method("[p:bool] [T,P].M4()"),
                        Names.Method("[p:bool] [T,P].M5()"),
                        Names.Method("[p:bool] [T,P].M6()")
                    },
                    UniqueAssemblies =
                    {
                        Names.Assembly("A,1.2.3.4")
                    },
                    NumAsmCalls = 14,
                    UniqueAsmMethods =
                    {
                        Names.Method("[p:bool] [T,P].M1()"),
                        Names.Method("[p:bool] [T,P].M2()")
                    },
                    NumAsmFieldRead = 15,
                    UniqueAsmFields =
                    {
                        Names.Field("[p:int] [T,P]._f1"),
                        Names.Field("[p:int] [T,P]._f2"),
                        Names.Field("[p:int] [T,P]._f3")
                    },
                    NumAsmPropertyRead = 16,
                    UniqueAsmProperties =
                    {
                        Names.Property("get set [p:double] [T,P].P1()"),
                        Names.Property("get set [p:double] [T,P].P2()"),
                        Names.Property("get set [p:double] [T,P].P3()"),
                        Names.Property("get set [p:double] [T,P].P4()")
                    }
                },
                counts);
        }
    }
}