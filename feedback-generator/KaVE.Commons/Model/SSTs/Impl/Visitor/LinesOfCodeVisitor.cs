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
using KaVE.Commons.Model.SSTs.Blocks;
using KaVE.Commons.Model.SSTs.Declarations;
using KaVE.Commons.Model.SSTs.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Expressions.LoopHeader;
using KaVE.Commons.Model.SSTs.Expressions.Simple;
using KaVE.Commons.Model.SSTs.References;
using KaVE.Commons.Model.SSTs.Statements;
using KaVE.Commons.Model.SSTs.Visitor;
using KaVE.Commons.Utils.Collections;

namespace KaVE.Commons.Model.SSTs.Impl.Visitor
{
    public class LinesOfCodeVisitor : ISSTNodeVisitor<int, int>
    {
        public int Visit(ISST sst, int context)
        {
            var size = 1;

            foreach (var e in sst.Events)
            {
                size += e.Accept(this, 0);
            }

            foreach (var f in sst.Fields)
            {
                size += f.Accept(this, 0);
            }

            foreach (var m in sst.Methods)
            {
                size += m.Accept(this, 0);
            }

            foreach (var p in sst.Properties)
            {
                size += p.Accept(this, 0);
            }

            return size;
        }

        public int Visit(IDelegateDeclaration decl, int context)
        {
            return 1;
        }

        public int Visit(IEventDeclaration decl, int context)
        {
            return 1;
        }

        public int Visit(IFieldDeclaration decl, int context)
        {
            return 1;
        }

        public int Visit(IMethodDeclaration decl, int context)
        {
            return 1 + Visit(decl.Body, 0);
        }

        private int Visit(IKaVEList<IStatement> body, int context)
        {
            int size = 0;
            foreach (var stmt in body)
            {
                size += stmt.Accept(this, 0);
            }

            return size;
        }

        public int Visit(IPropertyDeclaration decl, int context)
        {
            return 1 + Visit(decl.Get, 0) + Visit(decl.Set, 0);
        }

        public int Visit(IVariableDeclaration decl, int context)
        {
            return 1;
        }

        public int Visit(IAssignment stmt, int context)
        {
            return 1;
        }

        public int Visit(IBreakStatement stmt, int context)
        {
            return 1;
        }

        public int Visit(IContinueStatement stmt, int context)
        {
            return 1;
        }

        public int Visit(IEventSubscriptionStatement stmt, int context)
        {
            return 1;
        }

        public int Visit(IExpressionStatement stmt, int context)
        {
            return 1;
        }

        public int Visit(IGotoStatement stmt, int context)
        {
            return 1;
        }

        public int Visit(ILabelledStatement stmt, int context)
        {
            return 1;
        }

        public int Visit(IReturnStatement stmt, int context)
        {
            return 1;
        }

        public int Visit(IThrowStatement stmt, int context)
        {
            return 1;
        }

        public int Visit(IDoLoop block, int context)
        {
            return 1 + Visit(block.Body, 0);
        }

        public int Visit(IForEachLoop block, int context)
        {
            return 1 + Visit(block.Body, 0);
        }

        public int Visit(IForLoop block, int context)
        {
            return 1 + Visit(block.Body, 0);
        }

        public int Visit(IIfElseBlock block, int context)
        {
            return 1 + Visit(block.Then, 0) + Visit(block.Else, 0);
        }

        public int Visit(ILockBlock block, int context)
        {
            return 1 + Visit(block.Body, 0);
        }

        public int Visit(ISwitchBlock block, int context)
        {
            var size = 1;

            foreach (var c in block.Sections)
            {
                size += Visit(c.Body, 0);
            }

            size += Visit(block.DefaultSection, 0);

            return size;
        }

        public int Visit(ITryBlock block, int context)
        {
            var size = 1;
            size += Visit(block.Body, 0);
            foreach (var c in block.CatchBlocks)
            {
                size += Visit(c.Body, 0);
            }
            return size;
        }

        public int Visit(IUncheckedBlock block, int context)
        {
            return 1 + Visit(block.Body, 0);
        }

        public int Visit(IUnsafeBlock block, int context)
        {
            return 1;
        }

        public int Visit(IUsingBlock block, int context)
        {
            return 1 + Visit(block.Body, 0);
        }

        public int Visit(IWhileLoop block, int context)
        {
            return 1 + Visit(block.Body, 0);
        }

        public int Visit(ICompletionExpression expr, int context)
        {
            throw new NotImplementedException();
        }

        public int Visit(IComposedExpression expr, int context)
        {
            throw new NotImplementedException();
        }

        public int Visit(IIfElseExpression expr, int context)
        {
            throw new NotImplementedException();
        }

        public int Visit(IInvocationExpression expr, int context)
        {
            throw new NotImplementedException();
        }

        public int Visit(ILambdaExpression expr, int context)
        {
            throw new NotImplementedException();
        }

        public int Visit(ILoopHeaderBlockExpression expr, int context)
        {
            throw new NotImplementedException();
        }

        public int Visit(IConstantValueExpression expr, int context)
        {
            throw new NotImplementedException();
        }

        public int Visit(INullExpression expr, int context)
        {
            throw new NotImplementedException();
        }

        public int Visit(IReferenceExpression expr, int context)
        {
            throw new NotImplementedException();
        }

        public int Visit(ICastExpression expr, int context)
        {
            throw new NotImplementedException();
        }

        public int Visit(IIndexAccessExpression expr, int context)
        {
            throw new NotImplementedException();
        }

        public int Visit(ITypeCheckExpression expr, int context)
        {
            throw new NotImplementedException();
        }

        public int Visit(IUnaryExpression expr, int context)
        {
            throw new NotImplementedException();
        }

        public int Visit(IBinaryExpression expr, int context)
        {
            throw new NotImplementedException();
        }

        public int Visit(IEventReference eventRef, int context)
        {
            throw new NotImplementedException();
        }

        public int Visit(IFieldReference fieldRef, int context)
        {
            throw new NotImplementedException();
        }

        public int Visit(IMethodReference methodRef, int context)
        {
            throw new NotImplementedException();
        }

        public int Visit(IPropertyReference propertyRef, int context)
        {
            throw new NotImplementedException();
        }

        public int Visit(IVariableReference varRef, int context)
        {
            throw new NotImplementedException();
        }

        public int Visit(IIndexAccessReference indexAccessRef, int context)
        {
            throw new NotImplementedException();
        }

        public int Visit(IUnknownReference unknownRef, int context)
        {
            throw new NotImplementedException();
        }

        public int Visit(IUnknownExpression unknownExpr, int context)
        {
            throw new NotImplementedException();
        }

        public int Visit(IUnknownStatement unknownStmt, int context)
        {
            return 1;
        }
    }
}