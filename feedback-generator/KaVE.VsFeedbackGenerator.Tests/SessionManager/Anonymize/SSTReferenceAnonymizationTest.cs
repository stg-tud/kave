﻿/*
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

using KaVE.Model.SSTs.Declarations;
using KaVE.Model.SSTs.Impl.Declarations;
using KaVE.Model.SSTs.Impl.References;
using KaVE.Model.SSTs.References;
using KaVE.Model.SSTs.Visitor;
using KaVE.VsFeedbackGenerator.SessionManager.Anonymize;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.SessionManager.Anonymize
{
    public class SSTReferenceAnonymizationTest : SSTAnonymizationBaseTest
    {
        private SSTReferenceAnonymization _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new SSTReferenceAnonymization();
        }

        private void AssertAnonymization(ISSTNode expr, ISSTNode expected)
        {
            var actual = expr.Accept(_sut, 0);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void EventReference()
        {
            AssertAnonymization(
                new EventReference
                {
                    Reference = AnyVarReference,
                    EventName = Event("a")
                },
                new EventReference
                {
                    Reference = AnyVarReferenceAnonymized,
                    EventName = EventAnonymized("a")
                });
        }

        [Test]
        public void EventReference_null()
        {
            _sut.Visit(new EventReference(), 0);
        }

        [Test]
        public void FieldReference()
        {
            AssertAnonymization(
                new FieldReference
                {
                    Reference = AnyVarReference,
                    FieldName = Field("a")
                },
                new FieldReference
                {
                    Reference = AnyVarReferenceAnonymized,
                    FieldName = FieldAnonymized("a")
                });
        }

        [Test]
        public void FieldReference_null()
        {
            _sut.Visit(new FieldReference(), 0);
        }

        [Test]
        public void MethodReference()
        {
            AssertAnonymization(
                new MethodReference
                {
                    Reference = AnyVarReference,
                    MethodName = Method("a")
                },
                new MethodReference
                {
                    Reference = AnyVarReferenceAnonymized,
                    MethodName = MethodAnonymized("a")
                });
        }

        [Test]
        public void MethodReference_null()
        {
            _sut.Visit(new MethodReference(), 0);
        }

        [Test]
        public void PropertyReference()
        {
            AssertAnonymization(
                new PropertyReference
                {
                    Reference = AnyVarReference,
                    PropertyName = Property("a")
                },
                new PropertyReference
                {
                    Reference = AnyVarReferenceAnonymized,
                    PropertyName = PropertyAnonymized("a")
                });
        }

        [Test]
        public void PropertyReference_null()
        {
            _sut.Visit(new PropertyReference(), 0);
        }

        [Test]
        public void VariableReference()
        {
            AssertAnonymization(
                new VariableReference
                {
                    Identifier = "a"
                },
                new VariableReference
                {
                    Identifier = "a".ToHash()
                });
        }

        [Test]
        public void VariableReference_null()
        {
            _sut.Visit(new VariableReference(), 0);
        }

        [Test]
        public void Anonymization_VariableDeclaration()
        {
            var actual = _sut.Anonymize(
                new VariableDeclaration
                {
                    Reference = AnyVarReference
                });
            var expected =
                new VariableDeclaration
                {
                    Reference = AnyVarReferenceAnonymized
                };
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Anonymization_VariableDeclaration_NullSafe()
        {
            var actual = _sut.Anonymize(new VariableDeclaration());
            var expected = new VariableDeclaration();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Anonymization_VariableDeclaration_null()
        {
            Assert.Null(_sut.Anonymize((IVariableDeclaration) null));
        }

        [Test]
        public void Anonymization_IAssignableReference()
        {
            var actual = _sut.Anonymize(
                (IAssignableReference)
                    new VariableReference
                    {
                        Identifier = "a"
                    });
            var expected =
                new VariableReference
                {
                    Identifier = "a".ToHash()
                };
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Anonymization_IAssignableReference_NullSafe()
        {
            var actual = _sut.Anonymize((IAssignableReference) new VariableReference());
            var expected = new VariableReference();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Anonymization_IAssignableReference_null()
        {
            Assert.Null(_sut.Anonymize((IAssignableReference) null));
        }
    }
}