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

        private readonly StringBuilder _sb;

        public SSTPrintingContext()
        {
            _sb = new StringBuilder();
        }

        public SSTPrintingContext Text(string text)
        {
            _sb.Append(text);
            return this;
        }

        public SSTPrintingContext NewLine()
        {
            _sb.AppendLine();
            return this;
        }

        public SSTPrintingContext Space()
        {
            _sb.Append(" ");
            return this;
        }

        public SSTPrintingContext Indentation()
        {
            for (int i = 0; i < IndentationLevel; i++)
            {
                _sb.Append("    ");
            }
            return this;
        }

        public SSTPrintingContext Keyword(string keyword)
        {
            return Text(keyword);
        }

        public SSTPrintingContext CursorPosition()
        {
            return Text("$");
        }

        public SSTPrintingContext UnknownMarker()
        {
            return Text("???");
        }

        public SSTPrintingContext TypeName(ITypeName typeName)
        {
            _sb.Append(typeName.Name);

            if (typeName.HasTypeParameters)
            {
                _sb.Append("<");

                foreach (var p in typeName.TypeParameters)
                {
                    if (!p.IsUnknownType)
                    {
                        TypeName(p);
                    }
                    else
                    {
                        _sb.Append(p.TypeParameterShortName);
                    }

                    if (!ReferenceEquals(p, typeName.TypeParameters.Last()))
                    {
                        _sb.Append(", ");
                    }
                }

                _sb.Append(">");
            }

            return this;
        }

        public SSTPrintingContext ParameterList(IList<IParameterName> parameters)
        {
            Text("(");

            foreach (var parameter in parameters)
            {
                if (parameter.IsPassedByReference && parameter.ValueType.IsValueType)
                {
                    Keyword("ref").Space();
                }

                if (parameter.IsOutput)
                {
                    Keyword("out").Space();
                }

                if (parameter.IsOptional)
                {
                    Keyword("opt").Space();
                }

                if (parameter.IsParameterArray)
                {
                    Keyword("params").Space();
                }

                TypeName(parameter.ValueType).Space().Text(parameter.Name);

                if (!ReferenceEquals(parameter, parameters.Last()))
                {
                    Text(",").Space();
                }
            }

            _sb.Append(")");

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
            return _sb.ToString();
        }
    }
}