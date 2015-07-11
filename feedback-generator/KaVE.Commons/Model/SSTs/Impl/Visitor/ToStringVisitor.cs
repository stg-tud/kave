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

using System.Text;
using KaVE.Commons.Model.SSTs.Declarations;
using KaVE.Commons.Model.SSTs.References;

namespace KaVE.Commons.Model.SSTs.Impl.Visitor
{
    public class ToStringVisitor : AbstractNodeVisitor<StringBuilder>
    {
        public override void Visit(ISST sst, StringBuilder sb)
        {
            sb.Append("class ");
            sb.Append(sst.EnclosingType.Name);
            sb.Append(" {\n");

            foreach (var e in sst.Events)
            {
                e.Accept(this, sb);
            }
            foreach (var f in sst.Fields)
            {
                f.Accept(this, sb);
            }

            sb.Append("}\n");
        }

        public override void Visit(IDelegateDeclaration stmt, StringBuilder sb)
        {
            sb.Append(stmt.Name.DeclaringType.Name);
            sb.Append(" ");
            sb.Append(stmt.Name.Name);
            sb.Append(";\n");
        }

        public override void Visit(IEventDeclaration stmt, StringBuilder sb)
        {
            sb.Append("\tevent ");
            sb.Append(stmt.Name.HandlerType.Name);
            sb.Append(" ");
            sb.Append(stmt.Name.Name);
            sb.Append(";\n");
        }

        public override void Visit(IFieldDeclaration stmt, StringBuilder sb)
        {
            sb.Append("\t");
            sb.Append(stmt.Name.ValueType.Name);
            sb.Append(" ");
            sb.Append(stmt.Name.Name);
            sb.Append(";\n");
        }

        public override void Visit(IEventReference eventRef, StringBuilder sb)
        {
            sb.Append("TODO: IEventReference");
        }

        public override void Visit(IFieldReference fieldRef, StringBuilder sb)
        {
            sb.Append("TODO: IFieldReference");
        }

        public override void Visit(IMethodReference methodRef, StringBuilder sb)
        {
            sb.Append("TODO: IMethodReference");
        }

        public override void Visit(IPropertyReference propertyRef, StringBuilder sb)
        {
            sb.Append("TODO: IPropertyReference");
        }

        public override void Visit(IVariableReference varRef, StringBuilder sb)
        {
            sb.Append("TODO: IVariableReference");
        }
    }
}