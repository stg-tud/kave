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

using System;
using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.Names.CSharp.Parser;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Names.CSharp.Parser
{
    [TestFixture]
    public class CsNameUtilTest
    {

        [TestCase("n.T+T1,a", "n:n.T+T1,a"),
         TestCase("n.T+T1+T2,a", "n:n:n.T+T1+T2,a"),
         TestCase("T`1[[T1 -> i:T`1[[T -> T]], a, 4.0.0.0]], a, 4.0.0.0",
             "T'1[[T1 -> i:T'1[[T -> T]], a, 4.0.0.0]], a, 4.0.0.0"),
        TestCase("d:[e:n.T1+T2, a] [e:n.T1+T2, a].d([e:n.T, a] p)", "d:[n:n.e:T1+T2, a] [n:n.e:T1+T2, a].d([n.e:T, a] p)"),
        TestCase("i:T`1[[T -> i:n.T, a, 0.0.0.0]]+T2, a, 0.0.0.0", "n:i:T'1[[T -> n.i:T, a, 0.0.0.0]]+T2, a, 0.0.0.0")]
        public void HandleOldNamesSimpleNested(string input, string expected)
        {
            Assert.AreEqual(expected, CsNameUtil.HandleOldTypeNames(input));
        }

        [TestCase("[System.Void, mscorlib, 4.0.0.0] [?]..ctor([?] a)", "[?] [?]..ctor([?] a)"),
        TestCase("[?] [?].m(out [?] t)", "[?] [?].m(out [?] t)")]
        public void HandleOldMethodNames(string input, string expected)
        {
            Assert.AreEqual(expected, CsNameUtil.HandleOldMethodNames(input));
        }

        [TestCase("e:n.T, a, 0.0.0.0", "n.e:T, a, 0.0.0.0"),
        TestCase("e:n.T+T2, a, 0.0.0.0", "n.e:T+T2, a, 0.0.0.0")]
        public void HandleTypeIdentifier(string input, string expected)
        {
            Assert.AreEqual(expected, CsNameUtil.HandleTypeIdentifier(input));
        }

        [TestCase("e:n.T+T2, a, 0.0.0.0", "n:n.e:T+T2, a, 0.0.0.0"),
        TestCase("e:n.T+T2+T3, a, 0.0.0.0", "n:n:n.e:T+T2+T3, a, 0.0.0.0")]
        public void HandleNestedTypeIdentifier(string input, string expected)
        {
            Assert.AreEqual(expected, CsNameUtil.HandleNestedTypeNames(CsNameUtil.HandleTypeIdentifier(input)));
        }

        [TestCase("CSharp.AliasName:???", typeof(AliasName)),
        TestCase("CSharp.AssemblyName:???", typeof(AssemblyName)),
        TestCase("CSharp.EventName:???", typeof(EventName)),
        TestCase("CSharp.FieldName:???", typeof(FieldName)),
        TestCase("CSharp.LambdaName:???",typeof(LambdaName)),
        TestCase("CSharp.LocalVariableName:???", typeof(LocalVariableName)),
        TestCase("CSharp.MethodName:???", typeof(CsMethodName)),
        TestCase("CSharp.Name:???", typeof(Name)),
        TestCase("CSharp.NamespaceName:???", typeof(NamespaceName)),
        TestCase("CSharp.ParameterName:???", typeof(ParameterName)),
        TestCase("CSharp.PropertyName:???", typeof(PropertyName)),
        TestCase("CSharp.TypeName:???", typeof(CsTypeName))]
        public void ParseJsonOldNames(string input, Type expectedType)
        {
             Assert.AreEqual(CsNameUtil.ParseJson(input).GetType(), expectedType);
        }

        [TestCase("1a:???", typeof(AliasName)),
        TestCase("1b:???", typeof(AssemblyName)),
        TestCase("1c:???", typeof(EventName)),
        TestCase("1d:???", typeof(FieldName)),
        TestCase("1e:???", typeof(LambdaName)),
        TestCase("1f:???", typeof(LocalVariableName)),
        TestCase("1g:???", typeof(CsMethodName)),
        TestCase("1h:???", typeof(Name)),
        TestCase("1i:???", typeof(NamespaceName)),
        TestCase("1j:???", typeof(ParameterName)),
        TestCase("1k:???", typeof(PropertyName)),
        TestCase("1l:???", typeof(CsTypeName))]
        public void ParseJsonVersionOne(string input, Type expectedType)
        {
            Assert.AreEqual(CsNameUtil.ParseJson(input).GetType(), expectedType);
        }

        [TestCase("1a:???", "1a:???"),
        TestCase("1b:???", "1b:???"),
        TestCase("1c:???", "1c:???"),
        TestCase("1d:???", "1d:???"),
        TestCase("1e:???", "1e:???"),
        TestCase("1f:???", "1f:???"),
        TestCase("1g:???", "1g:?"),
        TestCase("1h:???", "1h:???"),
        TestCase("1i:???", "1i:???"),
        TestCase("1j:???", "1j:???"),
        TestCase("1k:???", "1k:???"),
        TestCase("1l:???", "1l:?")]
        public void NameToJson(string input, string expected)
        {
            Assert.AreEqual(CsNameUtil.ToJson(CsNameUtil.ParseJson(input)), expected);
        }

        [TestCase("T, a", "a"),
        TestCase("T, a.a.a", "a.a.a")]
        public void GetNameFromId(string input, string expected)
        {
            var name = CsNameUtil.ParseTypeName(input);
            Assert.AreEqual(name.Assembly.Name, expected);
        }
    }
}