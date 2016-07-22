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

using System.Collections.Generic;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Impl.v0.CodeElements;
using KaVE.Commons.Model.Naming.Impl.v0.Types;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Naming.Impl.v0.CodeElements
{
    internal class MethodNameTest : MemberTestBase
    {
        protected override IMemberName GetMemberNameForBaseTests(string basicMemberSignature)
        {
            return new MethodName(string.Format("{0}()", basicMemberSignature));
        }

        [Test]
        public void DefaultValues()
        {
            var sut = new MethodName();
            Assert.AreEqual(new TypeName(), sut.DeclaringType);
            Assert.AreEqual(new TypeName(), sut.ValueType);
            Assert.AreEqual(new TypeName(), sut.ReturnType);
            Assert.False(sut.IsStatic);
            Assert.AreEqual("[?] [?].???()", sut.Identifier);
            Assert.AreEqual("???", sut.Name);
            Assert.True(sut.IsUnknown);
            Assert.False(sut.IsHashed);
            Assert.False(sut.HasParameters);
            Assert.False(sut.HasTypeParameters);
            Assert.False(sut.IsConstructor);
            Assert.False(sut.IsExtensionMethod);
            Assert.False(sut.IsGenericEntity);
            Assert.AreEqual(Lists.NewList<IParameterName>(), sut.Parameters);
            Assert.AreEqual(Lists.NewList<ITypeParameterName>(), sut.TypeParameters);
        }

        [Test]
        public void ShouldImplementIsUnknown()
        {
            Assert.True(new MethodName().IsUnknown);
            Assert.True(new MethodName("[?] [?].???()").IsUnknown);
            Assert.False(new MethodName("[T1,P] [T2,P].f").IsUnknown);
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void ShouldAvoidNullParameters()
        {
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            new MethodName(null);
        }

        [TestCaseSource(typeof(TestUtils), "TypeSource")]
        public void ShouldParseSingleParameter(string typeId)
        {
            var paramId = string.Format("[{0}] p", typeId);
            var sut = new MethodName(string.Format("[{0}] [{0}].M({1})", typeId, paramId));

            Assert.True(sut.HasParameters);
            Assert.AreEqual(1, sut.Parameters.Count);
            Assert.AreEqual(paramId, sut.Parameters[0].Identifier);
        }

        [TestCaseSource(typeof(TestUtils), "TypeSource")]
        public void ShouldParseMultipleParameters(string typeId)
        {
            var param1Id = string.Format("[{0}] p", typeId);
            var param2Id = string.Format("[{0}] q", typeId);
            var sut = new MethodName(string.Format("[{0}] [{0}].M({1}, {2})", typeId, param1Id, param2Id));

            Assert.True(sut.HasParameters);
            Assert.AreEqual(2, sut.Parameters.Count);
            Assert.AreEqual(param1Id, sut.Parameters[0].Identifier);
            Assert.AreEqual(param2Id, sut.Parameters[1].Identifier);
        }


        [TestCaseSource(typeof(TestUtils), "TypeSource")]
        public void ShouldParseSingleUnboundTypeParameters(string typeId)
        {
            var sut = new MethodName(string.Format("[{0}] [{0}].M`1[[T]]([{0}] p)", typeId));

            Assert.True(sut.HasTypeParameters);
            var expecteds = Lists.NewList(new TypeParameterName("T"));
            Assert.AreEqual(expecteds, sut.TypeParameters);
        }

        [TestCaseSource(typeof(TestUtils), "TypeSource")]
        public void ShouldParseSingleBoundTypeParameters(string typeId)
        {
            var sut = new MethodName(string.Format("[{0}] [{0}].M`1[[T -> {0}]]([{0}] p)", typeId));

            Assert.True(sut.HasTypeParameters);
            var expecteds = Lists.NewList(new TypeParameterName("T -> " + typeId));
            Assert.AreEqual(expecteds, sut.TypeParameters);
        }

        [TestCaseSource(typeof(TestUtils), "TypeSource")]
        public void ShouldParseMultipleTypeParameters(string typeId)
        {
            var sut = new MethodName(string.Format("[{0}] [{0}].M`1[[T -> {0}],[U]]([{0}] p)", typeId));

            Assert.True(sut.HasTypeParameters);
            var expecteds = Lists.NewList(new TypeParameterName("T -> " + typeId), new TypeParameterName("U"));
            Assert.AreEqual(expecteds, sut.TypeParameters);
        }

        [Test]
        public void ShouldNotConfuseGenericParameterTypesWithTypeParameters1()
        {
            var methodName = new MethodName("[Rt,P] [DT,P].M([GT`1[[T]]] p)");

            Assert.IsFalse(methodName.HasTypeParameters);
            Assert.AreEqual(Lists.NewList<ITypeParameterName>(), methodName.TypeParameters);
        }

        [Test]
        public void ShouldNotConfuseGenericParameterTypesWithTypeParameters2()
        {
            var method = new MethodName("[RT,P] [DT,P].M`1[[T]]([G`1[[U]],P] p)");

            var expected = new List<ITypeName> {new TypeParameterName("T")};
            Assert.AreEqual(expected, method.TypeParameters);
        }

        [Test]
        public void ShouldExcludeTypeParametersFromName()
        {
            var methodName = new MethodName("[R] [D, D, 9.8.7.6].M`1[[T]]()");

            Assert.AreEqual("M", methodName.Name);
        }

        [Test]
        public void ShouldParseConstructors()
        {
            const string voidId = "[System.Void, mscorlib, 4.0.0.0]";
            Assert.IsFalse(new MethodName(voidId + " [D,P].M()").IsConstructor);
            Assert.IsTrue(new MethodName(voidId + " [D,P]..ctor()").IsConstructor);
            Assert.IsTrue(new MethodName(voidId + " [D,P]..cctor()").IsConstructor);
        }

        [ExpectedException(typeof(AssertException)), //
         TestCase("[T,P] [D,P]..ctor()"), TestCase("[T,P] [D,P]..cctor()")]
        public void ShouldRejectNonVoidConstructors(string ctorId)
        {
            // ReSharper disable once ObjectCreationAsStatement
            new MethodName(ctorId);
        }

        [Test]
        public void ShouldParseExtensionMethods()
        {
            Assert.True(new MethodName("static [T,P] [T,P].M(this [T,P] o)").IsExtensionMethod);
            // not static
            Assert.False(new MethodName("[T,P] [T,P].M(this [T,P] o)").IsExtensionMethod);
            // no parameters
            Assert.False(new MethodName("static [T,P] [T,P].M()").IsExtensionMethod);
            // no this modifier
            Assert.False(new MethodName("static [T,P] [T,P].M([T,P] o)").IsExtensionMethod);
        }

        /* tests for utilities */

        [Test]
        public void ParamsForGenerics()
        {
            Assert.False(new MethodName("[R,P] [D,P].M`1[[T->d:[?] [?].([?] p),P]]()").HasParameters);
        }

        [Test]
        public void ShouldHandleWhitespacesInParamList()
        {
            Assert.False(new MethodName("[R,P] [D,P].M( )").HasParameters);
            Assert.AreEqual(
                Lists.NewList(new ParameterName("[?] p")),
                new MethodName("[R,P] [D,P].M( [?] p )").Parameters);
            Assert.AreEqual(
                Lists.NewList(new ParameterName("[?] p"), new ParameterName("[?] q")),
                new MethodName("[R,P] [D,P].M( [?] p , [?] q )").Parameters);
        }

        [Test]
        public void ParameterParsingIsCached()
        {
            var sut = new MethodName();
            var a = sut.Parameters;
            var b = sut.Parameters;
            Assert.AreSame(a, b);
        }

        [Test]
        public void TypeParameterParsingIsCached()
        {
            var sut = new MethodName();
            var a = sut.TypeParameters;
            var b = sut.TypeParameters;
            Assert.AreSame(a, b);
        }
    }
}