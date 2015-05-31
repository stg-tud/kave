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
 *    - 
 */

using System.Collections.Generic;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Expressions;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.VsFeedbackGenerator.Utils.Names;

namespace KaVE.VsFeedbackGenerator.Analysis.Transformer
{
    public class ToArgumentRef : TreeNodeVisitor<IList<IStatement>, ISimpleExpression>
    {
        public override ISimpleExpression VisitInvocationExpression(IInvocationExpression expr, IList<IStatement> body)
        {
            var invoked = expr.InvokedExpression as IReferenceExpression;

            if (invoked != null && expr.Reference != null)
            {
                var returnType = TypeName.UnknownName;
                var resolvedMethod = expr.Reference.ResolveMethod();
                if (resolvedMethod != null)
                {
                    var methodName = resolvedMethod.GetName<IMethodName>();
                    returnType = methodName.ReturnType;
                }

                var varName = "%UNKNOWN_VAR_NAME%";
                var qualifier = invoked.QualifierExpression as IReferenceExpression;
                if (qualifier != null)
                {
                    varName = qualifier.NameIdentifier.Name;
                }

                body.Add(
                    new VariableDeclaration
                    {
                        Type = returnType,
                        Reference = new VariableReference {Identifier = varName}
                    });
                body.Add(
                    new Assignment
                    {
                        Reference = new VariableReference {Identifier = varName},
                        Expression = new InvocationExpression()
                    });
                return RefExpr(varName);
            }
            return RefExpr("%ERROR%");
        }

        public override ISimpleExpression VisitReferenceExpression(IReferenceExpression expr, IList<IStatement> body)
        {
            return RefExpr(expr.NameIdentifier.Name);
        }

        private static ReferenceExpression RefExpr(string varRefId)
        {
            return new ReferenceExpression
            {
                Reference = new VariableReference
                {
                    Identifier = varRefId
                }
            };
        }

        public override ISimpleExpression VisitThisExpression(IThisExpression thisExpressionParam,
            IList<IStatement> context)
        {
            return RefExpr("this");
        }
    }
}