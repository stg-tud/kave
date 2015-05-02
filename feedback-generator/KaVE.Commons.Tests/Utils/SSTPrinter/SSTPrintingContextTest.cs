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
 *    - Andreas Bauer
 */

using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Utils.SSTPrinter;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.SSTPrinter
{
    internal class SSTPrintingContextTest
    {
        private void AssertTypeFormat(string expected, string typeIdentifier)
        {
            var sut = new SSTPrintingContext();
            Assert.AreEqual(expected, sut.TypeName(TypeName.Get(typeIdentifier)).ToString());
        }

        [Test]
        public void TypeNameFormat()
        {
            AssertTypeFormat("T", "T,P");
        }

        [Test]
        public void TypeNameFormat_Generics()
        {
            AssertTypeFormat("EventHandler<EventArgsType>", "EventHandler`1[[T -> EventArgsType,P]],P");
        }

        [Test]
        public void TypeNameFormat_UnknownGenericType()
        {
            // these TypeNames are equivalent
            AssertTypeFormat("C<T>", "C`1[[T]],P");
            AssertTypeFormat("C<T>", "C`1[[T -> ?]],P");
            AssertTypeFormat("C<T>", "C`1[[T -> T]],P");
        }

        [Test]
        public void TypeNameFormat_MultipleGenerics()
        {
            AssertTypeFormat("A<B, C>", "A`2[[T1 -> B,P],[T2 -> C,P]],P");
        }

        [Test]
        public void TypeNameFormat_NestedGenerics()
        {
            AssertTypeFormat("A<B<C>>", "A`1[[T -> B`1[[T -> C,P]],P]],P");
        }
    }
}