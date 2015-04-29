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
using System.Diagnostics;
using System.Linq;
using System.Text;
using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Blocks;
using KaVE.Commons.Model.SSTs.Declarations;
using KaVE.Commons.Model.SSTs.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Expressions.LoopHeader;
using KaVE.Commons.Model.SSTs.Expressions.Simple;
using KaVE.Commons.Model.SSTs.References;
using KaVE.Commons.Model.SSTs.Statements;
using KaVE.Commons.Model.SSTs.Visitor;
using KaVE.Commons.Utils.Collections;

namespace KaVE.Commons.Utils
{
    public class SSTPrintingVisitor : ISSTNodeVisitor<StringBuilder>
    {
        private int _indentationLevel = 0;

        public void Visit(ISST sst, StringBuilder sb)
        {
            // TODO: using list
            sb.AppendFormat("class {0}", sst.EnclosingType.Name).AppendLine()
              .AppendLine("{");

            // TODO: visit delegates
            // TODO: visit events
            // TODO: visit fields
            // TODO: visit properties
            // TODO: visit methods

            sb.Append("}");
        }

        public void Visit(IDelegateDeclaration stmt, StringBuilder sb)
        {
            // TODO: @Sven delegate parameters
            sb.AppendIndentation(_indentationLevel)
                .AppendFormat("delegate {0}();", stmt.Name.Name);
        }

        public void Visit(IEventDeclaration stmt, StringBuilder sb)
        {
            sb.AppendIndentation(_indentationLevel)
                .AppendFormat("event {0} {1};", FormatTypeName(stmt.Name.HandlerType), stmt.Name.Name);
        }

        public void Visit(IFieldDeclaration stmt, StringBuilder sb)
        {
            sb.AppendIndentation(_indentationLevel);

            if (stmt.Name.IsStatic)
            {
                sb.Append("static ");
            }

            sb.AppendFormat("{0} {1};", FormatTypeName(stmt.Name.ValueType), stmt.Name.Name);
        }

        public void Visit(IMethodDeclaration stmt, StringBuilder sb)
        {
            sb.AppendIndentation(_indentationLevel);

            if (stmt.Name.IsStatic)
            {
                sb.Append("static ");
            }

            sb.AppendFormat("{0} {1}(", FormatTypeName(stmt.Name.ReturnType), stmt.Name.Name);

            foreach (var parameter in stmt.Name.Parameters)
            {
                if (parameter.IsPassedByReference && parameter.ValueType.IsValueType)
                {
                    sb.Append("ref ");
                }

                if (parameter.IsOutput)
                {
                    sb.Append("out ");
                }

                if (parameter.IsOptional)
                {
                    sb.Append("opt ");
                }

                if (parameter.IsParameterArray)
                {
                    sb.Append("params ");
                }

                sb.AppendFormat("{0} {1}", FormatTypeName(parameter.ValueType), parameter.Name);

                if (!ReferenceEquals(parameter, stmt.Name.Parameters.Last()))
                {
                    sb.Append(", ");
                }
            }

            sb.Append(")");

            AppendBlock(stmt.Body, sb);
        }

        public void Visit(IPropertyDeclaration stmt, StringBuilder sb)
        {
            sb.AppendIndentation(_indentationLevel).AppendFormat("{0} {1}", FormatTypeName(stmt.Name.ValueType), stmt.Name.Name);

            var hasBody = stmt.Get.Any() || stmt.Set.Any();

            if (hasBody) // Long version: At least one body exists --> line breaks + indentation
            {
                sb.AppendLine()
                  .AppendIndentation(_indentationLevel++).AppendLine("{");

                if (stmt.Name.HasGetter)
                {
                    AppendPropertyAccessor(sb, stmt.Get, "get");
                }

                if (stmt.Name.HasSetter)
                {
                    AppendPropertyAccessor(sb, stmt.Set, "set");
                }

                sb.AppendIndentation(--_indentationLevel).Append("}");
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
                sb.Append("}");
            }
        }

        private void AppendPropertyAccessor(StringBuilder sb, IKaVEList<IStatement> body, string keyword)
        {
            if (body.Any())
            {
                sb.AppendIndentation(_indentationLevel).Append(keyword);
                AppendBlock(body, sb);
            }
            else
            {
                sb.AppendIndentation(_indentationLevel).Append(keyword).Append(";");
            }

            sb.AppendLine();
        }

        public void Visit(IVariableDeclaration stmt, StringBuilder sb)
        {
            sb.AppendIndentation(_indentationLevel).Append(FormatTypeName(stmt.Type)).Append(" ");
            stmt.Reference.Accept(this, sb);
            sb.Append(";");
        }

        public void Visit(IAssignment stmt, StringBuilder sb)
        {
            sb.AppendIndentation(_indentationLevel);
            stmt.Reference.Accept(this, sb);
            sb.Append(" = ");
            stmt.Expression.Accept(this, sb);
            sb.Append(";");
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
            sb.AppendIndentation(_indentationLevel);
            stmt.Expression.Accept(this, sb);
            sb.Append(";");
        }

        public void Visit(IGotoStatement stmt, StringBuilder sb)
        {
            sb.AppendIndentation(_indentationLevel).AppendFormat("goto {0};", stmt.Label);
        }

        public void Visit(ILabelledStatement stmt, StringBuilder sb)
        {
            sb.AppendIndentation(_indentationLevel).AppendFormat("{0}:", stmt.Label).AppendLine();
            stmt.Statement.Accept(this, sb);
        }

        public void Visit(IReturnStatement stmt, StringBuilder sb)
        {
            // TODO: Void return seems to be impossible --> stmt.Expression declared as NotNull, interpreting existing expressions as void return makes no sense
            sb.AppendIndentation(_indentationLevel).AppendFormat("return ");
            stmt.Expression.Accept(this, sb);
            sb.Append(";");
        }

        public void Visit(IThrowStatement stmt, StringBuilder sb)
        {
            sb.AppendIndentation(_indentationLevel).AppendFormat("throw new {0}();", stmt.Exception.Name);
        }

        public void Visit(IDoLoop block, StringBuilder sb)
        {
            sb.AppendIndentation(_indentationLevel).Append("do");

            AppendBlock(block.Body, sb);

            sb.AppendLine().AppendIndentation(_indentationLevel).Append("while (");
            _indentationLevel++;
            block.Condition.Accept(this, sb);
            _indentationLevel--;
            sb.AppendLine().AppendIndentation(_indentationLevel).Append(")");

        }

        public void Visit(IForEachLoop block, StringBuilder sb)
        {
            sb.AppendIndentation(_indentationLevel)
              .Append("foreach (")
              .Append(FormatTypeName(block.Declaration.Type))
              .Append(" ");
            block.Declaration.Reference.Accept(this, sb);
            sb.Append(" in ");
            block.LoopedReference.Accept(this, sb);
            sb.Append(")");

            AppendBlock(block.Body, sb);
        }

        public void Visit(IForLoop block, StringBuilder sb)
        {
            sb.AppendIndentation(_indentationLevel).Append("for (");

            _indentationLevel++;

            AppendBlock(block.Init, sb);
            sb.Append(";");
            block.Condition.Accept(this, sb);
            sb.Append(";");
            AppendBlock(block.Step, sb);

            _indentationLevel--;

            sb.AppendLine().AppendIndentation(_indentationLevel).Append(")");

            AppendBlock(block.Body, sb);
        }

        public void Visit(IIfElseBlock block, StringBuilder sb)
        {
            sb.AppendIndentation(_indentationLevel).Append("if (");
            block.Condition.Accept(this, sb);
            sb.Append(")");

            AppendBlock(block.Then, sb);

            if (block.Else.Any())
            {
                sb.AppendLine().AppendIndentation(_indentationLevel).Append("else");

                AppendBlock(block.Else, sb);
            }
        }

        public void Visit(ILockBlock stmt, StringBuilder sb)
        {
            sb.AppendIndentation(_indentationLevel).Append("lock (");
            stmt.Reference.Accept(this, sb);
            sb.Append(")");

            AppendBlock(stmt.Body, sb);
        }

        public void Visit(ISwitchBlock block, StringBuilder sb)
        {
            sb.AppendIndentation(_indentationLevel).AppendFormat("switch (");
            block.Reference.Accept(this, sb);
            sb.Append(")").AppendLine().AppendIndentation(_indentationLevel++).Append("{");

            foreach (var section in block.Sections)
            {
                sb.AppendLine();

                sb.AppendIndentation(_indentationLevel).AppendFormat("case ");
                section.Label.Accept(this, sb);
                sb.Append(":");
                AppendBlock(section.Body, sb, false);
            }

            if (block.DefaultSection.Any())
            {
                sb.AppendLine().AppendIndentation(_indentationLevel).Append("default:");
                AppendBlock(block.DefaultSection, sb, false);
            }

            sb.AppendLine().AppendIndentation(--_indentationLevel).Append("}");
        }

        public void Visit(ITryBlock block, StringBuilder sb)
        {
            sb.AppendIndentation(_indentationLevel).Append("try");
            AppendBlock(block.Body, sb);

            foreach (var catchBlock in block.CatchBlocks)
            {
                sb.AppendLine().AppendIndentation(_indentationLevel).Append("catch");

                if (!catchBlock.Exception.IsMissing)
                {
                    sb.Append(" (")
                      .Append(FormatTypeName(catchBlock.Exception.Type))
                      .Append(" ");
                    catchBlock.Exception.Reference.Accept(this, sb);
                    sb.Append(")");
                }

                AppendBlock(catchBlock.Body, sb);
            }

            if (block.Finally.Any())
            {
                sb.AppendLine().AppendIndentation(_indentationLevel).Append("finally");
                AppendBlock(block.Finally, sb);
            }
        }

        public void Visit(IUncheckedBlock block, StringBuilder sb)
        {
            sb.AppendIndentation(_indentationLevel).Append("unchecked");
            AppendBlock(block.Body, sb);
        }

        public void Visit(IUnsafeBlock block, StringBuilder sb)
        {
            sb.AppendIndentation(_indentationLevel).Append("unsafe { /* content ignored */ }");
        }

        public void Visit(IUsingBlock block, StringBuilder sb)
        {
            sb.AppendIndentation(_indentationLevel).Append("using (");
            block.Reference.Accept(this, sb);
            sb.Append(")");

            AppendBlock(block.Body, sb);
        }

        public void Visit(IWhileLoop block, StringBuilder sb)
        {
            sb.AppendIndentation(_indentationLevel).Append("while (");
            _indentationLevel++;
            block.Condition.Accept(this, sb);
            _indentationLevel--;
            sb.AppendLine().AppendIndentation(_indentationLevel).Append(")");

            AppendBlock(block.Body, sb);
        }

        public void Visit(ICompletionExpression entity, StringBuilder sb)
        {
            if (entity.ObjectReference != null)
            {
                sb.Append(entity.ObjectReference.Identifier).Append(".");
            }
            else if (entity.TypeReference != null)
            {
                sb.Append(entity.TypeReference.Name).Append(".");
            }

            sb.Append(entity.Token).Append("$");
        }

        public void Visit(IComposedExpression expr, StringBuilder sb)
        {
            sb.Append("composed(");

            foreach (var reference in expr.References)
            {
                reference.Accept(this, sb);

                if (!ReferenceEquals(reference, expr.References.Last()))
                {
                    sb.Append(", ");
                }
            }

            sb.Append(")");
        }

        public void Visit(IIfElseExpression expr, StringBuilder sb)
        {
            sb.Append("(");
            expr.Condition.Accept(this, sb);
            sb.Append(") ? ");
            expr.ThenExpression.Accept(this, sb);
            sb.Append(" : ");
            expr.ElseExpression.Accept(this, sb);
        }

        public void Visit(IInvocationExpression expr, StringBuilder sb)
        {
            expr.Reference.Accept(this, sb);
            sb.Append(".").AppendFormat("{0}(", expr.MethodName.Name);

            foreach (var parameter in expr.Parameters)
            {
                parameter.Accept(this, sb);

                if (!ReferenceEquals(parameter, expr.Parameters.Last()))
                {
                    sb.Append(", ");
                }
            }

            sb.Append(")");
        }

        public void Visit(ILambdaExpression expr, StringBuilder sb)
        {
            // TODO: problem with VariableDeclaration again!
            throw new NotImplementedException();
        }

        // TODO: make this a bit nicer ...
        public void Visit(ILoopHeaderBlockExpression expr, StringBuilder sb)
        {
            AppendBlock(expr.Body, sb);
        }

        public void Visit(IConstantValueExpression expr, StringBuilder sb)
        {
            string value = expr.Value ?? "null";

            double parsed;
            if (Double.TryParse(expr.Value, out parsed)
                || value.Equals("false", StringComparison.Ordinal)
                || value.Equals("true", StringComparison.Ordinal)
                || value.Equals("null", StringComparison.Ordinal))
            {
                sb.Append(value);
            }
            else
            {
                sb.AppendFormat("\"{0}\"", value);
            }
        }

        public void Visit(INullExpression expr, StringBuilder sb)
        {
            sb.Append("null");
        }

        public void Visit(IReferenceExpression expr, StringBuilder sb)
        {
            expr.Reference.Accept(this, sb);
        }

        public void Visit(IEventReference eventRef, StringBuilder sb)
        {
            sb.Append(eventRef.Reference.Identifier);
        }

        public void Visit(IFieldReference fieldRef, StringBuilder sb)
        {
            sb.Append(fieldRef.Reference.Identifier);
        }

        public void Visit(IMethodReference methodRef, StringBuilder sb)
        {
            sb.Append(methodRef.Reference.Identifier);
        }

        public void Visit(IPropertyReference propertyRef, StringBuilder sb)
        {
            sb.Append(propertyRef.Reference.Identifier);
        }

        public void Visit(IVariableReference varRef, StringBuilder sb)
        {
            sb.Append(varRef.Identifier);
        }

        public void Visit(IUnknownReference unknownRef, StringBuilder sb)
        {
            sb.Append("???");
        }

        public void Visit(IUnknownExpression unknownExpr, StringBuilder sb)
        {
            sb.Append("???");
        }

        public void Visit(IUnknownStatement unknownStmt, StringBuilder sb)
        {
            sb.AppendIndentation(_indentationLevel).Append("???;");
        }

        // TODO: write tests
        // TODO: append to string builder instead of returning string
        private string FormatTypeName(ITypeName typeName)
        {
            if (typeName.IsTypeParameter)
            {
                return typeName.TypeParameterType.Name;
            }

            var sb = new StringBuilder();
            sb.Append(typeName.Name);

            if (typeName.HasTypeParameters)
            {
                sb.Append("<");

                foreach (var p in typeName.TypeParameters)
                {
                    sb.Append(FormatTypeName(p));
                }

                sb.Append(">");
            }

            return sb.ToString();
        }

        // TODO: write tests
        /// <summary>
        /// Appends the print result of a statement block to a string builder with correct indentation.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="sb"></param>
        /// <param name="withBrackets">If false, opening and closing brackets will be omitted.</param>
        internal void AppendBlock(IKaVEList<IStatement> block, StringBuilder sb, bool withBrackets = true)
        {
            if (!block.Any())
            {
                if (withBrackets)
                {
                    sb.Append(" { }");
                }

                return;
            }

            if (withBrackets)
            {
                sb.AppendLine().AppendIndentation(_indentationLevel).Append("{");
            }

            _indentationLevel++;

            foreach (var statement in block)
            {
                sb.AppendLine();
                statement.Accept(this, sb);
            }

            _indentationLevel--;

            if (withBrackets)
            {
                sb.AppendLine().AppendIndentation(_indentationLevel).Append("}");
            }
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