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

using System.Text;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.Model.SSTs.Impl.Visitor;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.SSTs.Impl.Visitor
{
    internal class ToStringVisitorTest
    {
        [Test]
        public void Asd()
        {
            var eventDeclaration = new EventDeclaration
            {
                Name = Names.Event("[MyEvent, IO, 1.2.3.4] [DeclaringType, GUI, 5.6.7.8].E")
            };
            var fieldDeclaration = new FieldDeclaration
            {
                Name = Names.Field("[MyField, mscore, 4.0.0.0] [DeclaringType, mscore, 4.0.0.0]._f")
            };

            var sst = new SST {EnclosingType = Names.Type("MyType, mscore, 4.0.0.0")};
            sst.Events.Add(eventDeclaration);
            sst.Fields.Add(fieldDeclaration);

            var context = new StringBuilder();

            var sut = new ToStringVisitor();
            sst.Accept(sut, context);

            var actual = context.ToString();
            const string expected = "class MyType {\n\tevent MyEvent E;\n\tMyField _f;\n}\n";
            Assert.AreEqual(expected, actual);
        }
    }
}