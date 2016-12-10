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
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.TypeShapes;

namespace KaVE.Commons.Tests.Model.TypeShapes
{
    internal class EventHierarchyTest : MemberHierarchyTestBase<IEventName>
    {
        protected override IMemberHierarchy<IEventName> CreateSut()
        {
            return new EventHierarchy();
        }

        protected override IMemberHierarchy<IEventName> CreateSut(IEventName n)
        {
            return new EventHierarchy(n);
        }

        protected override IEventName Get(int num = 0)
        {
            return num == 0
                ? Names.UnknownEvent
                : Names.Event(string.Format("[T1,P1] [T2,P2].E{0}", num));
        }
    }
}