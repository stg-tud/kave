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

using KaVE.Model.Names.CSharp;
using KaVE.Model.SSTs.Declarations;
using KaVE.Model.SSTs.Expressions;
using KaVE.Model.SSTs.Impl.Declarations;
using KaVE.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Model.SSTs.Impl.References;
using KaVE.Model.SSTs.References;

namespace KaVE.Model.Tests.SSTs.Impl.Blocks
{
    internal abstract class BaseBlockTest
    {
        protected ISimpleExpression Label(string label)
        {
            return new ConstantValueExpression {Value = label};
        }

        protected IVariableDeclaration SomeDeclaration(string type = "T1")
        {
            return new VariableDeclaration {Type = TypeName.Get(type + ",P1")};
        }

        protected IVariableReference SomeVarRef(string id = "v")
        {
            return new VariableReference {Identifier = id};
        }
    }
}