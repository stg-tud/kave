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
using KaVE.Commons.Model.SSTs.Blocks;
using KaVE.Commons.Model.SSTs.Declarations;
using KaVE.Commons.Model.SSTs.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Expressions.LoopHeader;
using KaVE.Commons.Model.SSTs.Impl.Visitor;
using KaVE.Commons.Model.SSTs.Statements;
using KaVE.Commons.Utils.Collections;

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

        private void AddMethodIf(IList<Event> events)
        {
            if (_currentName != null)
            {
                events.Add(Events.NewStopEvent());
                events.Add(Events.NewMethodEvent(_currentName));
                _currentName = null;
            }
        }

        public override void Visit(IMethodDeclaration method, IList<Event> events)
        {
            _currentName = method.Name;

            foreach (var stmt in method.Body)
            {
                stmt.Accept(this, events);
            }
        }

        public override void Visit(IForEachLoop stmt, IList<Event> events)
        {
            VisitBody(stmt.Body, events);
        }

        public override void Visit(IForLoop stmt, IList<Event> events)
        {
            VisitBody(stmt.Init, events);
            stmt.Condition.Accept(this, events);
            VisitBody(stmt.Step, events);
            VisitBody(stmt.Body, events);
        }

        public override void Visit(IIfElseBlock stmt, IList<Event> events)
        {
            stmt.Condition.Accept(this, events);
            VisitBody(stmt.Else, events);
            VisitBody(stmt.Then, events);
        }

        public override void Visit(IUsingBlock stmt, IList<Event> events)
        {
            VisitBody(stmt.Body, events);
        }

        public override void Visit(ITryBlock stmt, IList<Event> events)
        {
            VisitBody(stmt.Body, events);
            foreach (var catchBlock in stmt.CatchBlocks)
            {
                VisitBody(catchBlock.Body, events);
            }
            VisitBody(stmt.Finally, events);
        }

        private void VisitBody(IKaVEList<IStatement> body, IList<Event> events)
        {
            foreach (var stmt in body)
            {
                stmt.Accept(this, events);
            }
        }

        #region stmt

        public override void Visit(IAssignment stmt, IList<Event> events)
        {
            stmt.Expression.Accept(this, events);
        }

        public override void Visit(IExpressionStatement stmt, IList<Event> events)
        {
            stmt.Expression.Accept(this, events);
        }

        #endregion

        #region expr

        public override void Visit(IInvocationExpression inv, IList<Event> events)
        {
            AddMethodIf(events);
            events.Add(Events.NewInvocation(inv.MethodName));
        }

        public override void Visit(ILoopHeaderBlockExpression expr, IList<Event> events)
        {
            VisitBody(expr.Body, events);
        }

        #endregion
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