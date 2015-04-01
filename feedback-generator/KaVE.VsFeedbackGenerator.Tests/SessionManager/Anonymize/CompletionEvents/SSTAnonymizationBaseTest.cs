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
 *    - Sebastian Proksch
 */

using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Declarations;
using KaVE.Commons.Model.SSTs.Expressions;
using KaVE.Commons.Model.SSTs.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Expressions.LoopHeader;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.LoopHeader;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.Model.SSTs.References;
using KaVE.VsFeedbackGenerator.SessionManager.Anonymize;
using KaVE.VsFeedbackGenerator.SessionManager.Anonymize.CompletionEvents;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.SessionManager.Anonymize.CompletionEvents
{
    public abstract class SSTAnonymizationBaseTest
    {
        protected IStatement AnyStatement { get; private set; }
        protected IStatement AnyStatementAnonymized { get; private set; }

        protected ISimpleExpression AnyExpression { get; private set; }
        protected ISimpleExpression AnyExpressionAnonymized { get; private set; }

        protected IVariableReference AnyVarReference { get; private set; }
        protected IVariableReference AnyVarReferenceAnonymized { get; private set; }

        protected IVariableDeclaration AnyVarDeclaration { get; private set; }
        protected IVariableDeclaration AnyVarDeclarationAnonymized { get; private set; }

        protected ILoopHeaderBlockExpression AnyLoopHeaderBlock { get; private set; }
        protected ILoopHeaderBlockExpression AnyLoopHeaderBlockAnonymized { get; private set; }
        protected ILambdaExpression AnyLambdaExpr { get; private set; }
        protected ILambdaExpression AnyLambdaExprAnonymized { get; private set; }

        protected SSTStatementAnonymization StatementAnonymizationMock { get; private set; }
        protected SSTExpressionAnonymization ExpressionAnonymizationMock { get; private set; }
        protected SSTReferenceAnonymization ReferenceAnonymizationMock { get; private set; }

        [SetUp]
        public void BaseSetup()
        {
            AnyVarReference = new VariableReference {Identifier = "i"};
            AnyVarReferenceAnonymized = new VariableReference {Identifier = "i".ToHash()};
            AnyVarDeclaration = new VariableDeclaration {Reference = AnyVarReference};
            AnyVarDeclarationAnonymized = new VariableDeclaration {Reference = AnyVarReferenceAnonymized};
            AnyExpression = new ReferenceExpression {Reference = AnyVarReference};
            AnyExpressionAnonymized = new ReferenceExpression {Reference = AnyVarReferenceAnonymized};
            AnyStatement = new ThrowStatement {Exception = Type("abc")};
            AnyStatementAnonymized = new ThrowStatement {Exception = TypeAnonymized("abc")};

            AnyLoopHeaderBlock = new LoopHeaderBlockExpression
            {
                Body = {AnyStatement}
            };
            AnyLoopHeaderBlockAnonymized = new LoopHeaderBlockExpression
            {
                Body = {AnyStatementAnonymized}
            };

            AnyLambdaExpr = new LambdaExpression
            {
                Parameters = {AnyVarDeclaration},
                Body = {AnyStatement}
            };
            AnyLambdaExprAnonymized = new LambdaExpression
            {
                Parameters = {AnyVarDeclarationAnonymized},
                Body = {AnyStatementAnonymized}
            };

            ReferenceAnonymizationMock = new SSTReferenceAnonymization();
            ExpressionAnonymizationMock = new SSTExpressionAnonymization(ReferenceAnonymizationMock);
            StatementAnonymizationMock = new SSTStatementAnonymization(
                ExpressionAnonymizationMock,
                ReferenceAnonymizationMock);
        }

        protected static ITypeName Type(string type)
        {
            return TypeName.Get(type + ", MyProject");
        }

        protected static ITypeName TypeAnonymized(string type)
        {
            return Type(type).ToAnonymousName();
        }

        protected IEventName Event(string name)
        {
            return EventName.Get(string.Format("[T1] [T2].{0}", name));
        }

        protected IEventName EventAnonymized(string name)
        {
            return Event(name).ToAnonymousName();
        }

        protected IFieldName Field(string name)
        {
            return FieldName.Get(string.Format("[T1] [T2].{0}", name));
        }

        protected IFieldName FieldAnonymized(string name)
        {
            return Field(name).ToAnonymousName();
        }

        protected IMethodName Method(string m)
        {
            return MethodName.Get(string.Format("[T1,P1] [T2,P2].{0}()", m));
        }

        protected IMethodName MethodAnonymized(string m)
        {
            return Method(m).ToAnonymousName();
        }

        protected IPropertyName Property(string name)
        {
            return PropertyName.Get(string.Format("[T1] [T2].{0}", name));
        }

        protected IPropertyName PropertyAnonymized(string name)
        {
            return Property(name).ToAnonymousName();
        }
    }
}