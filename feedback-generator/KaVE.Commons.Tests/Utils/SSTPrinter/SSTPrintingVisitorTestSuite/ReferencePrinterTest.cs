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

using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.References;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.SSTPrinter.SSTPrintingVisitorTestSuite
{
    internal class ReferencePrinterTest : SSTPrintingVisitorTestBase
    {
        [Test]
        public void VariableReference()
        {
            var sst = SSTUtil.VariableReference("variable");
            AssertPrint(sst, "variable");
        }

        [Test]
        public void EventReference()
        {
            var sst = new EventReference
            {
                EventName = EventName.Get("[EventType,P] [DeclaringType,P].E"),
                Reference = SSTUtil.VariableReference("o")
            };

            AssertPrint(sst, "o.E");
        }

        [Test]
        public void FieldReference()
        {
            var sst = new FieldReference
            {
                FieldName = FieldName.Get("[FieldType,P] [DeclaringType,P].F"),
                Reference = SSTUtil.VariableReference("o")
            };

            AssertPrint(sst, "o.F");
        }

        [Test]
        public void MethodReference()
        {
            var sst = new MethodReference
            {
                MethodName = MethodName.Get("[ReturnType,P] [DeclaringType,P].M([ParameterType,P] p)"),
                Reference = SSTUtil.VariableReference("o")
            };

            AssertPrint(sst, "o.M");
        }

        [Test]
        public void PropertyReference()
        {
            var sst = new PropertyReference
            {
                PropertyName = PropertyName.Get("get set [PropertyType,P] [DeclaringType,P].P"),
                Reference = SSTUtil.VariableReference("o")
            };

            AssertPrint(sst, "o.P");
        }

        [Test]
        public void UnknownReference()
        {
            var sst = new UnknownReference();
            AssertPrint(sst, "???");
        }
    }
}