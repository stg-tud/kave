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
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.VS.FeedbackGenerator.SessionManager.Anonymize;
using KaVE.VS.FeedbackGenerator.SessionManager.Anonymize.CompletionEvents;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.SessionManager.Anonymize.CompletionEvents
{
    public class SSTAnonymizationTest : SSTAnonymizationBaseTest
    {
        private SSTAnonymization _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new SSTAnonymization(StatementAnonymizationMock);
        }

        [Test]
        public void AnonymizeNullValues()
        {
            _sut.Anonymize(new SST());
        }

        [Test]
        public void EnclosingTypeIsAnonymized()
        {
            var typeName = TypeName.Get("My.Type, MyProject");

            var actual = _sut.Anonymize(new SST {EnclosingType = typeName});
            var expected = new SST {EnclosingType = typeName.ToAnonymousName()};
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DelegateDeclarationsAreAnonymized()
        {
            var typeName = DelegateTypeName.Get("d:[R,P] [My.Type, MyProject].()");

            var actual = _sut.Anonymize(
                new SST
                {
                    Delegates = {new DelegateDeclaration {Name = typeName}}
                });

            var expected =
                new SST
                {
                    Delegates = {new DelegateDeclaration {Name = typeName.ToAnonymousName()}}
                };

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void EventDeclarationsAreAnonymized()
        {
            var eventName = EventName.Get("[ChangeEventHandler, IO, 1.2.3.4] [TextBox, GUI, 5.6.7.8].Changed");

            var actual = _sut.Anonymize(
                new SST
                {
                    Events = {new EventDeclaration {Name = eventName}}
                });

            var expected =
                new SST
                {
                    Events = {new EventDeclaration {Name = eventName.ToAnonymousName()}}
                };

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FieldDeclarationsAreAnonymized()
        {
            var fieldName = FieldName.Get("static [T1,P1] [T2,P2].F");

            var actual = _sut.Anonymize(
                new SST
                {
                    Fields = {new FieldDeclaration {Name = fieldName}}
                });


            var expected =
                new SST
                {
                    Fields = {new FieldDeclaration {Name = fieldName.ToAnonymousName()}}
                };

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PropertyDeclarationsAreAnonymized_Name()
        {
            var propertyName = PropertyName.Get("set [T1,P1] [T2,P2].P");

            var actual = _sut.Anonymize(
                new SST
                {
                    Properties = {new PropertyDeclaration {Name = propertyName}}
                });


            var expected =
                new SST
                {
                    Properties = {new PropertyDeclaration {Name = propertyName.ToAnonymousName()}}
                };

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PropertyDeclarationsAreAnonymized_Get()
        {
            var actual = _sut.Anonymize(
                new SST
                {
                    Properties = {new PropertyDeclaration {Get = {AnyStatement}}}
                });

            var expected =
                new SST
                {
                    Properties = {new PropertyDeclaration {Get = {AnyStatementAnonymized}}}
                };

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PropertyDeclarationsAreAnonymized_Set()
        {
            var actual = _sut.Anonymize(
                new SST
                {
                    Properties = {new PropertyDeclaration {Set = {AnyStatement}}}
                });

            var expected =
                new SST
                {
                    Properties = {new PropertyDeclaration {Set = {AnyStatementAnonymized}}}
                };

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MethodDeclarationsAreAnonymized_Name()
        {
            var methodName = MethodName.Get("[T1, P1] [T2, P2].M()");

            var actual = _sut.Anonymize(
                new SST
                {
                    Methods = {new MethodDeclaration {Name = methodName}}
                });

            var expected =
                new SST
                {
                    Methods = {new MethodDeclaration {Name = methodName.ToAnonymousName()}}
                };

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MethodDeclarationsAreAnonymized_EntryPointFlagIsNotChanged()
        {
            var actual = _sut.Anonymize(
                new SST
                {
                    Methods =
                    {
                        new MethodDeclaration {IsEntryPoint = false},
                        new MethodDeclaration {IsEntryPoint = true}
                    }
                });

            var expected =
                new SST
                {
                    Methods =
                    {
                        new MethodDeclaration {IsEntryPoint = false},
                        new MethodDeclaration {IsEntryPoint = true}
                    }
                };

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MethodDeclarationsAreAnonymized_Body()
        {
            var actual = _sut.Anonymize(
                new SST
                {
                    Methods = {new MethodDeclaration {Body = {AnyStatement}}}
                });

            var expected =
                new SST
                {
                    Methods = {new MethodDeclaration {Body = {AnyStatementAnonymized}}}
                };

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DefaultName_Delegates()
        {
            var actual = _sut.Anonymize(new DelegateDeclaration());
            var expected = new DelegateDeclaration();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DefaultName_Events()
        {
            var actual = _sut.Anonymize(new EventDeclaration());
            var expected = new EventDeclaration();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DefaultName_Fields()
        {
            var actual = _sut.Anonymize(new FieldDeclaration());
            var expected = new FieldDeclaration();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DefaultName_Methods()
        {
            var actual = _sut.Anonymize(new MethodDeclaration());
            var expected = new MethodDeclaration();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DefaultName_Properties()
        {
            var actual = _sut.Anonymize(new PropertyDeclaration());
            var expected = new PropertyDeclaration();
            Assert.AreEqual(expected, actual);
        }
    }
}