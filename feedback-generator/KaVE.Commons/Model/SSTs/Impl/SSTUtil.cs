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
using System.Linq;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Model.SSTs.Blocks;
using KaVE.Commons.Model.SSTs.Expressions;
using KaVE.Commons.Model.SSTs.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.Blocks;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.Model.SSTs.References;
using KaVE.Commons.Model.SSTs.Statements;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.SSTs.Impl
{
    // TODO: get rid of this obsolete class
    public class SSTUtil
    {
        public static IVariableDeclaration Declare(string identifier, ITypeName type)
        {
            return new VariableDeclaration
            {
                Reference = VariableReference(identifier),
                Type = type
            };
        }

        public static IReferenceExpression ReferenceExprToVariable(string id)
        {
            var variableReference = new VariableReference {Identifier = id};
            return new ReferenceExpression {Reference = variableReference};
        }

        public static IVariableReference VariableReference(string id)
        {
            return new VariableReference {Identifier = id};
        }

        public static IComposedExpression ComposedExpression(params string[] strReferences)
        {
            var varRefs = strReferences.ToList().Select(VariableReference);
            var refs = Lists.NewListFrom(varRefs);
            return new ComposedExpression {References = refs};
        }

        public static IAssignment AssignmentToLocal([NotNull] string identifier, [NotNull] IAssignableExpression expr)
        {
            return new Assignment
            {
                Reference = VariableReference(identifier),
                Expression = expr
            };
        }

        public static IExpressionStatement InvocationStatement(IMethodName name,
            IEnumerable<ISimpleExpression> parameters)
        {
            return new ExpressionStatement
            {
                Expression = InvocationExpression(name, parameters)
            };
        }

        public static IExpressionStatement InvocationStatement(string id,
            IMethodName name)
        {
            return InvocationStatement(id, name, new ISimpleExpression[] {});
        }

        public static IExpressionStatement InvocationStatement(string id,
            IMethodName name,
            IEnumerable<ISimpleExpression> parameters)
        {
            return new ExpressionStatement
            {
                Expression = InvocationExpression(id, name, parameters)
            };
        }

        public static IInvocationExpression InvocationExpression(string id, IMethodName name)
        {
            return InvocationExpression(id, name, new ISimpleExpression[] {});
        }

        public static IInvocationExpression InvocationExpression(IMethodName name,
            IEnumerable<ISimpleExpression> parameters)
        {
            Asserts.That(name.IsStatic || name.IsConstructor);
            return new InvocationExpression
            {
                MethodName = name,
                Parameters = Lists.NewListFrom(parameters)
            };
        }

        public static IInvocationExpression InvocationExpression(string id,
            IMethodName name,
            IEnumerable<ISimpleExpression> parameters)
        {
            Asserts.Not(name.IsStatic || name.IsConstructor);
            return new InvocationExpression
            {
                Reference = new VariableReference {Identifier = id},
                MethodName = name,
                Parameters = Lists.NewListFrom(parameters)
            };
        }

        public static ILockBlock LockBlock(string id)
        {
            return new LockBlock {Reference = new VariableReference {Identifier = id}};
        }

        public static IStatement Return(ISimpleExpression expression)
        {
            return new ReturnStatement
            {
                Expression = expression
            };
        }

        public static IStatement ReturnVariable(string id)
        {
            return Return(ReferenceExprToVariable(id));
        }
    }
}