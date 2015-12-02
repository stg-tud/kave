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

using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.Utils;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils
{
    public class NameCreatorTest
    {
        [Test]
        public void ShouldRecreateMethodNameWithoutParameters()
        {
            var methodName = MethodName.Get("[System.Void, mscore, 4.0.0.0] [T, P, 1.2.3.4].MethodName()");

            AssertMethodNameRecreation(methodName);
        }

        [Test]
        public void ShouldRecreateMethodNameWithParameter()
        {
            var methodName =
                MethodName.Get("[Value, A, 1.0.0.0] [Decl, B, 2.0.0.0].A([A, A, 1.0.0.0] p)");

            AssertMethodNameRecreation(methodName);
        }

        [Test]
        public void ShouldRecreateMethodNameWithMultipleParameters()
        {
            const string declaringTypeIdentifier = "[A, B, 9.9.9.9]";
            const string returnTypeIdentifier = "[R, C, 7.6.5.4]";
            const string param1Identifier = "[P, D, 3.4.3.2] n";
            const string param2Identifier = "[Q, E, 9.1.8.2] o";
            const string param3Identifier = "[R, F, 6.5.7.4] p";

            var methodName = MethodName.Get(
                "{0} {1}.TestMethod({2}, {3}, {4})",
                returnTypeIdentifier,
                declaringTypeIdentifier,
                param1Identifier,
                param2Identifier,
                param3Identifier);
            AssertMethodNameRecreation(methodName);
        }

        [Test]
        public void ShouldRecreateMethodNameWithOptParameter()
        {
            var methodName =
                MethodName.Get("[Value, A, 1.0.0.0] [Decl, B, 2.0.0.0].A(out [A, A, 1.0.0.0] p1)");

            AssertMethodNameRecreation(methodName);
        }

        [Test]
        public void ShouldRecreateMethodNameWithOutParameter()
        {
            var methodName =
                MethodName.Get("[Value, A, 1.0.0.0] [Decl, B, 2.0.0.0].A(opt [A, A, 1.0.0.0] p1)");

            AssertMethodNameRecreation(methodName);
        }

        [Test]
        public void ShouldRecreateMethodNameWithVariableParameters()
        {
            var methodName =
                MethodName.Get("[Value, A, 1.0.0.0] [Decl, B, 2.0.0.0].A(params [A, A, 1.0.0.0] p1)");

            AssertMethodNameRecreation(methodName);
        }

        [Test]
        public void ShouldRecreateMethodNameWithRefParameter()
        {
            var methodName =
                MethodName.Get("[Value, A, 1.0.0.0] [Decl, B, 2.0.0.0].A(ref [System.Int32, mscore, 4.0.0.0] p1)");

            AssertMethodNameRecreation(methodName);
        }

        [Test]
        public void ShouldRecreateMethodNameWithUnboundTypeParameters()
        {
            var methodName = MethodName.Get("[T] [D, D, 4.5.6.7].M`2[[T],[O]]([O] p)");

            AssertMethodNameRecreation(methodName);
        }

        [Test]
        public void ShouldRecreateMethodNameWithBoundTypeParameters()
        {
            var methodName = MethodName.Get("[A] [D, D, 1.2.3.4].M`1[[A -> System.Int32, mscorlib, 4.0.0.0]]()");

            AssertMethodNameRecreation(methodName);
        }

        [Test]
        public void ShouldRecreateMethodNameWithDelegateParameters()
        {
            var methodName = MethodName.Get("[R,B] [D,B].M([d:[R,A] [D,A].()] p)");

            AssertMethodNameRecreation(methodName);
        }

        [Test]
        public void ShouldRecreateStaticMethodName()
        {
            var methodName = MethodName.Get("static [I, A, 1.0.2.0] [K, K, 0.1.0.2].m()");

            AssertMethodNameRecreation(methodName);
        }

        [Test]
        public void ShouldRecreateConstructorName()
        {
            var methodName = MethodName.Get("[MyType, A, 0.0.0.1] [MyType, A, 0.0.0.1]..cctor()");

            AssertMethodNameRecreation(methodName);
        }

        [Test]
        public void ShouldRecreateFieldName()
        {
            var fieldName = FieldName.Get("[System.Int32, mscore, 4.0.0.0] [Collections.IList, mscore, 4.0.0.0]._count");

            var fieldDecl = new FieldDeclaration
            {
                Name = fieldName
            };

            Assert.AreEqual(fieldName, fieldDecl.CreateFieldName());
        }

        [Test]
        public void ShouldRecreateStaticFieldName()
        {
            var fieldName =
                FieldName.Get("static [System.Int32, mscore, 4.0.0.0] [MyClass, MyAssembly, 1.2.3.4].Constant");

            var fieldDecl = new FieldDeclaration
            {
                Name = fieldName
            };

            Assert.AreEqual(fieldName, fieldDecl.CreateFieldName());
        }

        [Test]
        public void ShouldRecreateFieldNameWithTypeParameters()
        {
            const string valueTypeIdentifier = "T`1[[A, B, 1.0.0.0]], A, 9.1.8.2";
            const string declaringTypeIdentifier = "U`2[[B, C, 6.7.5.8],[C, D, 8.3.7.4]], Z, 0.0.0.0";
            var fieldName = FieldName.Get("[{0}] [{1}].bar",valueTypeIdentifier,declaringTypeIdentifier);

            var fieldDecl = new FieldDeclaration
            {
                Name = fieldName
            };

            Assert.AreEqual(fieldName, fieldDecl.CreateFieldName());
        }

        private static void AssertMethodNameRecreation(IMethodName methodName)
        {
            var methodDecl = new MethodDeclaration
            {
                Name = methodName
            };

            Assert.AreEqual(methodName, methodDecl.CreateMethodName());
        }
    }
}