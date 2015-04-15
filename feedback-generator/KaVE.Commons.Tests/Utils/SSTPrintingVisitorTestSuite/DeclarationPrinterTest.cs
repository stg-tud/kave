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
 *    - Andreas Bauer
 */

using System;
using System.Text;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.Model.SSTs.Visitor;
using KaVE.Commons.Utils;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.SSTPrintingVisitorTestSuite
{
    internal class DeclarationPrinterTest : SSTPrintingVisitorTestBase
    {
        [Test]
        public void EmptyClassDeclaration()
        {
            var sst = new SST {EnclosingType = TypeName.Get("TestClass, TestProject")};
            
            AssertPrint(sst,
                "class TestClass",
                "{",
                "}");
        }

        [Test]
        public void DelegateDeclaration()
        {
            // TODO discuss with Sven: TypeName or MethodName??
            var sst = new DelegateDeclaration {Name = TypeName.Get("d:T,P")};
            AssertPrint(sst, "delegate T();");
        }

        [Test]
        public void EventDeclaration()
        {
            var sst = new EventDeclaration
            {
                Name = EventName.Get("[EventType,P] [DeclaringType,P].E")
            };

            AssertPrint(sst, "event EventType E;");
        }

        [Test]
        public void FieldDeclaration()
        {
            var sst = new FieldDeclaration
            {
                Name = FieldName.Get("[FieldType,P] [DeclaringType,P].F")
            };

            AssertPrint(sst, "FieldType F;");
        }

        [Test]
        public void PropertyDeclaration_GetterOnly()
        {
            var sst = new PropertyDeclaration
            {
                Name = PropertyName.Get("get [PropertyType,P] [DeclaringType,P].P")
            };

            AssertPrint(sst, "PropertyType P { get; };");
        }

        [Test]
        public void PropertyDeclaration_SetterOnly()
        {
            var sst = new PropertyDeclaration
            {
                Name = PropertyName.Get("set [PropertyType,P] [DeclaringType,P].P")
            };

            AssertPrint(sst, "PropertyType P { set; };");
        }

        [Test]
        public void PropertyDeclaration()
        {
            var sst = new PropertyDeclaration
            {
                Name = PropertyName.Get("get set [PropertyType,P] [DeclaringType,P].P"),
            };

            AssertPrint(sst, "PropertyType P { get; set; };");
        }

        [Test]
        public void PropertyDeclaration_WithBodies()
        {
            var sst = new PropertyDeclaration
            {
                Name = PropertyName.Get("get set [PropertyType,P] [DeclaringType,P].P"),
                Get =
                {
                    new ContinueStatement(),
                    new BreakStatement()
                },
                Set =
                {
                    new BreakStatement(),
                    new ContinueStatement(),
                }
            };

            AssertPrint(sst,
                "PropertyType P",
                "{",
                "    get",
                "    {",
                "        continue;",
                "        break;",
                "    }",
                "    set",
                "    {",
                "        break;",
                "        continue;",
                "    }",
                "};");
        }

        [Test]
        public void PropertyDeclaration_WithOnlyGetterBody()
        {
            var sst = new PropertyDeclaration
            {
                Name = PropertyName.Get("get set [PropertyType,P] [DeclaringType,P].P"),
                Get =
                {
                    new BreakStatement()
                }
            };

            AssertPrint(sst,
                "PropertyType P",
                "{",
                "    get",
                "    {", 
                "        break;",
                "    }",
                "    set;",
                "};");
        }

        [Test]
        public void PropertyDeclaration_WithOnlySetterBody()
        {
            var sst = new PropertyDeclaration
            {
                Name = PropertyName.Get("get set [PropertyType,P] [DeclaringType,P].P"),
                Set =
                {
                    new BreakStatement()
                }
            };
         
            AssertPrint(sst,
                "PropertyType P",
                 "{",
                 "    get;",
                 "    set",
                 "    {",
                 "        break;",
                 "    }",
                 "};");
        }

        [Test]
        public void MethodDeclaration_EmptyMethod()
        {
            var sst = new MethodDeclaration
            {
                Name = MethodName.Get("[ReturnType,P] [DeclaringType,P].M([ParameterType,P] p)")
                
            };

            AssertPrint(sst,
                "ReturnType M(ParameterType p) { }");
        }

        [Test]
        public void MethodDeclaration_WithBody()
        {
            var sst = new MethodDeclaration
            {
                Name = MethodName.Get("[ReturnType,P] [DeclaringType,P].M([ParameterType,P] p)"),
                Body =
                {
                    new ContinueStatement(),
                    new BreakStatement()
                }
            };

            AssertPrint(sst,
                "ReturnType M(ParameterType p)",
                "{",
                "    continue;",
                "    break;",
                "}");
        }
    }
}