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
 *    - Dennis Albrecht
 */

using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.Util;
using KaVE.Model.SSTs.Declarations;
using KaVE.Model.SSTs.Expressions;
using KaVE.Model.SSTs.Statements;
using KaVE.VsFeedbackGenerator.Utils.Names;

namespace KaVE.VsFeedbackGenerator.Analysis
{
    public class SSTArgumentCollector : BaseSSTTransformer
    {
        private readonly IList<string> _arguments = new List<string>();

        public SSTArgumentCollector(MethodDeclaration declaration) : base(declaration) {}

        public string[] Arguments
        {
            get { return Enumerable.ToArray(_arguments); }
        }

        public override void VisitArgumentList(IArgumentList argumentListParam)
        {
            argumentListParam.Arguments.ForEach(argument => argument.Accept(this));
        }

        public override void VisitCSharpArgument(ICSharpArgument cSharpArgumentParam)
        {
            cSharpArgumentParam.Value.Accept(this);
        }

        public override void VisitReferenceExpression(IReferenceExpression referenceExpressionParam)
        {
            _arguments.Add(referenceExpressionParam.NameIdentifier.Name);
        }

        public override void VisitCSharpLiteralExpression(ICSharpLiteralExpression cSharpLiteralExpressionParam)
        {
            var tmp = Declaration.GetNewTempVariable();
            Declaration.Body.Add(new VariableDeclaration(tmp, cSharpLiteralExpressionParam.Type().GetName()));
            Declaration.Body.Add(new Assignment(tmp, new ConstantExpression()));
            _arguments.Add(tmp);
        }

        public override void VisitInvocationExpression(IInvocationExpression invocationExpressionParam)
        {
            HandleInvocationExpression(
                invocationExpressionParam,
                (declaration, callee, method, args, retType) =>
                {
                    var tmp = declaration.GetNewTempVariable();
                    declaration.Body.Add(new VariableDeclaration(tmp, retType));
                    declaration.Body.Add(new Assignment(tmp, new InvocationExpression(callee, method, args)));
                    _arguments.Add(tmp);
                });
        }

        public override void VisitThisExpression(IThisExpression thisExpressionParam)
        {
            _arguments.Add("this");
        }
    }
}