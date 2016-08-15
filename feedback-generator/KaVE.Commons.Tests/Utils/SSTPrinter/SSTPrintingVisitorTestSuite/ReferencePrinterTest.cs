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

using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
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
                EventName = Names.Event("[EventType,P] [DeclaringType,P].E"),
                Reference = SSTUtil.VariableReference("o")
            };

            AssertPrint(sst, "o.E");
        }

        [Test]
        public void EventReference_static()
        {
            var sst = new EventReference
            {
                EventName = Names.Event("static [EventType,P] [DeclaringType,P].E")
            };

            AssertPrint(sst, "DeclaringType.E");
        }

        [Test]
        public void FieldReference()
        {
            var sst = new FieldReference
            {
                FieldName = Names.Field("[FieldType,P] [DeclaringType,P].F"),
                Reference = SSTUtil.VariableReference("o")
            };

            AssertPrint(sst, "o.F");
        }

        [Test]
        public void FieldReference_StaticField()
        {
            var sst = new FieldReference
            {
                FieldName =
                    Names.Field("static [System.String, mscorlib, 4.0.0.0] [System.String, mscorlib, 4.0.0.0].Empty")
            };

            AssertPrint(sst, "string.Empty");
        }

        [Test]
        public void MethodReference()
        {
            var sst = new MethodReference
            {
                MethodName = Names.Method("[ReturnType,P] [DeclaringType,P].M([ParameterType,P] p)"),
                Reference = SSTUtil.VariableReference("o")
            };

            AssertPrint(sst, "o.M");
        }

        [Test]
        public void MethodReference_static()
        {
            var sst = new MethodReference
            {
                MethodName = Names.Method("static [ReturnType,P] [DeclaringType,P].M([ParameterType,P] p)")
            };

            AssertPrint(sst, "DeclaringType.M");
        }

        [Test]
        public void PropertyReference()
        {
            var sst = new PropertyReference
            {
                PropertyName = Names.Property("get set [PropertyType,P] [DeclaringType,P].P()"),
                Reference = SSTUtil.VariableReference("o")
            };

            AssertPrint(sst, "o.P");
        }

        [Test]
        public void PropertyReference_static()
        {
            var sst = new PropertyReference
            {
                PropertyName = Names.Property("static get set [PropertyType,P] [DeclaringType,P].P()")
            };

            AssertPrint(sst, "DeclaringType.P");
        }

        [Test]
        public void IndexAccessReference()
        {
            var sst = new IndexAccessReference
            {
                Expression = new IndexAccessExpression
                {
                    Reference = new VariableReference {Identifier = "arr"},
                    Indices = {new ConstantValueExpression {Value = "1"}}
                }
            };

            AssertPrint(sst, "arr[1]");
        }

        [Test]
        public void UnknownReference()
        {
            var sst = new UnknownReference();
            AssertPrint(sst, "???");
        }
    }
}