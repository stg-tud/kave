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
using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Declarations;
using KaVE.Commons.Model.SSTs.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Visitor;
using KaVE.Commons.Model.SSTs.Statements;

namespace KaVE.FeedbackProcessor.Episodes
{
    internal class EventStreamGenerator : AbstractNodeVisitor<IList<Event>>
    {
        public override void Visit(ISST sst, IList<Event> events)
        {
            foreach (var method in sst.Methods)
            {
                Visit(method, events);
            }
        }

        public override void Visit(IMethodDeclaration method, IList<Event> events)
        {
            events.Add(Events.NewStopEvent());
            events.Add(Events.NewMethodEvent(method.Name));

            foreach (var stmt in method.Body)
            {
                stmt.Accept(this, events);
            }
        }

        public override void Visit(IExpressionStatement stmt, IList<Event> events)
        {
            stmt.Expression.Accept(this, events);
        }

        public override void Visit(IInvocationExpression inv, IList<Event> events)
        {
            events.Add(Events.NewInvocation(inv.MethodName));
        }
    }

    internal class Events
    {
        public static Event NewStopEvent()
        {
            return new Event
            {
                Kind = EventKind.Stop,
                Method = MethodName.UnknownName
            };
        }

        public static Event NewMethodEvent(IMethodName name)
        {
            return new Event
            {
                Kind = EventKind.MethodStart,
                Method = name
            };
        }

        public static Event NewInvocation(IMethodName name)
        {
            return new Event
            {
                Kind = EventKind.Invocation,
                Method = name
            };
        }
    }
}