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

namespace KaVE.VsFeedbackGenerator.Analysis
{
    public class SSTReferenceCollector : BaseSSTTransformer
    {
        private readonly IList<string> _references = new List<string>();

        public SSTReferenceCollector(MethodDeclaration declaration) : base(declaration) {}

        public string[] References
        {
            get { return Enumerable.ToArray(_references); }
        }

        public override void VisitBinaryExpression(IBinaryExpression binaryExpressionParam)
        {
            binaryExpressionParam.LeftOperand.Accept(this);
            binaryExpressionParam.RightOperand.Accept(this);
        }

        public override void VisitReferenceExpression(IReferenceExpression referenceExpressionParam)
        {
            _references.Add(referenceExpressionParam.NameIdentifier.Name);
        }

        public override void VisitArrayInitializer(IArrayInitializer arrayInitializerParam)
        {
            arrayInitializerParam.ElementInitializers.ForEach(i => i.Accept(this));
        }

        public override void VisitExpressionInitializer(IExpressionInitializer expressionInitializerParam)
        {
            expressionInitializerParam.Value.Accept(this);
        }
    }
}