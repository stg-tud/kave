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
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using KaVE.ReSharper.Commons.Analysis.Util;

namespace KaVE.ReSharper.Commons.Analysis.Transformer
{
    public class ToAssignableReference : TreeNodeVisitor<IList<IStatement>, IAssignableReference>
    {
        public ToAssignableReference(UniqueVariableNameGenerator varNameGenerator) {}

        public override IAssignableReference VisitReferenceExpression(IReferenceExpression reference,
            IList<IStatement> body)
        {
            if (reference.QualifierExpression == null)
            {
                return FindLocalVariable(reference);
            }

            return FindFieldReference(reference);
        }

        private static IAssignableReference FindLocalVariable(IReferenceExpression reference)
        {
            var declaredElement = Resolve(reference.Reference);

            if (declaredElement != null)
            {
                return new VariableReference
                {
                    Identifier = declaredElement.ShortName
                };
            }
            return new VariableReference();
        }

        private static FieldReference FindFieldReference(IReferenceExpression reference)
        {
            return new FieldReference();
        }

        private static IDeclaredElement Resolve(IReferenceExpressionReference reference)
        {
            var resolvedRef = reference.Resolve();
            var isResolved = resolvedRef.ResolveErrorType == ResolveErrorType.OK;
            return isResolved ? resolvedRef.DeclaredElement : null;
        }
    }
}