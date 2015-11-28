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
        public void ShouldRecreateMethodNameWithParameters()
        {
            var methodName = MethodName.Get("[Value, A, 1.0.0.0] [Decl, B, 2.0.0.0].A(out [A, A, 1.0.0.0] p1, [B, B, 1.0.0.0] p2)");
           
            AssertMethodNameRecreation(methodName);
        }

        [Test]
        public void ShouldRecreateMethodNameWithOptParameters()
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
        public void ShouldRecreateMethodNameWithTypeParameters()
        {
            var methodName = MethodName.Get("[T] [D, D, 4.5.6.7].M`2[[T],[O]]([O] p)");

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
            var fieldName = FieldName.Get("static [System.Int32, mscore, 4.0.0.0] [MyClass, MyAssembly, 1.2.3.4].Constant");

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