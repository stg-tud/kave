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
 *    - Sebastian Proksch
 */

using System.Text;
using KaVE.Model.Names.CSharp;
using KaVE.Model.SSTs;
using KaVE.Model.SSTs.Declarations;
using KaVE.Model.SSTs.Visitor;
using NUnit.Framework;

namespace KaVE.Model.Tests.SSTs.Visitor
{
    internal class SSTToStringVisitorTest
    {
        [Test]
        public void Asd()
        {
            var eventDeclaration = new EventDeclaration
            {
                Name = EventName.Get("[MyEvent, IO, 1.2.3.4] [DeclaringType, GUI, 5.6.7.8].E")
            };
            var fieldDeclaration = new FieldDeclaration
            {
                Name = FieldName.Get("[MyField, mscore, 4.0.0.0] [DeclaringType, mscore, 4.0.0.0]._f")
            };

            var sst = new SST { EnclosingType = TypeName.Get("MyType, mscore, 4.0.0.0") };
            sst.Events.Add(eventDeclaration);
            sst.Fields.Add(fieldDeclaration);

            var context = new StringBuilder();

            var sut = new SSTToStringVisitor();
            sst.Accept(sut, context);

            var actual = context.ToString();
            const string expected = "class MyType {\n\tevent MyEvent E;\n\tMyField _f;\n}\n";
            Assert.AreEqual(expected, actual);
        }
    }
}