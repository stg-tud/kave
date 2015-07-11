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

using System;
using KaVE.Commons.Model.Names;

namespace KaVE.Commons.Utils.SSTPrinter
{
    public class XamlSSTPrintingContext : SSTPrintingContext
    {
        public override SSTPrintingContext LeftAngleBracket()
        {
            return Text("&lt;");
        }

        public override SSTPrintingContext RightAngleBracket()
        {
            return Text("&gt;");
        }

        public override SSTPrintingContext Keyword(string keyword)
        {
            Text("<Span Foreground=\"Blue\">");
            base.Keyword(keyword);
            Text("</Span>");
            return this;
        }

        public override SSTPrintingContext UnknownMarker()
        {
            Text("<Span Foreground=\"Blue\">");
            base.UnknownMarker();
            Text("</Span>");
            return this;
        }

        public override SSTPrintingContext CursorPosition()
        {
            Text("<Bold>");
            base.CursorPosition();
            Text("</Bold>");
            return this;
        }

        public override SSTPrintingContext TypeNameOnly(ITypeName typeName)
        {
            var color = "#2B91AF";

            if (typeName.IsInterfaceType)
            {
                color = "#4EC9B0";
            }
            else if (typeName.IsValueType)
            {
                color = "Blue";
            }

            Text(String.Format("<Span Foreground=\"{0}\">", color));
            base.TypeNameOnly(typeName);
            Text("</Span>");
            return this;
        }

        protected override SSTPrintingContext TypeParameterShortName(string typeParameterShortName)
        {
            Text("<Bold>");
            base.TypeParameterShortName(typeParameterShortName);
            Text("</Bold>");
            return this;
        }

        public override SSTPrintingContext StringLiteral(string value)
        {
            Text("<Span Foreground=\"#A31515\">");
            base.StringLiteral(value);
            Text("</Span>");
            return this;
        }

        public override SSTPrintingContext Comment(string commentText)
        {
            Text("<Span Foreground=\"#8F8F8F\">");
            base.Comment(commentText);
            Text("</Span>");
            return this;
        }
    }
}