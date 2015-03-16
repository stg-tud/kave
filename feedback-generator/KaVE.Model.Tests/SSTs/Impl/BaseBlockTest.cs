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

using System.Collections.Generic;
using System.Linq;
using KaVE.Model.Collections;
using KaVE.Model.Names;
using KaVE.Model.Names.CSharp;
using KaVE.Model.SSTs.Declarations;
using KaVE.Model.SSTs.Expressions;
using KaVE.Model.SSTs.Impl.Declarations;
using KaVE.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Model.SSTs.Impl.References;
using KaVE.Model.SSTs.References;

namespace KaVE.Model.Tests.SSTs.Impl
{
    internal abstract class SSTBaseTest
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

        protected IList<IVariableReference> Refs(params string[] strRefs)
        {
            var refs = strRefs.ToList().Select(SomeVarRef);
            return Lists.NewListFrom(refs);
        }

        protected IMethodName GetMethod(string simpleName)
        {
            var methodName = string.Format("[T1, P1] [T2, P2].{0}()", simpleName);
            return MethodName.Get(methodName);
        }

        protected ISimpleExpression[] RefExprs(params string[] ids)
        {
            return
                ids.Select<string, ISimpleExpression>(
                    id => new ReferenceExpression {Reference = new VariableReference {Identifier = id}}).ToArray();
        }
    }
}