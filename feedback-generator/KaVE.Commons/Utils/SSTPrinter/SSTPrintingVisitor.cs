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
using System.Collections.Generic;
using System.Linq;
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

namespace KaVE.Commons.Utils.SSTPrinter
{
    public class SSTPrintingVisitor : ISSTNodeVisitor<SSTPrintingContext>
    {
        public void Visit(ISST sst, SSTPrintingContext c)
        {
            c.Indentation();

            if (!string.IsNullOrEmpty(sst.PartialClassIdentifier))
            {
                c.Keyword("partial").Space();
            }

            if (sst.EnclosingType.IsInterfaceType)
            {
                c.Keyword("interface");
            }
            else if (sst.EnclosingType.IsEnumType)
            {
                c.Keyword("enum");
            }
            else if (sst.EnclosingType.IsStructType)
            {
                c.Keyword("struct");
            }
            else
            {
                c.Keyword("class");
            }

            c.Space().Type(sst.EnclosingType);

            if (c.TypeShape != null && c.TypeShape.TypeHierarchy.HasSupertypes)
            {
                c.Text(" : ");

                if (c.TypeShape.TypeHierarchy.HasSuperclass && c.TypeShape.TypeHierarchy.Extends != null)
                {
                    c.Type(c.TypeShape.TypeHierarchy.Extends.Element);

                    if (c.TypeShape.TypeHierarchy.IsImplementingInterfaces)
                    {
                        c.Text(", ");
                    }
                }

                foreach (var i in c.TypeShape.TypeHierarchy.Implements)
                {
                    c.Type(i.Element);

                    if (!ReferenceEquals(i, c.TypeShape.TypeHierarchy.Implements.Last()))
                    {
                        c.Text(", ");
                    }
                }
            }

            c.NewLine()
             .Indentation().Text("{").NewLine();

            c.IndentationLevel++;

            AppendMemberDeclarationGroup(c, sst.Delegates);
            AppendMemberDeclarationGroup(c, sst.Events);
            AppendMemberDeclarationGroup(c, sst.Fields);
            AppendMemberDeclarationGroup(c, sst.Properties);
            AppendMemberDeclarationGroup(c, sst.Methods, 2, 1);

            c.IndentationLevel--;

            c.Indentation().Text("}");
        }

        private void AppendMemberDeclarationGroup(SSTPrintingContext c,
            IEnumerable<IMemberDeclaration> nodeGroup,
            int inBetweenNewLineCount = 1,
            int trailingNewLineCount = 2)
        {
            var sstNodes = nodeGroup.ToList();
            foreach (var node in sstNodes)
            {
                node.Accept(this, c);

                int newLinesNeeded = !ReferenceEquals(node, sstNodes.Last())
                    ? inBetweenNewLineCount
                    : trailingNewLineCount;

                for (int i = 0; i < newLinesNeeded; i++)
                {
                    c.NewLine();
                }
            }
        }

        public void Visit(IDelegateDeclaration stmt, SSTPrintingContext c)
        {
            c.Indentation()
             .Keyword("delegate").Space().Type(stmt.Name)
             .ParameterList(stmt.Name.Parameters)
             .Text(";");
        }

        public void Visit(IEventDeclaration stmt, SSTPrintingContext c)
        {
            c.Indentation()
             .Keyword("event").Space().Type(stmt.Name.HandlerType).Space().Text(stmt.Name.Name).Text(";");
        }

        public void Visit(IFieldDeclaration stmt, SSTPrintingContext c)
        {
            c.Indentation();

            if (stmt.Name.IsStatic)
            {
                c.Keyword("static").Space();
            }

            c.Type(stmt.Name.ValueType).Space().Text(stmt.Name.Name).Text(";");
        }

        public void Visit(IMethodDeclaration stmt, SSTPrintingContext c)
        {
            c.Indentation();

            if (stmt.Name.IsStatic)
            {
                c.Keyword("static").Space();
            }

            c.Type(stmt.Name.ReturnType).Space().Text(stmt.Name.Name);

            if (stmt.Name.HasTypeParameters)
            {
                c.TypeParameters(stmt.Name.TypeParameters);
            }

            c.ParameterList(stmt.Name.Parameters);

            c.StatementBlock(stmt.Body, this);
        }

        public void Visit(IPropertyDeclaration stmt, SSTPrintingContext c)
        {
            c.Indentation().Type(stmt.Name.ValueType).Space().Text(stmt.Name.Name);

            var hasBody = stmt.Get.Any() || stmt.Set.Any();

            if (hasBody) // Long version: At least one body exists --> line breaks + indentation
            {
                c.NewLine()
                 .Indentation();

                c.IndentationLevel++;

                c.Text("{").NewLine();

                if (stmt.Name.HasGetter)
                {
                    AppendPropertyAccessor(c, stmt.Get, "get");
                }

                if (stmt.Name.HasSetter)
                {
                    AppendPropertyAccessor(c, stmt.Set, "set");
                }

                c.IndentationLevel--;

                c.Indentation().Text("}");
            }
            else // Short Version: No bodies --> getter/setter declaration in same line
            {
                c.Text(" { ");
                if (stmt.Name.HasGetter)
                {
                    c.Keyword("get").Text(";").Space();
                }
                if (stmt.Name.HasSetter)
                {
                    c.Keyword("set").Text(";").Space();
                }
                c.Text("}");
            }
        }

        private void AppendPropertyAccessor(SSTPrintingContext c, IKaVEList<IStatement> body, string keyword)
        {
            if (body.Any())
            {
                c.Indentation().Text(keyword);
                c.StatementBlock(body, this);
            }
            else
            {
                c.Indentation().Text(keyword).Text(";");
            }

            c.NewLine();
        }

        public void Visit(IVariableDeclaration stmt, SSTPrintingContext c)
        {
            c.Indentation().Type(stmt.Type).Space();
            stmt.Reference.Accept(this, c);
            c.Text(";");
        }

        public void Visit(IAssignment stmt, SSTPrintingContext c)
        {
            c.Indentation();
            stmt.Reference.Accept(this, c);
            c.Text(" = ");
            stmt.Expression.Accept(this, c);
            c.Text(";");
        }

        public void Visit(IBreakStatement stmt, SSTPrintingContext c)
        {
            c.Indentation().Keyword("break").Text(";");
        }

        public void Visit(IContinueStatement stmt, SSTPrintingContext c)
        {
            c.Indentation().Keyword("continue").Text(";");
        }

        public void Visit(IEventSubscriptionStatement stmt, SSTPrintingContext c)
        {
            c.Indentation();
            stmt.Reference.Accept(this, c);
            c.Space();

            if (stmt.Operation == EventSubscriptionOperation.Add)
            {
                c.Text("+=");
            }
            else if (stmt.Operation == EventSubscriptionOperation.Remove)
            {
                c.Text("-=");
            }

            c.Space();
            stmt.Expression.Accept(this, c);
            c.Text(";");
        }

        public void Visit(IExpressionStatement stmt, SSTPrintingContext c)
        {
            c.Indentation();
            stmt.Expression.Accept(this, c);
            c.Text(";");
        }

        public void Visit(IGotoStatement stmt, SSTPrintingContext c)
        {
            c.Indentation().Keyword("goto").Space().Text(stmt.Label).Text(";");
        }

        public void Visit(ILabelledStatement stmt, SSTPrintingContext c)
        {
            c.Indentation().Keyword(stmt.Label).Text(":").NewLine();
            stmt.Statement.Accept(this, c);
        }

        public void Visit(IReturnStatement stmt, SSTPrintingContext c)
        {
            c.Indentation().Keyword("return");

            if (!stmt.IsVoid)
            {
                c.Space();
                stmt.Expression.Accept(this, c);
            }

            c.Text(";");
        }

        public void Visit(IThrowStatement stmt, SSTPrintingContext c)
        {
            c.Indentation().Keyword("throw");

            if (!stmt.IsReThrow)
            {
                c.Space();
                stmt.Reference.Accept(this, c);
            }

            c.Text(";");
        }

        public void Visit(IDoLoop block, SSTPrintingContext c)
        {
            c.Indentation().Keyword("do");

            c.StatementBlock(block.Body, this);

            c.NewLine().Indentation().Keyword("while").Space().Text("(");
            c.IndentationLevel++;
            block.Condition.Accept(this, c);
            c.IndentationLevel--;
            c.NewLine().Indentation().Text(")");
        }

        public void Visit(IForEachLoop block, SSTPrintingContext c)
        {
            c.Indentation()
             .Keyword("foreach").Space().Text("(")
             .Type(block.Declaration.Type)
             .Space();
            block.Declaration.Reference.Accept(this, c);
            c.Space().Keyword("in").Space();
            block.LoopedReference.Accept(this, c);
            c.Text(")");

            c.StatementBlock(block.Body, this);
        }

        public void Visit(IForLoop block, SSTPrintingContext c)
        {
            c.Indentation().Keyword("for").Space().Text("(");

            c.IndentationLevel++;

            c.StatementBlock(block.Init, this);
            c.Text(";");
            block.Condition.Accept(this, c);
            c.Text(";");
            c.StatementBlock(block.Step, this);

            c.IndentationLevel--;

            c.NewLine().Indentation().Text(")");

            c.StatementBlock(block.Body, this);
        }

        public void Visit(IIfElseBlock block, SSTPrintingContext c)
        {
            c.Indentation().Keyword("if").Space().Text("(");
            block.Condition.Accept(this, c);
            c.Text(")");

            c.StatementBlock(block.Then, this);

            if (block.Else.Any())
            {
                c.NewLine().Indentation().Keyword("else");

                c.StatementBlock(block.Else, this);
            }
        }

        public void Visit(ILockBlock stmt, SSTPrintingContext c)
        {
            c.Indentation().Keyword("lock").Space().Text("(");
            stmt.Reference.Accept(this, c);
            c.Text(")");

            c.StatementBlock(stmt.Body, this);
        }

        public void Visit(ISwitchBlock block, SSTPrintingContext c)
        {
            c.Indentation().Keyword("switch").Space().Text("(");
            block.Reference.Accept(this, c);
            c.Text(")").NewLine().Indentation();
            c.IndentationLevel++;
            c.Text("{");

            foreach (var section in block.Sections)
            {
                c.NewLine().Indentation().Keyword("case").Space();
                section.Label.Accept(this, c);
                c.Text(":").StatementBlock(section.Body, this, false);
            }

            if (block.DefaultSection.Any())
            {
                c.NewLine().Indentation().Keyword("default").Text(":")
                 .StatementBlock(block.DefaultSection, this, false);
            }

            c.NewLine();
            c.IndentationLevel--;
            c.Indentation().Text("}");
        }

        public void Visit(ITryBlock block, SSTPrintingContext c)
        {
            c.Indentation().Keyword("try")
             .StatementBlock(block.Body, this);

            foreach (var catchBlock in block.CatchBlocks)
            {
                c.NewLine().Indentation().Keyword("catch");

                if (catchBlock.Kind != CatchBlockKind.General)
                {
                    c.Space().Text("(")
                     .Type(catchBlock.Parameter.ValueType);

                    if (catchBlock.Kind != CatchBlockKind.Unnamed)
                    {
                        c.Space()
                         .Text(catchBlock.Parameter.Name);
                    }

                    c.Text(")");
                }

                c.StatementBlock(catchBlock.Body, this);
            }

            if (block.Finally.Any())
            {
                c.NewLine().Indentation().Keyword("finally")
                 .StatementBlock(block.Finally, this);
            }
        }

        public void Visit(IUncheckedBlock block, SSTPrintingContext c)
        {
            c.Indentation().Keyword("unchecked")
             .StatementBlock(block.Body, this);
        }

        public void Visit(IUnsafeBlock block, SSTPrintingContext c)
        {
            c.Indentation().Keyword("unsafe").Text(" { ").Comment("/* content ignored */").Text(" }");
        }

        public void Visit(IUsingBlock block, SSTPrintingContext c)
        {
            c.Indentation().Keyword("using").Space().Text("(");
            block.Reference.Accept(this, c);
            c.Text(")").StatementBlock(block.Body, this);
        }

        public void Visit(IWhileLoop block, SSTPrintingContext c)
        {
            c.Indentation().Keyword("while").Space().Text("(");
            c.IndentationLevel++;
            block.Condition.Accept(this, c);
            c.IndentationLevel--;
            c.NewLine().Indentation().Text(")");

            c.StatementBlock(block.Body, this);
        }

        public void Visit(ICompletionExpression entity, SSTPrintingContext c)
        {
            if (entity.VariableReference != null)
            {
                c.Text(entity.VariableReference.Identifier).Text(".");
            }
            else if (entity.TypeReference != null)
            {
                c.Type(entity.TypeReference).Text(".");
            }

            c.Text(entity.Token).CursorPosition();
        }

        public void Visit(IComposedExpression expr, SSTPrintingContext c)
        {
            c.Keyword("composed").Text("(");

            foreach (var reference in expr.References)
            {
                reference.Accept(this, c);

                AddCommaIfNotLast(reference, expr.References, c);
            }

            c.Text(")");
        }

        public void Visit(IIfElseExpression expr, SSTPrintingContext c)
        {
            c.Text("(");
            expr.Condition.Accept(this, c);
            c.Text(")").Space().Text("?").Space();
            expr.ThenExpression.Accept(this, c);
            c.Space().Text(":").Space();
            expr.ElseExpression.Accept(this, c);
        }

        public void Visit(IInvocationExpression expr, SSTPrintingContext c)
        {
            if (expr.MethodName.IsConstructor)
            {
                c.Keyword("new").Space().Type(expr.MethodName.DeclaringType);
            }
            else
            {
                if (expr.MethodName.IsStatic)
                {
                    c.Text(expr.MethodName.DeclaringType.Name);
                }
                else
                {
                    expr.Reference.Accept(this, c);
                }

                c.Text(".").Text(expr.MethodName.Name);
            }

            c.Text("(");

            foreach (var parameter in expr.Parameters)
            {
                parameter.Accept(this, c);

                AddCommaIfNotLast(parameter, expr.Parameters, c);
            }

            c.Text(")");
        }

        public void Visit(ILambdaExpression expr, SSTPrintingContext c)
        {
            c.ParameterList(expr.Name.Parameters).Space().Text("=>");
            c.StatementBlock(expr.Body, this);
        }

        public void Visit(ILoopHeaderBlockExpression expr, SSTPrintingContext c)
        {
            c.StatementBlock(expr.Body, this);
        }

        public void Visit(IConstantValueExpression expr, SSTPrintingContext c)
        {
            string value = expr.Value ?? "...";

            double parsed;
            if (Double.TryParse(expr.Value, out parsed)
                || value.Equals("false", StringComparison.Ordinal)
                || value.Equals("true", StringComparison.Ordinal))
            {
                c.Keyword(value);
            }
            else
            {
                c.StringLiteral(value);
            }
        }

        public void Visit(INullExpression expr, SSTPrintingContext c)
        {
            c.Keyword("null");
        }

        public void Visit(IReferenceExpression expr, SSTPrintingContext c)
        {
            expr.Reference.Accept(this, c);
        }

        public void Visit(ICastExpression expr, SSTPrintingContext c)
        {
            c.Text("(").Type(expr.TargetType).Text(")").Space();
            expr.Reference.Accept(this, c);
        }

        public void Visit(IIndexAccessExpression expr, SSTPrintingContext c)
        {
            expr.Reference.Accept(this, c);
            c.Text("[");

            foreach (var indexExpr in expr.Indices)
            {
                indexExpr.Accept(this, c);

                AddCommaIfNotLast(indexExpr, expr.Indices, c);
            }

            c.Text("]");
        }

        public void Visit(ITypeCheckExpression expr, SSTPrintingContext c)
        {
            expr.Reference.Accept(this, c);
            c.Space().Keyword("is").Space().Type(expr.Type);
        }

        public void Visit(IUnaryExpression expr, SSTPrintingContext c)
        {
            if (expr.Operator == UnaryOperator.PostIncrement || expr.Operator == UnaryOperator.PostDecrement)
            {
                expr.Operand.Accept(this, c);
                c.Text(expr.Operator.ToPrettyString());
            }
            else
            {
                c.Text(expr.Operator.ToPrettyString());
                expr.Operand.Accept(this, c);
            }
        }

        public void Visit(IBinaryExpression expr, SSTPrintingContext c)
        {
            expr.LeftOperand.Accept(this, c);
            c.Space().Text(expr.Operator.ToPrettyString()).Space();
            expr.RightOperand.Accept(this, c);
        }

        public void Visit(IEventReference eventRef, SSTPrintingContext c)
        {
            if (eventRef.EventName.IsStatic)
            {
                c.Type(eventRef.EventName.DeclaringType);
            }
            else
            {
                c.Text(eventRef.Reference.Identifier);
            }
            c.Text(".").Text(eventRef.EventName.Name);
        }

        public void Visit(IFieldReference fieldRef, SSTPrintingContext c)
        {
            if (fieldRef.FieldName.IsStatic)
            {
                c.Type(fieldRef.FieldName.DeclaringType);
            }
            else
            {
                c.Text(fieldRef.Reference.Identifier);
            }

            c.Text(".").Text(fieldRef.FieldName.Name);
        }

        public void Visit(IMethodReference methodRef, SSTPrintingContext c)
        {
            if (methodRef.MethodName.IsStatic)
            {
                c.Type(methodRef.MethodName.DeclaringType);
            }
            else
            {
                c.Text(methodRef.Reference.Identifier);
            }

            c.Text(".").Text(methodRef.MethodName.Name);
        }

        public void Visit(IPropertyReference propertyRef, SSTPrintingContext c)
        {
            if (propertyRef.PropertyName.IsStatic)
            {
                c.Type(propertyRef.PropertyName.DeclaringType);
            }
            else
            {
                c.Text(propertyRef.Reference.Identifier);
            }

            c.Text(".").Text(propertyRef.PropertyName.Name);
        }

        public void Visit(IVariableReference varRef, SSTPrintingContext c)
        {
            c.Text(varRef.Identifier);
        }

        public void Visit(IIndexAccessReference indexAccessRef, SSTPrintingContext context)
        {
            indexAccessRef.Expression.Accept(this, context);
        }

        public void Visit(IUnknownReference unknownRef, SSTPrintingContext c)
        {
            c.UnknownMarker();
        }

        public void Visit(IUnknownExpression unknownExpr, SSTPrintingContext c)
        {
            c.UnknownMarker();
        }

        public void Visit(IUnknownStatement unknownStmt, SSTPrintingContext c)
        {
            c.Indentation().UnknownMarker().Text(";");
        }

        private static void AddCommaIfNotLast<T>(T node, IKaVEList<T> list, SSTPrintingContext c)
            where T : ISSTNode
        {
            if (!ReferenceEquals(node, list.Last()))
            {
                c.Text(", ");
            }
        }
    }
}