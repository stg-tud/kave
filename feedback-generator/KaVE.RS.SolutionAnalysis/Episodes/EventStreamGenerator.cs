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
using KaVE.Commons.Model.Naming.Impl.v0;
using KaVE.Commons.Model.Naming.Impl.v0.CodeElements;
using KaVE.Commons.Model.Naming.Impl.v0.Types;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Declarations;
using KaVE.Commons.Model.SSTs.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Visitor;

namespace KaVE.RS.SolutionAnalysis.Episodes
{
    internal class EventStreamGenerator : AbstractNodeVisitor<IList<Event>>
    {
        private IMethodName _currentName;

        public override void Visit(ISST sst, IList<Event> events)
        {
            foreach (var method in sst.Methods)
            {
                Visit(method, events);
            }
        }

        public override void Visit(IMethodDeclaration method, IList<Event> events)
        {
            _currentName = method.Name;
            base.Visit(method, events);
        }

        public override void Visit(IInvocationExpression inv, IList<Event> events)
        {
            if (ShouldInclude(inv.MethodName))
            {
                AddMethodIf(events);
                events.Add(Events.NewInvocation(inv.MethodName));
            }
        }

        private static bool ShouldInclude(IMethodName name)
        {
            if (MethodName.UnknownName.Equals(name))
            {
                return false;
            }

            var mscorlib = AssemblyName.Get("mscorlib, 4.0.0.0");
            var actualLib = name.DeclaringType.Assembly;
            if (!mscorlib.Equals(actualLib))
            {
                return false;
            }

            return true;
        }

        public override void Visit(ILambdaExpression inv, IList<Event> events)
        {
            // stop here for now!
        }

        private void AddMethodIf(IList<Event> events)
        {
            if (_currentName != null)
            {
                //events.Add(Events.NewStopEvent());
                events.Add(Events.NewMethodEvent(_currentName));
                _currentName = null;
            }
        }
    }

    internal class Events
    {
        public static Event NewMethodEvent(IMethodName name)
        {
            return new Event
            {
                Kind = EventKind.MethodDeclaration,
                Method = name.RemoveGenerics()
            };
        }

        public static Event NewInvocation(IMethodName name)
        {
            return new Event
            {
                Kind = EventKind.Invocation,
                Method = name.RemoveGenerics()
            };
        }
    }
}