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

using NUnit.Framework;
using Test.Targets;

namespace KaVE.Commons.Tests.Model.Naming.CSharp
{
    internal class CSharpNameFromInfoFactoryTest
    {
        private static readonly string TestTargetAssemblyQualifiedName = typeof (SimpleType).Assembly.FullName;

        [Test]
        public void ShouldCreateSimpleTypeName()
        {
            var typeName = typeof (SimpleType).GetName();

            Assert.AreEqual("Test.Targets.SimpleType", typeName.FullName);
            Assert.AreEqual(TestTargetAssemblyQualifiedName, typeName.Assembly.Identifier);
        }

        [Test]
        public void ShouldCreateTypeNameWithGenericParameters()
        {
            var typeName = typeof (GenericType<SimpleType>).GetName();

            Assert.AreEqual(
                "Test.Targets.GenericType`1[[Test.Targets.SimpleType, " + TestTargetAssemblyQualifiedName + "]]",
                typeName.FullName);
            Assert.AreEqual(TestTargetAssemblyQualifiedName, typeName.Assembly.Identifier);
        }

        [Test]
        public void ShouldGenerateInnerType()
        {
            var typeName = typeof (GenericType<>.IInnerType).GetName();

            Assert.AreEqual("Test.Targets.GenericType`1+IInnerType", typeName.FullName);
            Assert.AreEqual("Test.Targets.GenericType`1", typeName.DeclaringType.FullName);
        }

        [Test]
        public void ShouldCreateMethodNameWithoutParameters()
        {
            var methodName = typeof (SimpleType).GetMethod("SimpleMethod").GetName();

            Assert.AreEqual("System.Int32", methodName.ReturnType.FullName);
            Assert.AreEqual("SimpleMethod", methodName.Name);
            Assert.AreEqual("Test.Targets.SimpleType", methodName.DeclaringType.FullName);
            Assert.IsFalse(methodName.HasParameters);
            Assert.IsFalse(methodName.IsStatic);
        }

        [Test]
        public void ShouldCreateStaticMethod()
        {
            var methodName = typeof (SimpleType).GetMethod("StaticMethod").GetName();

            Assert.IsTrue(methodName.IsStatic);
        }

        [Test]
        public void ShouldCreateMethodNameWithParameter()
        {
            var methodName = typeof (SimpleType).GetMethod("ParameterizedMethod").GetName();

            Assert.IsTrue(methodName.HasParameters);
            var firstParameterName = methodName.Parameters[0];
            Assert.AreEqual("System.Int32", firstParameterName.ValueType.FullName);
            Assert.AreEqual("firstParam", firstParameterName.Name);
            var secondParameterName = methodName.Parameters[1];
            Assert.AreEqual("System.Object", secondParameterName.ValueType.FullName);
            Assert.AreEqual("secondParam", secondParameterName.Name);
        }

        [Test]
        public void ShouldCreateVarArgsParameter()
        {
            var parameterName = typeof (SimpleType).GetMethod("ParamsMethod").GetParameters()[0].GetName();

            Assert.AreEqual("System.Int32[]", parameterName.ValueType.FullName);
            Assert.IsTrue(parameterName.IsParameterArray);
        }

        [Test]
        public void ShouldCreateRefParameter()
        {
            var parameterName = typeof (SimpleType).GetMethod("RefParamMethod").GetParameters()[0].GetName();

            Assert.IsTrue(parameterName.IsPassedByReference);
            Assert.IsFalse(parameterName.IsOutput);
        }

        [Test]
        public void ShouldCreateOutParameter()
        {
            var parameterName = typeof (SimpleType).GetMethod("OutParamMethod").GetParameters()[0].GetName();

            Assert.IsTrue(parameterName.IsOutput);
            Assert.IsTrue(parameterName.IsPassedByReference);
        }

        [Test]
        public void ShouldCreateOptionalParameter()
        {
            var parameterName = typeof (SimpleType).GetMethod("OptionalParamMethod").GetParameters()[0].GetName();

            Assert.IsTrue(parameterName.IsOptional);
        }

        [Test]
        public void ShouldCreateFieldName()
        {
            var fieldName = typeof (SimpleType).GetField("_simpleTypedField").GetName();

            Assert.AreEqual("_simpleTypedField", fieldName.Name);
            Assert.AreEqual("System.Int32", fieldName.ValueType.FullName);
            Assert.AreEqual("Test.Targets.SimpleType", fieldName.DeclaringType.FullName);
            Assert.IsFalse(fieldName.IsStatic);
        }

        [Test]
        public void ShouldCreateStaticFieldName()
        {
            var fieldName = typeof (SimpleType).GetField("_staticField").GetName();

            Assert.IsTrue(fieldName.IsStatic);
        }

        [Test]
        public void ShouldCreateGenericField()
        {
            var fieldName = typeof (GenericType<>).GetField("_genericTypedField").GetName();

            Assert.AreEqual("Test.Targets.GenericType`1", fieldName.DeclaringType.FullName);
            Assert.AreEqual("?", fieldName.ValueType.FullName);
        }

        [Test]
        public void ShouldCreateSimpleProperty()
        {
            var propertyName = typeof (SimpleType).GetProperty("SimpleProperty").GetName();

            Assert.AreEqual("SimpleProperty", propertyName.Name);
            Assert.AreEqual("Test.Targets.SimpleType", propertyName.DeclaringType.FullName);
            Assert.AreEqual("System.String", propertyName.ValueType.FullName);
            Assert.IsTrue(propertyName.HasGetter);
            Assert.IsTrue(propertyName.HasSetter);
            Assert.IsFalse(propertyName.IsStatic);
        }

        [Test]
        public void ShouldCreateGetOnlyProperty()
        {
            var propertyName = typeof (SimpleType).GetProperty("GetOnlyProperty").GetName();

            Assert.IsTrue(propertyName.HasGetter);
            Assert.IsFalse(propertyName.HasSetter);
        }

        [Test]
        public void ShouldCreateSetOnlyProperty()
        {
            var propertyName = typeof (SimpleType).GetProperty("SetOnlyProperty").GetName();

            Assert.IsTrue(propertyName.HasSetter);
            Assert.IsFalse(propertyName.HasGetter);
        }

        [Test]
        public void ShouldCreateGetOnlyPropertyIfSetIsPrivate()
        {
            var propertyName = typeof (SimpleType).GetProperty("PrivateSetProperty").GetName();

            Assert.IsTrue(propertyName.HasGetter);
            Assert.IsFalse(propertyName.HasSetter);
        }

        [Test]
        public void ShouldCreateStaticProperty()
        {
            var propertyName = typeof (SimpleType).GetProperty("StaticProperty").GetName();

            Assert.IsTrue(propertyName.IsStatic);
        }

        [Test]
        public void ShouldCreateSimpleEvent()
        {
            var eventName = typeof (SimpleType).GetEvent("SomeEvent").GetName();

            Assert.AreEqual("SomeEvent", eventName.Name);
            Assert.AreEqual("Test.Targets.SimpleType", eventName.DeclaringType.FullName);
            Assert.AreEqual("Test.Targets.SimpleType+SomeEventHandler", eventName.HandlerType.FullName);
            Assert.IsFalse(eventName.IsStatic);
        }

        [Test]
        public void ShouldCreateStaticEvent()
        {
            var eventName = typeof (SimpleType).GetEvent("StaticEvent").GetName();

            Assert.IsTrue(eventName.IsStatic);
        }

        [Test]
        public void ShouldCreateArrayTypeName()
        {
            var typeName = typeof (SimpleType[,][][,,]).GetName();

            Assert.AreEqual("Test.Targets.SimpleType[,,,,,]", typeName.FullName);
        }
    }

    // ReSharper disable UnusedMember.Global
    // ReSharper disable UnusedParameter.Global
    // ReSharper disable ConvertToConstant.Local
    // ReSharper disable UnusedField.Compiler
    // ReSharper disable InconsistentNaming
    // ReSharper disable UnusedAutoPropertyAccessor.Local
    // ReSharper disable EventNeverInvoked
    // ReSharper disable EventNeverSubscribedTo.Global

    // ReSharper restore EventNeverSubscribedTo.Global
    // ReSharper restore EventNeverInvoked
    // ReSharper restore UnusedAutoPropertyAccessor.Local
    // ReSharper restore InconsistentNaming
    // ReSharper restore UnusedField.Compiler
    // ReSharper restore ConvertToConstant.Local
    // ReSharper restore UnusedParameter.Global
    // ReSharper restore UnusedMember.Global
}