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
 *    - Andreas Bauer
 */

using System.Collections.Generic;
using System.Linq;
using System.Text;
using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Visitor;
using KaVE.Commons.Utils.Collections;

namespace KaVE.Commons.Utils.SSTPrinter
{
    public class SSTPrintingContext
    {
        public int IndentationLevel { get; set; }
        public ISSTPrintingKeywords Keywords { get; private set; }

        private readonly StringBuilder sb;

        public SSTPrintingContext() : this(new DefaultKeywords()) {}

        public SSTPrintingContext(ISSTPrintingKeywords keywords)
        {
            this.sb = new StringBuilder();
            this.Keywords = keywords;
        }

        public SSTPrintingContext Text(string text)
        {
            sb.Append(text);
            return this;
        }

        public SSTPrintingContext NewLine()
        {
            sb.AppendLine();
            return this;
        }

        public SSTPrintingContext Space()
        {
            sb.Append(" ");
            return this;
        }

        public SSTPrintingContext Indentation()
        {
            for (int i = 0; i < IndentationLevel; i++)
            {
                sb.Append(this.Keywords.IndentationToken);
            }
            return this;
        }

        public SSTPrintingContext TypeName(ITypeName typeName)
        {
            sb.Append(typeName.Name);

            if (typeName.HasTypeParameters)
            {
                sb.Append("<");

                foreach (var p in typeName.TypeParameters)
                {
                    if (!p.IsUnknownType)
                    {
                        this.TypeName(p);
                    }
                    else
                    {
                        sb.Append(p.TypeParameterShortName);
                    }

                    if (!ReferenceEquals(p, typeName.TypeParameters.Last()))
                    {
                        sb.Append(", ");
                    }
                }

                sb.Append(">");
            }

            return this;
        }

        public SSTPrintingContext ParameterList(IList<IParameterName> parameters)
        {
            this.Text("(");

            foreach (var parameter in parameters)
            {
                if (parameter.IsPassedByReference && parameter.ValueType.IsValueType)
                {
                    this.Text(Keywords.RefModifier).Space();
                }

                if (parameter.IsOutput)
                {
                    this.Text(Keywords.OutModifier).Space();
                }

                if (parameter.IsOptional)
                {
                    this.Text(Keywords.OptModifier).Space();
                }

                if (parameter.IsParameterArray)
                {
                    this.Text(Keywords.ParamsModifier).Space();
                }

                this.TypeName(parameter.ValueType).Space().Text(parameter.Name);

                if (!ReferenceEquals(parameter, parameters.Last()))
                {
                    this.Text(",").Space();
                }
            }

            sb.Append(")");

            return this;
        }

        // TODO: write tests
        /// <summary>
        ///     Appends the print result of a statement block to a string builder with correct indentation.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="block"></param>
        /// <param name="visitor"></param>
        /// <param name="withBrackets">If false, opening and closing brackets will be omitted.</param>
        internal void StatementBlock(SSTPrintingContext c,
            IKaVEList<IStatement> block,
            ISSTNodeVisitor<SSTPrintingContext> visitor,
            bool withBrackets = true)
        {
            if (!block.Any())
            {
                if (withBrackets)
                {
                    c.Text(" { }");
                }

                return;
            }

            if (withBrackets)
            {
                c.NewLine().Indentation().Text("{");
            }

            c.IndentationLevel++;

            foreach (var statement in block)
            {
                c.NewLine();
                statement.Accept(visitor, c);
            }

            c.IndentationLevel--;

            if (withBrackets)
            {
                c.NewLine().Indentation().Text("}");
            }
        }

        public override string ToString()
        {
            return sb.ToString();
        }
    }
}