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
using KaVE.Commons.Model.Naming.Types;

namespace KaVE.Commons.Utils.SSTPrinter
{
    public class XamlSSTPrintingContext : SSTPrintingContext
    {
        public override SSTPrintingContext Text(string text)
        {
            return base.Text(text.EncodeSpecialChars());
        }

        public override SSTPrintingContext LeftAngleBracket()
        {
            return RawText("&lt;");
        }

        public override SSTPrintingContext RightAngleBracket()
        {
            return RawText("&gt;");
        }

        public override SSTPrintingContext Keyword(string keyword)
        {
            RawText("<Span Foreground=\"Blue\">");
            base.Keyword(keyword);
            RawText("</Span>");
            return this;
        }

        public override SSTPrintingContext UnknownMarker()
        {
            RawText("<Span Foreground=\"Blue\">");
            base.UnknownMarker();
            RawText("</Span>");
            return this;
        }

        public override SSTPrintingContext CursorPosition()
        {
            RawText("<Bold>");
            base.CursorPosition();
            RawText("</Bold>");
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

            RawText(String.Format("<Span Foreground=\"{0}\">", color));
            base.TypeNameOnly(typeName);
            RawText("</Span>");
            return this;
        }

        protected override SSTPrintingContext TypeParameterShortName(string typeParameterShortName)
        {
            RawText("<Bold>");
            base.TypeParameterShortName(typeParameterShortName);
            RawText("</Bold>");
            return this;
        }

        public override SSTPrintingContext StringLiteral(string value)
        {
            RawText("<Span Foreground=\"#A31515\">");
            base.StringLiteral(value);
            RawText("</Span>");
            return this;
        }

        public override SSTPrintingContext Comment(string commentText)
        {
            RawText("<Span Foreground=\"#8F8F8F\">");
            base.Comment(commentText);
            RawText("</Span>");
            return this;
        }
    }
}