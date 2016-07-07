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

using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.Impl.v0.CodeElements;
using KaVE.Commons.Model.Naming.Impl.v0.Types;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Naming.CSharp.MemberNames
{
    internal class EventNameTest
    {
        [Test]
        public void ShouldImplementIsUnknown()
        {
            Assert.That(EventName.UnknownName.IsUnknown);
        }

        [Test]
        public void ShouldBeSimpleEvent()
        {
            var eventName = Names.Event("[ChangedEventHandler, IO, 1.2.3.4] [TextBox, GUI, 0.8.7.6].Changed");

            Assert.AreEqual("ChangedEventHandler", eventName.HandlerType.FullName);
            Assert.AreEqual("TextBox", eventName.DeclaringType.FullName);
            Assert.AreEqual("Changed", eventName.Name);
        }

        [Test]
        public void ShouldBeUnknownEvent()
        {
            Assert.AreSame(TypeName.UnknownName, EventName.UnknownName.HandlerType);
            Assert.AreSame(TypeName.UnknownName, EventName.UnknownName.DeclaringType);
            Assert.AreEqual("???", EventName.UnknownName.Name);
        }

        [Test]
        public void StringHelperWorks()
        {
            const string id = "[{0}] [T,P].E";
            const string t1 = "T1,P";
            var a = EventName.Get(string.Format(id, t1));
            var b = EventName.Get(id, t1);

            Assert.AreEqual(a, b);
            Assert.AreSame(a, b);
        }
    }
}