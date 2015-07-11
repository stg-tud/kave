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
using System.Linq.Expressions;
using KaVE.Commons.Model.SSTs.Impl.Visitor;
using KaVE.Commons.Model.SSTs.Visitor;
using Moq;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.SSTs.Impl
{
    public static class SSTTestHelper
    {
        public static VisitorAssertion Accept(this ISSTNode node, int context)
        {
            return new VisitorAssertion(node, context);
        }

        public class VisitorAssertion
        {
            private readonly ISSTNode _node;
            private readonly int _context;

            public VisitorAssertion(ISSTNode node, int context)
            {
                _node = node;
                _context = context;
            }

            public void Verify(Expression<Action<AbstractNodeVisitor<int>>> fun)
            {
                var visitor = Mock.Of<AbstractNodeVisitor<int>>();
                _node.Accept(visitor, _context);
                Mock.Get(visitor).Verify(fun);
            }

            public void VerifyWithReturn(Expression<Func<AbstractNodeVisitor<int, object>, object>> fun)
            {
                var expected = new object();

                var visitor = Mock.Of<AbstractNodeVisitor<int, object>>();
                Mock.Get(visitor).Setup(fun).Returns(expected);

                var actual = _node.Accept(visitor, _context);
                Assert.AreEqual(expected, actual);
                Mock.Get(visitor).Verify(fun);
            }
        }
    }
}