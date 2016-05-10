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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr4.Runtime;
using KaVE.Commons.Utils.Assertion;

namespace KaVE.Commons.Model.Names.CSharp.Parser
{
    class TypeNameParseUtil
    {
        private class MyErrorListener : IAntlrErrorListener<int>, IAntlrErrorListener<IToken>
        {
            public bool HasError;
            public void SyntaxError(IRecognizer recognizer,
                IToken offendingSymbol,
                int line,
                int charPositionInLine,
                string msg,
                RecognitionException e)
            {
                HasError = true;
            }

            public void SyntaxError(IRecognizer recognizer,
                int offendingSymbol,
                int line,
                int charPositionInLine,
                string msg,
                RecognitionException e)
            {
                HasError = true;
            }
        }


        public static TypeNamingParser.TypeContext ValidateTypeName(string input)
        {
            MyErrorListener el = new MyErrorListener();
            TypeNamingParser parser = SetupParser(input, el);
            var typeEOL = parser.typeEOL();
            if(el.HasError)
            {
                throw new AssertException("Wrong Syntax: " + input);
            }
            return typeEOL.type();
        }

        private static TypeNamingParser SetupParser(string input, MyErrorListener el)
        {
            var inputStream = new AntlrInputStream(input + "\n");
            var lexer = new TypeNamingLexer(inputStream);
            lexer.AddErrorListener(el);
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            TypeNamingParser parser = new TypeNamingParser(tokens);
            parser.AddErrorListener(el);
            return parser;
        }

    }
}
