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

using KaVE.Commons.Model.SSTs.Expressions.Assignable;

namespace KaVE.Commons.Model.SSTs.Impl.Visitor
{
    public class HasCompletionExpressionVisitor :
        AbstractNodeVisitor<HasCompletionExpressionVisitor.CompletionExpressionCounter>
    {
        public static CompletionExpressionCounter On(ISST sst)
        {
            var counter = new CompletionExpressionCounter();
            sst.Accept(new HasCompletionExpressionVisitor(), counter);
            return counter;
        }

        public override void Visit(ICompletionExpression expr, CompletionExpressionCounter context)
        {
            context.Add();
        }

        public class CompletionExpressionCounter
        {
            public int Count { get; private set; }

            public bool HasCompletionExpression
            {
                get { return Count != 0; }
            }

            public void Add()
            {
                Count++;
            }
        }
    }
}