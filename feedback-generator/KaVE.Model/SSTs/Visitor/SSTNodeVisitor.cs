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

namespace KaVE.Model.SSTs.Visitor
{
    // TODO implement "navigation" in SST structure in this class?!
    public class SSTNodeVisitor<TContext> : ISSTNodeVisitor<TContext>
    {
        public virtual void Visit(SST sst, TContext context) {}

        public virtual void Visit(DelegateDeclaration stmt, TContext context) {}
        public virtual void Visit(EventDeclaration stmt, TContext context) {}
        public virtual void Visit(FieldDeclaration stmt, TContext context) {}
        public virtual void Visit(MethodDeclaration stmt, TContext context) {}
        public virtual void Visit(PropertyDeclaration stmt, TContext context) {}

        public virtual void Visit(Statement stmt, TContext context) {}
        public virtual void Visit(Expression expr, TContext context) {}
    }
}