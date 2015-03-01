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

using System.Linq;
using KaVE.Model.Collections;
using KaVE.Model.Names;
using KaVE.Model.SSTs.Expressions;
using KaVE.Model.SSTs.Expressions.Assignable;
using KaVE.Model.SSTs.Expressions.Simple;
using KaVE.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Model.SSTs.Impl.References;
using KaVE.Model.SSTs.References;
using KaVE.Utils.Assertion;

namespace KaVE.Model.SSTs
{
    public class SSTUtil
    {
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

        // TODO use interface
        public static InvocationExpression InvocationExpression(IMethodName name, params ISimpleExpression[] parameters)
        {
            Asserts.That(name.IsStatic || name.IsConstructor);
            return new InvocationExpression
            {
                Name = name,
                Parameters = Lists.NewListFrom(parameters),
            };
        }

        // TODO use interface
        public static InvocationExpression InvocationExpression(string id,
            IMethodName name,
            params ISimpleExpression[] parameters)
        {
            Asserts.Not(name.IsStatic || name.IsConstructor);
            return new InvocationExpression
            {
                Reference = new VariableReference {Identifier = id},
                Name = name,
                Parameters = Lists.NewListFrom(parameters),
            };
        }
    }
}