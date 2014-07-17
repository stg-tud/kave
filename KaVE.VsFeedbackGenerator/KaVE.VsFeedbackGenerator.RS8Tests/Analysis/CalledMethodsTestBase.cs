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
 * 
 * Contributors:
 *    - Sven Amann
 *    - Sebastian Proksch
 */

using System.Linq;
using KaVE.Model.Names;
using KaVE.Model.Names.CSharp;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis
{
    internal class CalledMethodsTestBase : BaseTest
    {
        protected void AssertNoCallsDetected()
        {
            var entryPoint = FindSingleEntryPoint();
            CollectionAssert.IsEmpty(ResultContext.EntryPointToCalledMethods[entryPoint]);
        }

        protected void AssertCallDetected(string methodIdentifier)
        {
            var singleEntryPoint = FindSingleEntryPoint();
            AssertCallDetected(singleEntryPoint, methodIdentifier);
        }

        protected void AssertNumberOfCalls(string entryPointDescriptor, int num)
        {
            var entryPoint = FindEntryPoint(entryPointDescriptor);
            Assert.AreEqual(ResultContext.EntryPointToCalledMethods[entryPoint].Count, num);
        }

        protected void AssertCallDetected(string entryPointDescriptor, string calledMethodIndentifier)
        {
            var entryPoint = FindEntryPoint(entryPointDescriptor);
            AssertCallDetected(entryPoint, calledMethodIndentifier);
        }

        private void AssertCallDetected(IMethodName entryPoint, string calledMethodIdentifier)
        {
            var calledMethods = ResultContext.EntryPointToCalledMethods[entryPoint];
            var unexpectedMethod = MethodName.Get(calledMethodIdentifier);
            CollectionAssert.Contains(calledMethods, unexpectedMethod);
        }

        protected void AssertNoCallDetected(string methodIdentifier)
        {
            var singleEntryPoint = FindSingleEntryPoint();
            AssertNoCallDetected(singleEntryPoint, methodIdentifier);
        }

        private IMethodName FindSingleEntryPoint()
        {
            Assert.AreEqual(1, ResultContext.EntryPoints.Count);
            var singleEntryPoint = ResultContext.EntryPoints.First();
            return singleEntryPoint;
        }

        protected void AssertNoCallDetected(string entryPointDesc, string calledMethodIdentifier)
        {
            var entryPoint = FindEntryPoint(entryPointDesc);
            AssertNoCallDetected(entryPoint, calledMethodIdentifier);
        }

        private IMethodName FindEntryPoint(string entryPointDesc)
        {
            var candidates = ResultContext.EntryPoints.Where(ep => ep.Identifier.Contains(entryPointDesc)).ToList();
            Assert.AreEqual(1, candidates.Count, "ambiguous entry point descriptor");
            return candidates.First();
        }

        private void AssertNoCallDetected(IMethodName entryPoint, string calledMethodIdentifier)
        {
            var calledMethods = ResultContext.EntryPointToCalledMethods[entryPoint];
            var unexpectedMethod = MethodName.Get(calledMethodIdentifier);
            CollectionAssert.DoesNotContain(calledMethods, unexpectedMethod);
        }
    }
}