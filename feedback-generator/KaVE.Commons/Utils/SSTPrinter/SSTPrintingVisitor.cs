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

            if (sst.EnclosingType.IsInterfaceType)
            {
                c.Text(c.Keywords.Interface);
            }
            else if (sst.EnclosingType.IsEnumType)
            {
                c.Text(c.Keywords.Enum);
            }
            else if (sst.EnclosingType.IsStructType)
            {
                c.Text(c.Keywords.Struct);
            }
            else
            {
                c.Text(c.Keywords.Class);
            }

            c.Space().Text(sst.EnclosingType.Name).NewLine()
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

        private void AppendMemberDeclarationGroup(SSTPrintingContext c, IEnumerable<IMemberDeclaration> nodeGroup, int inBetweenNewLineCount = 1, int trailingNewLineCount = 2)
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
            // TODO: @Sven delegate parameters
            c.Indentation()
              .Text(c.Keywords.Delegate).Space().Text(stmt.Name.Name).Text("();");
            
        }

        public void Visit(IEventDeclaration stmt, SSTPrintingContext c)
        {
            c.Indentation()
              .Text(c.Keywords.Event).Space().TypeName(stmt.Name.HandlerType).Space().Text(stmt.Name.Name).Text(";");
        }

        public void Visit(IFieldDeclaration stmt, SSTPrintingContext c)
        {
            c.Indentation();

            if (stmt.Name.IsStatic)
            {
                c.Text(c.Keywords.StaticModifier).Space();
            }

            c.TypeName(stmt.Name.ValueType).Space().Text(stmt.Name.Name).Text(";");
        }

        public void Visit(IMethodDeclaration stmt, SSTPrintingContext c)
        {
            c.Indentation();

            if (stmt.Name.IsStatic)
            {
                c.Text(c.Keywords.StaticModifier).Space();
            }

            c.TypeName(stmt.Name.ReturnType).Space().Text(stmt.Name.Name);

            c.ParameterList(stmt.Name.Parameters);

            c.StatementBlock(c, stmt.Body, this);
        }

        public void Visit(IPropertyDeclaration stmt, SSTPrintingContext c)
        {
            c.Indentation().TypeName(stmt.Name.ValueType).Space().Text(stmt.Name.Name);

            var hasBody = stmt.Get.Any() || stmt.Set.Any();

            if (hasBody) // Long version: At least one body exists --> line breaks + indentation
            {
                c.NewLine()
                 .Indentation();

                c.IndentationLevel++;
                  
                c.Text("{").NewLine();

                if (stmt.Name.HasGetter)
                {
                    AppendPropertyAccessor(c, stmt.Get, c.Keywords.PropertyGet);
                }

                if (stmt.Name.HasSetter)
                {
                    AppendPropertyAccessor(c, stmt.Set, c.Keywords.PropertySet);
                }

                c.IndentationLevel--;

                c.Indentation().Text("}");
            }
            else // Short Version: No bodies --> getter/setter declaration in same line
            {
                c.Text(" { ");
                if (stmt.Name.HasGetter)
                {
                    c.Text(c.Keywords.PropertyGet).Text(";").Space();
                }
                if (stmt.Name.HasSetter)
                {
                    c.Text(c.Keywords.PropertySet).Text(";").Space();
                }
                c.Text("}");
            }
        }

        private void AppendPropertyAccessor(SSTPrintingContext c, IKaVEList<IStatement> body, string keyword)
        {
            if (body.Any())
            {
                c.Indentation().Text(keyword);
                c.StatementBlock(c, body, this);
            }
            else
            {
                c.Indentation().Text(keyword).Text(";");
            }

            c.NewLine();
        }

        public void Visit(IVariableDeclaration stmt, SSTPrintingContext c)
        {
            c.Indentation().TypeName(stmt.Type).Space();
            stmt.Reference.Accept(this, c);
            c.Text(";");
        }

        public void Visit(IAssignment stmt, SSTPrintingContext c)
        {
            c.Indentation();
            stmt.Reference.Accept(this, c);
            c.Space().Text("=").Space();
            stmt.Expression.Accept(this, c);
            c.Text(";");
        }

        public void Visit(IBreakStatement stmt, SSTPrintingContext c)
        {
            c.Indentation().Text(c.Keywords.BreakStatement).Text(";");
        }

        public void Visit(IContinueStatement stmt, SSTPrintingContext c)
        {
            c.Indentation().Text(c.Keywords.ContinueStatement).Text(";");
        }

        public void Visit(IExpressionStatement stmt, SSTPrintingContext c)
        {
            c.Indentation();
            stmt.Expression.Accept(this, c);
            c.Text(";");
        }

        public void Visit(IGotoStatement stmt, SSTPrintingContext c)
        {
            c.Indentation().Text(c.Keywords.Goto).Space().Text(stmt.Label).Text(";");
        }

        public void Visit(ILabelledStatement stmt, SSTPrintingContext c)
        {
            c.Indentation().Text(stmt.Label).Text(":").NewLine();
            stmt.Statement.Accept(this, c);
        }

        public void Visit(IReturnStatement stmt, SSTPrintingContext c)
        {
            c.Indentation().Text(c.Keywords.Return);

            // note: UnknownExpression interpreted as void return
            if (!(stmt.Expression is IUnknownExpression))
            {
                c.Space();
                stmt.Expression.Accept(this, c);
            }

            c.Text(";");
        }

        public void Visit(IThrowStatement stmt, SSTPrintingContext c)
        {
            c.Indentation().Text(c.Keywords.Throw).Space().Text(c.Keywords.New).Space().Text(stmt.Exception.Name).Text("();");
        }

        public void Visit(IDoLoop block, SSTPrintingContext c)
        {
            c.Indentation().Text(c.Keywords.Do);

            c.StatementBlock(c, block.Body, this);

            c.NewLine().Indentation().Text(c.Keywords.While).Space().Text("(");
            c.IndentationLevel++;
            block.Condition.Accept(this, c);
            c.IndentationLevel--;
            c.NewLine().Indentation().Text(")");

        }

        public void Visit(IForEachLoop block, SSTPrintingContext c)
        {
            c.Indentation()
              .Text(c.Keywords.Foreach).Space().Text("(")
              .TypeName(block.Declaration.Type)
              .Space();
            block.Declaration.Reference.Accept(this, c);
            c.Space().Text(c.Keywords.ForeachIn).Space();
            block.LoopedReference.Accept(this, c);
            c.Text(")");

            c.StatementBlock(c, block.Body, this);
        }

        public void Visit(IForLoop block, SSTPrintingContext c)
        {
            c.Indentation().Text(c.Keywords.For).Space().Text("(");

            c.IndentationLevel++;

            c.StatementBlock(c, block.Init, this);
            c.Text(";");
            block.Condition.Accept(this, c);
            c.Text(";");
            c.StatementBlock(c, block.Step, this);

            c.IndentationLevel--;

            c.NewLine().Indentation().Text(")");

            c.StatementBlock(c, block.Body, this);
        }

        public void Visit(IIfElseBlock block, SSTPrintingContext c)
        {
            c.Indentation().Text(c.Keywords.If).Space().Text("(");
            block.Condition.Accept(this, c);
            c.Text(")");

            c.StatementBlock(c, block.Then, this);

            if (block.Else.Any())
            {
                c.NewLine().Indentation().Text(c.Keywords.Else);

                c.StatementBlock(c, block.Else, this);
            }
        }

        public void Visit(ILockBlock stmt, SSTPrintingContext c)
        {
            c.Indentation().Text(c.Keywords.Lock).Space().Text("(");
            stmt.Reference.Accept(this, c);
            c.Text(")");

            c.StatementBlock(c, stmt.Body, this);
        }

        public void Visit(ISwitchBlock block, SSTPrintingContext c)
        {
            c.Indentation().Text(c.Keywords.Switch).Space().Text("(");
            block.Reference.Accept(this, c);
            c.Text(")").NewLine().Indentation();
            c.IndentationLevel++;
            c.Text("{");

            foreach (var section in block.Sections)
            {
                c.NewLine().Indentation().Text(c.Keywords.SwitchCase).Space();
                section.Label.Accept(this, c);
                c.Text(":").StatementBlock(c, section.Body, this, false);
            }

            if (block.DefaultSection.Any())
            {
                c.NewLine().Indentation().Text(c.Keywords.SwitchDefault).Text(":")
                 .StatementBlock(c, block.DefaultSection, this, false);
            }

            c.NewLine();
            c.IndentationLevel--;
            c.Indentation().Text("}");
        }

        public void Visit(ITryBlock block, SSTPrintingContext c)
        {
            c.Indentation().Text(c.Keywords.Try)
             .StatementBlock(c, block.Body, this);

            foreach (var catchBlock in block.CatchBlocks)
            {
                c.NewLine().Indentation().Text(c.Keywords.TryCatch);

                if (!catchBlock.Exception.IsMissing)
                {
                    c.Space().Text("(")
                      .TypeName(catchBlock.Exception.Type)
                      .Space();
                    catchBlock.Exception.Reference.Accept(this, c);
                    c.Text(")");
                }

                c.StatementBlock(c, catchBlock.Body, this);
            }

            if (block.Finally.Any())
            {
                c.NewLine().Indentation().Text(c.Keywords.TryFinally)
                 .StatementBlock(c, block.Finally, this);
            }
        }

        public void Visit(IUncheckedBlock block, SSTPrintingContext c)
        {
            c.Indentation().Text(c.Keywords.Unchecked)
             .StatementBlock(c, block.Body, this);
        }

        public void Visit(IUnsafeBlock block, SSTPrintingContext c)
        {
            c.Indentation().Text(c.Keywords.UnsafeBlock);
        }

        public void Visit(IUsingBlock block, SSTPrintingContext c)
        {
            c.Indentation().Text(c.Keywords.Using).Space().Text("(");
            block.Reference.Accept(this, c);
            c.Text(")").StatementBlock(c, block.Body, this);
        }

        public void Visit(IWhileLoop block, SSTPrintingContext c)
        {
            c.Indentation().Text(c.Keywords.While).Space().Text("(");
            c.IndentationLevel++;
            block.Condition.Accept(this, c);
            c.IndentationLevel--;
            c.NewLine().Indentation().Text(")");

            c.StatementBlock(c, block.Body, this);
        }

        public void Visit(ICompletionExpression entity, SSTPrintingContext c)
        {
            if (entity.ObjectReference != null)
            {
                c.Text(entity.ObjectReference.Identifier).Text(".");
            }
            else if (entity.TypeReference != null)
            {
                c.TypeName(entity.TypeReference).Text(".");
            }

            c.Text(entity.Token).Text(c.Keywords.CursorPosition);
        }

        public void Visit(IComposedExpression expr, SSTPrintingContext c)
        {
            c.Text(c.Keywords.ComposedExpression).Text("(");

            foreach (var reference in expr.References)
            {
                reference.Accept(this, c);

                if (!ReferenceEquals(reference, expr.References.Last()))
                {
                    c.Text(", ");
                }
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
            expr.Reference.Accept(this, c);
            c.Text(".").Text(expr.MethodName.Name).Text("(");

            foreach (var parameter in expr.Parameters)
            {
                parameter.Accept(this, c);

                if (!ReferenceEquals(parameter, expr.Parameters.Last()))
                {
                    c.Text(", ");
                }
            }

            c.Text(")");
        }

        public void Visit(ILambdaExpression expr, SSTPrintingContext c)
        {
            c.ParameterList(expr.Name.Parameters).Space().Text("=>");
            c.StatementBlock(c, expr.Body, this);
        }

        // TODO: make this a bit nicer ...
        public void Visit(ILoopHeaderBlockExpression expr, SSTPrintingContext c)
        {
            c.StatementBlock(c, expr.Body, this);
        }

        public void Visit(IConstantValueExpression expr, SSTPrintingContext c)
        {
            string value = expr.Value ?? "null";

            double parsed;
            if (Double.TryParse(expr.Value, out parsed)
                || value.Equals("false", StringComparison.Ordinal)
                || value.Equals("true", StringComparison.Ordinal)
                || value.Equals("null", StringComparison.Ordinal))
            {
                c.Text(value);
            }
            else
            {
                c.Text("\"").Text(value).Text("\"");
            }
        }

        public void Visit(INullExpression expr, SSTPrintingContext c)
        {
            c.Text("null");
        }

        public void Visit(IReferenceExpression expr, SSTPrintingContext c)
        {
            expr.Reference.Accept(this, c);
        }

        public void Visit(IEventReference eventRef, SSTPrintingContext c)
        {
            c.Text(eventRef.Reference.Identifier);
        }

        public void Visit(IFieldReference fieldRef, SSTPrintingContext c)
        {
            c.Text(fieldRef.Reference.Identifier);
        }

        public void Visit(IMethodReference methodRef, SSTPrintingContext c)
        {
            c.Text(methodRef.Reference.Identifier);
        }

        public void Visit(IPropertyReference propertyRef, SSTPrintingContext c)
        {
            c.Text(propertyRef.Reference.Identifier);
        }

        public void Visit(IVariableReference varRef, SSTPrintingContext c)
        {
            c.Text(varRef.Identifier);
        }

        public void Visit(IUnknownReference unknownRef, SSTPrintingContext c)
        {
            c.Text(c.Keywords.UnknownEntity);
        }

        public void Visit(IUnknownExpression unknownExpr, SSTPrintingContext c)
        {
            c.Text(c.Keywords.UnknownEntity);
        }

        public void Visit(IUnknownStatement unknownStmt, SSTPrintingContext c)
        {
            c.Indentation().Text(c.Keywords.UnknownEntity).Text(";");
        }
    }
}