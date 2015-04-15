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

namespace KaVE.Commons.Utils
{
    public class SSTPrintingVisitor : ISSTNodeVisitor<StringBuilder>
    {
        private int _indentationLevel = 0;

        public void Visit(ISST sst, StringBuilder sb)
        {
            sb.AppendFormat("class {0}", sst.EnclosingType.Name).AppendLine()
              .AppendLine("{");

            // TODO: visit delegates
            // TODO: visit events
            // TODO: visit fields
            // TODO: visit properties
            // TODO: visit methods
            // TODO: visit entrypoints/nonentrypoints?

            sb.Append("}");
        }

        public void Visit(IDelegateDeclaration stmt, StringBuilder sb)
        {
            sb.AppendFormat("delegate {0}();", stmt.Name.Name);
        }

        public void Visit(IEventDeclaration stmt, StringBuilder sb)
        {
            // TODO: HandlerType with generics?
            sb.AppendFormat("event {0} {1};", stmt.Name.HandlerType.Name, stmt.Name.Name);
        }

        public void Visit(IFieldDeclaration stmt, StringBuilder sb)
        {
            // CHECK: static? print other options on .Name.ValueType?
            sb.AppendFormat("{0} {1};", stmt.Name.ValueType.Name, stmt.Name.Name);
        }

        public void Visit(IMethodDeclaration stmt, StringBuilder sb)
        {
            throw new NotImplementedException();
        }

        public void Visit(IPropertyDeclaration stmt, StringBuilder sb)
        {
            sb.AppendIndentation(_indentationLevel).AppendFormat("{0} {1}", stmt.Name.ValueType.Name, stmt.Name.Name);

            var getterBodyExists = stmt.Get.Any();
            var setterBodyExists = stmt.Set.Any();
            var hasBody = getterBodyExists || setterBodyExists;

            if (hasBody) // Long version: At least one body exists --> line breaks + indentation
            {
                sb.AppendLine()
                  .AppendIndentation(_indentationLevel++).AppendLine("{");

                if (stmt.Name.HasGetter)
                {
                    if (getterBodyExists)
                    {
                        sb.AppendIndentation(_indentationLevel).AppendLine("get");
                        AppendBlock(stmt.Get, sb);
                    }
                    else
                    {
                        sb.AppendIndentation(_indentationLevel).Append("get;");
                    }

                    sb.AppendLine();
                }

                if (stmt.Name.HasSetter)
                {
                    if (setterBodyExists)
                    {
                        sb.AppendIndentation(_indentationLevel).AppendLine("set");
                        AppendBlock(stmt.Set, sb);
                    }
                    else
                    {
                        sb.AppendIndentation(_indentationLevel).Append("set;");
                    }

                    sb.AppendLine();
                }

                sb.AppendIndentation(--_indentationLevel).Append("};");
            }
            else // Short Version: No bodies --> getter/setter declaration in same line
            {
                sb.Append(" { ");
                if (stmt.Name.HasGetter)
                {
                    sb.Append("get; ");
                }
                if (stmt.Name.HasSetter)
                {
                    sb.Append("set; ");
                }
                sb.Append("};");
            }
        }

        private void AppendBlock(IEnumerable<IStatement> block, StringBuilder sb)
        {
            sb.AppendIndentation(_indentationLevel++).AppendLine("{");
            foreach (var s in block)
            {
                s.Accept(this, sb);
                sb.AppendLine();
            }
            sb.AppendIndentation(--_indentationLevel).Append("}");
        }

        public void Visit(IVariableDeclaration stmt, StringBuilder sb)
        {
            throw new NotImplementedException();
        }

        public void Visit(IAssignment stmt, StringBuilder sb)
        {
            throw new NotImplementedException();
        }

        public void Visit(IBreakStatement stmt, StringBuilder sb)
        {
            sb.AppendIndentation(_indentationLevel).Append("break;");
        }

        public void Visit(IContinueStatement stmt, StringBuilder sb)
        {
            sb.AppendIndentation(_indentationLevel).Append("continue;");
        }

        public void Visit(IExpressionStatement stmt, StringBuilder sb)
        {
            throw new NotImplementedException();
        }

        public void Visit(IGotoStatement stmt, StringBuilder sb)
        {
            throw new NotImplementedException();
        }

        public void Visit(ILabelledStatement stmt, StringBuilder sb)
        {
            throw new NotImplementedException();
        }

        public void Visit(IReturnStatement stmt, StringBuilder sb)
        {
            throw new NotImplementedException();
        }

        public void Visit(IThrowStatement stmt, StringBuilder sb)
        {
            throw new NotImplementedException();
        }

        public void Visit(IDoLoop block, StringBuilder sb)
        {
            throw new NotImplementedException();
        }

        public void Visit(IForEachLoop block, StringBuilder sb)
        {
            throw new NotImplementedException();
        }

        public void Visit(IForLoop block, StringBuilder sb)
        {
            throw new NotImplementedException();
        }

        public void Visit(IIfElseBlock block, StringBuilder sb)
        {
            throw new NotImplementedException();
        }

        public void Visit(ILockBlock stmt, StringBuilder sb)
        {
            throw new NotImplementedException();
        }

        public void Visit(ISwitchBlock block, StringBuilder sb)
        {
            throw new NotImplementedException();
        }

        public void Visit(ITryBlock block, StringBuilder sb)
        {
            throw new NotImplementedException();
        }

        public void Visit(IUncheckedBlock block, StringBuilder sb)
        {
            throw new NotImplementedException();
        }

        public void Visit(IUnsafeBlock block, StringBuilder sb)
        {
            throw new NotImplementedException();
        }

        public void Visit(IUsingBlock block, StringBuilder sb)
        {
            throw new NotImplementedException();
        }

        public void Visit(IWhileLoop block, StringBuilder sb)
        {
            throw new NotImplementedException();
        }

        public void Visit(ICompletionExpression entity, StringBuilder sb)
        {
            throw new NotImplementedException();
        }

        public void Visit(IComposedExpression expr, StringBuilder sb)
        {
            throw new NotImplementedException();
        }

        public void Visit(IIfElseExpression expr, StringBuilder sb)
        {
            throw new NotImplementedException();
        }

        public void Visit(IInvocationExpression entity, StringBuilder sb)
        {
            throw new NotImplementedException();
        }

        public void Visit(ILambdaExpression expr, StringBuilder sb)
        {
            throw new NotImplementedException();
        }

        public void Visit(ILoopHeaderBlockExpression expr, StringBuilder sb)
        {
            throw new NotImplementedException();
        }

        public void Visit(IConstantValueExpression expr, StringBuilder sb)
        {
            throw new NotImplementedException();
        }

        public void Visit(INullExpression expr, StringBuilder sb)
        {
            throw new NotImplementedException();
        }

        public void Visit(IReferenceExpression expr, StringBuilder sb)
        {
            throw new NotImplementedException();
        }

        public void Visit(IEventReference eventRef, StringBuilder sb)
        {
            throw new NotImplementedException();
        }

        public void Visit(IFieldReference fieldRef, StringBuilder sb)
        {
            throw new NotImplementedException();
        }

        public void Visit(IMethodReference methodRef, StringBuilder sb)
        {
            throw new NotImplementedException();
        }

        public void Visit(IPropertyReference methodRef, StringBuilder sb)
        {
            throw new NotImplementedException();
        }

        public void Visit(IVariableReference varRef, StringBuilder sb)
        {
            throw new NotImplementedException();
        }

        public void Visit(IUnknownReference unknownRef, StringBuilder sb)
        {
            throw new NotImplementedException();
        }

        public void Visit(IUnknownExpression unknownExpr, StringBuilder sb)
        {
            throw new NotImplementedException();
        }

        public void Visit(IUnknownStatement unknownStmt, StringBuilder sb)
        {
            throw new NotImplementedException();
        }
    }

    internal static class SSTPrintingVisitorHelper
    {
        private const string IndentationToken = "    ";

        public static StringBuilder AppendIndentation(this StringBuilder sb, int indentationLevel)
        {
            for (int i = 0; i < indentationLevel; i++)
            {
                sb.Append(IndentationToken);
            }

            return sb;
        }
    }
}