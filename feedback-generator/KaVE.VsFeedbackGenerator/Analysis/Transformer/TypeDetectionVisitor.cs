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

using System;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Utils.Assertion;
using KaVE.VsFeedbackGenerator.Utils.Names;

namespace KaVE.VsFeedbackGenerator.Analysis.Transformer
{
    public class TypeDetectionVisitor : TreeNodeVisitor<object, ITypeName>
    {
        public override ITypeName VisitNode(ITreeNode node, object context)
        {
            foreach (var child in node.Children<ICSharpTreeNode>())
            {
                try
                {
                    var res = child.Accept(this, context);
                    if (res != null)
                    {
                        return res;
                    }
                }
                catch (NullReferenceException) {}
                catch (AssertException) {}
            }
            return TypeName.UnknownName;
        }

        public override ITypeName VisitReferenceExpression(IReferenceExpression expr, object context)
        {
            if (expr.QualifierExpression != null)
            {
                var expressionType = expr.QualifierExpression.GetExpressionType();
                return expressionType.ToIType().GetName();
            }

            return TypeName.UnknownName;
        }
    }
}