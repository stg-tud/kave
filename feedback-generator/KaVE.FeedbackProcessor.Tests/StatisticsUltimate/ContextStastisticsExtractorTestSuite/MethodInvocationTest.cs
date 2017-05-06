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

using System.Linq;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Utils;
using KaVE.FeedbackProcessor.StatisticsUltimate;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.StatisticsUltimate.ContextStastisticsExtractorTestSuite
{
    internal class MethodInvocationTest : ContextStatisticsExtractorTestBase
    {
        private IContextStatistics ExtractFromInvocations(params string[] ids)
        {
            var stmts = ids.Select(InvStmt).ToArray();
            // ReSharper disable once CoVariantArrayConversion
            return Extract(CreateContextWithSSTAndMethodBody(stmts));
        }

        [Test]
        public void UnknownCalls()
        {
            var actual =
                ExtractFromInvocations(
                    Names.UnknownMethod.Identifier);

            Assert.AreEqual(1, actual.NumUnknownInvocations);
            Assert.AreEqual(0, actual.NumValidInvocations);
            Assert.AreEqual(0, actual.NumAsmCalls);
            Assert.AreEqual(0, actual.NumAsmDelegateCalls);

            AssertUniqueAsmMethods(actual);
            AssertUniqueAssemblies(actual);
        }

        [Test]
        public void LocalCalls()
        {
            var actual = ExtractFromInvocations(
                "[p:void] [T,P].M()");

            Assert.AreEqual(0, actual.NumUnknownInvocations);
            Assert.AreEqual(1, actual.NumValidInvocations);
            Assert.AreEqual(0, actual.NumAsmCalls);
            Assert.AreEqual(0, actual.NumAsmDelegateCalls);

            AssertUniqueAsmMethods(actual);
            AssertUniqueAssemblies(actual);
        }

        [Test]
        public void LocalDelegateCalls()
        {
            var actual =
                ExtractFromInvocations(
                    "[p:void] [d:[p:void] [D,P].()].Invoke()");

            Assert.AreEqual(0, actual.NumUnknownInvocations);
            Assert.AreEqual(1, actual.NumValidInvocations);
            Assert.AreEqual(0, actual.NumAsmCalls);
            Assert.AreEqual(0, actual.NumAsmDelegateCalls);

            AssertUniqueAsmMethods(actual);
            AssertUniqueAssemblies(actual);
        }

        [Test]
        public void ApiCalls()
        {
            var actual = ExtractFromInvocations("[p:void] [T,A,1.2.3.4].M()");

            Assert.AreEqual(0, actual.NumUnknownInvocations);
            Assert.AreEqual(1, actual.NumValidInvocations);
            Assert.AreEqual(1, actual.NumAsmCalls);
            Assert.AreEqual(0, actual.NumAsmDelegateCalls);

            AssertUniqueAsmMethods(actual, "[p:void] [T,A,1.2.3.4].M()");
            AssertUniqueAssemblies(actual, "A,1.2.3.4");
        }

        [Test]
        public void ApiDelegateCalls()
        {
            const string asm = "mscorlib, 4.0.0.0";
            var id =
                "[p:void] [d:[p:void] [System.Action`1[[T -> p:int]], {0}].([T] obj)].Invoke([T] obj)".FormatEx(asm);
            var idNorm =
                "[p:void] [d:[p:void] [System.Action`1[[T]], {0}].([T] obj)].Invoke([T] obj)".FormatEx(asm);

            var actual = ExtractFromInvocations(id);

            Assert.AreEqual(0, actual.NumUnknownInvocations);
            Assert.AreEqual(1, actual.NumValidInvocations);
            Assert.AreEqual(0, actual.NumAsmCalls);
            Assert.AreEqual(1, actual.NumAsmDelegateCalls);

            AssertUniqueAsmMethods(actual, idNorm);
            AssertUniqueAssemblies(actual, asm);
        }
    }
}