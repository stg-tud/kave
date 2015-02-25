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

using KaVE.Model.SSTs.Declarations;
using KaVE.Model.SSTs.Impl;
using KaVE.Model.SSTs.Impl.Declarations;

namespace KaVE.Model.SSTs.Visitor
{
    public interface ISSTNodeVisitor<in TContext>
    {
        void Visit(ISST sst, TContext context);

        void Visit(IDelegateDeclaration stmt, TContext context);
        void Visit(IEventDeclaration stmt, TContext context);
        void Visit(IFieldDeclaration stmt, TContext context);
        void Visit(IMethodDeclaration stmt, TContext context);
        void Visit(IPropertyDeclaration stmt, TContext context);

        void Visit(IStatement stmt, TContext context);
        void Visit(IExpression expr, TContext context);
    }
}