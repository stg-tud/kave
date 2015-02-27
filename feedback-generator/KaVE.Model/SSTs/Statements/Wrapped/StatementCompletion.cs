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

using KaVE.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Model.SSTs.Impl.References;

namespace KaVE.Model.SSTs.Statements.Wrapped
{
    public class StatementCompletion : ExpressionStatement
    {
        public ExpressionCompletion Completion
        {
            get { return Target as ExpressionCompletion; }
        }

        public static StatementCompletion Create(string token)
        {
            return new StatementCompletion {Target = new ExpressionCompletion {Token = token}};
        }

        public static StatementCompletion Create(string identifier, string token)
        {
            var vref = new VariableReference {Identifier = identifier};
            return new StatementCompletion {Target = new ExpressionCompletion {ObjectReference = vref, Token = token}};
        }
    }
}