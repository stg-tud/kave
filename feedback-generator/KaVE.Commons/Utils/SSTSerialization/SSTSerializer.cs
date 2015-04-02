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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Blocks;
using KaVE.Commons.Model.SSTs.Declarations;
using KaVE.Commons.Model.SSTs.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Expressions.LoopHeader;
using KaVE.Commons.Model.SSTs.Expressions.Simple;
using KaVE.Commons.Model.SSTs.References;
using KaVE.Commons.Model.SSTs.Statements;
using KaVE.Commons.Model.SSTs.Visitor;

namespace KaVE.Commons.Utils.SSTSerialization
{
    public class SSTSerializer : ISSTNodeVisitor<StringBuilder>
    {
        public void Visit(ISST sst, StringBuilder context)
        {
            context.AppendFormat("class {0} {{}}", sst.EnclosingType.Name);
        }

        public void Visit(IDelegateDeclaration stmt, StringBuilder context)
        {
            throw new NotImplementedException();
        }

        public void Visit(IEventDeclaration stmt, StringBuilder context)
        {
            throw new NotImplementedException();
        }

        public void Visit(IFieldDeclaration stmt, StringBuilder context)
        {
            throw new NotImplementedException();
        }

        public void Visit(IMethodDeclaration stmt, StringBuilder context)
        {
            throw new NotImplementedException();
        }

        public void Visit(IPropertyDeclaration stmt, StringBuilder context)
        {
            throw new NotImplementedException();
        }

        public void Visit(IVariableDeclaration stmt, StringBuilder context)
        {
            throw new NotImplementedException();
        }

        public void Visit(IAssignment stmt, StringBuilder context)
        {
            throw new NotImplementedException();
        }

        public void Visit(IBreakStatement stmt, StringBuilder context)
        {
            throw new NotImplementedException();
        }

        public void Visit(IContinueStatement stmt, StringBuilder context)
        {
            throw new NotImplementedException();
        }

        public void Visit(IExpressionStatement stmt, StringBuilder context)
        {
            throw new NotImplementedException();
        }

        public void Visit(IGotoStatement stmt, StringBuilder context)
        {
            throw new NotImplementedException();
        }

        public void Visit(ILabelledStatement stmt, StringBuilder context)
        {
            throw new NotImplementedException();
        }

        public void Visit(IReturnStatement stmt, StringBuilder context)
        {
            throw new NotImplementedException();
        }

        public void Visit(IThrowStatement stmt, StringBuilder context)
        {
            throw new NotImplementedException();
        }

        public void Visit(IDoLoop block, StringBuilder context)
        {
            throw new NotImplementedException();
        }

        public void Visit(IForEachLoop block, StringBuilder context)
        {
            throw new NotImplementedException();
        }

        public void Visit(IForLoop block, StringBuilder context)
        {
            throw new NotImplementedException();
        }

        public void Visit(IIfElseBlock block, StringBuilder context)
        {
            throw new NotImplementedException();
        }

        public void Visit(ILockBlock stmt, StringBuilder context)
        {
            throw new NotImplementedException();
        }

        public void Visit(ISwitchBlock block, StringBuilder context)
        {
            throw new NotImplementedException();
        }

        public void Visit(ITryBlock block, StringBuilder context)
        {
            throw new NotImplementedException();
        }

        public void Visit(IUncheckedBlock block, StringBuilder context)
        {
            throw new NotImplementedException();
        }

        public void Visit(IUnsafeBlock block, StringBuilder context)
        {
            throw new NotImplementedException();
        }

        public void Visit(IUsingBlock block, StringBuilder context)
        {
            throw new NotImplementedException();
        }

        public void Visit(IWhileLoop block, StringBuilder context)
        {
            throw new NotImplementedException();
        }

        public void Visit(ICompletionExpression entity, StringBuilder context)
        {
            throw new NotImplementedException();
        }

        public void Visit(IComposedExpression expr, StringBuilder context)
        {
            throw new NotImplementedException();
        }

        public void Visit(IIfElseExpression expr, StringBuilder context)
        {
            throw new NotImplementedException();
        }

        public void Visit(IInvocationExpression entity, StringBuilder context)
        {
            throw new NotImplementedException();
        }

        public void Visit(ILambdaExpression expr, StringBuilder context)
        {
            throw new NotImplementedException();
        }

        public void Visit(ILoopHeaderBlockExpression expr, StringBuilder context)
        {
            throw new NotImplementedException();
        }

        public void Visit(IConstantValueExpression expr, StringBuilder context)
        {
            throw new NotImplementedException();
        }

        public void Visit(INullExpression expr, StringBuilder context)
        {
            throw new NotImplementedException();
        }

        public void Visit(IReferenceExpression expr, StringBuilder context)
        {
            throw new NotImplementedException();
        }

        public void Visit(IEventReference eventRef, StringBuilder context)
        {
            throw new NotImplementedException();
        }

        public void Visit(IFieldReference fieldRef, StringBuilder context)
        {
            throw new NotImplementedException();
        }

        public void Visit(IMethodReference methodRef, StringBuilder context)
        {
            throw new NotImplementedException();
        }

        public void Visit(IPropertyReference methodRef, StringBuilder context)
        {
            throw new NotImplementedException();
        }

        public void Visit(IVariableReference varRef, StringBuilder context)
        {
            throw new NotImplementedException();
        }

        public void Visit(IUnknownReference unknownRef, StringBuilder context)
        {
            throw new NotImplementedException();
        }

        public void Visit(IUnknownExpression unknownExpr, StringBuilder context)
        {
            throw new NotImplementedException();
        }

        public void Visit(IUnknownStatement unknownStmt, StringBuilder context)
        {
            throw new NotImplementedException();
        }
    }
}
