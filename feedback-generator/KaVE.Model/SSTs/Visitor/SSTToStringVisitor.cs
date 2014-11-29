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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KaVE.Model.SSTs.Declarations;

namespace KaVE.Model.SSTs.Visitor
{
    public class SSTToStringVisitor : SSTNodeVisitor<StringBuilder>
    {
        public override void Visit(SST sst, StringBuilder context)
        {
            context.Append("class ");
            context.Append(sst.EnclosingType.Name);
            context.Append(" {\n");
                
            foreach (var e in sst.Events)
            {
                e.Accept(this, context);
            }
            foreach (var f in sst.Fields)
            {
                f.Accept(this, context);
            }

            context.Append("}\n");
        }

        public override void Visit(EventDeclaration stmt, StringBuilder context)
        {
            context.Append("\tevent ");
            context.Append(stmt.Name.HandlerType.Name);
            context.Append(" ");
            context.Append(stmt.Name.Name);
            context.Append(";\n");
        }

        public override void Visit(FieldDeclaration stmt, StringBuilder context)
        {
            context.Append("\t");
            context.Append(stmt.Name.ValueType.Name);
            context.Append(" ");
            context.Append(stmt.Name.Name);
            context.Append(";\n");
        }
    }
}
