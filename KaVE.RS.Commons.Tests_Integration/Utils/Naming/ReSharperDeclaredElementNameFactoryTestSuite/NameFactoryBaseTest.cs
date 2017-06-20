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

using System.Linq;
using JetBrains.Util;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Impl.v0.CodeElements;
using KaVE.Commons.Model.Naming.Impl.v0.Types;
using KaVE.Commons.Model.SSTs.Declarations;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Collections;
using KaVE.JetBrains.Annotations;
using KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite;
using NUnit.Framework;

namespace KaVE.RS.Commons.Tests_Integration.Utils.Naming.ReSharperDeclaredElementNameFactoryTestSuite
{
    internal abstract class NameFactoryBaseTest : BaseSSTAnalysisTest
    {
        protected void AssertParameterTypes(params string[] paramTypes)
        {
            var m = AssertSingleMethod();
            var actualTypes = m.Name.Parameters.Select(p => p.ValueType);
            var expectedTypes = Lists.NewList(paramTypes).Select(TypeUtils.CreateTypeName);
            CollectionAssert.AreEqual(expectedTypes, actualTypes);
        }

        protected IParameterName AssertSingleParameter()
        {
            var m = AssertSingleMethod();
            Assert.AreEqual(1, m.Name.Parameters.Count);
            return m.Name.Parameters[0];
        }

        protected IMethodDeclaration AssertSingleMethod()
        {
            var sst = ResultSST;
            Assert.AreEqual(1, sst.Methods.Count);
            var fallback = new MethodDeclaration();
            var decl = sst.Methods.FirstOrDefault(fallback);
            Assert.AreNotSame(fallback, decl);
            return decl;
        }

        [StringFormatMethod("expectedMethodId")]
        protected void AssertSingleMethodName(string expectedMethodId, params object[] args)
        {
            var m = AssertSingleMethod();
            Assert.AreEqual(new MethodName(expectedMethodId.FormatEx(args)), m.Name);
        }
    }
}