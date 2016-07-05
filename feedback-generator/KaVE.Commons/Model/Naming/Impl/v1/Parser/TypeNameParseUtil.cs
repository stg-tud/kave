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

using Antlr4.Runtime;
using KaVE.Commons.Utils.Assertion;

namespace KaVE.Commons.Model.Naming.Impl.v1.Parser
{
    public class TypeNameParseUtil
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
            Asserts.Not(el.HasError, "Syntax Error: " + input);
            return typeEOL.type();
        }

        public static TypeNamingParser.MethodContext ValidateMethodName(string input)
        {
            MyErrorListener el = new MyErrorListener();
            TypeNamingParser parser = SetupParser(input, el);
            var methodEol= parser.methodEOL();
            Asserts.Not(el.HasError, "Syntax Error: " + input);
            return methodEol.method();
        }

        private static TypeNamingParser SetupParser(string input, MyErrorListener el)
        {
            var inputStream = new AntlrInputStream(input + "\n");
            var lexer = new TypeNamingLexer(inputStream);
            lexer.RemoveErrorListeners();
            lexer.AddErrorListener(el);
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            TypeNamingParser parser = new TypeNamingParser(tokens);
            parser.RemoveErrorListeners();
            parser.AddErrorListener(el);
            return parser;
        }

        public static TypeNamingParser.NamespaceContext ValidateNamespaceName(string input)
        {
            MyErrorListener el = new MyErrorListener();
            TypeNamingParser parser = SetupParser(input, el);
            var namespaceEol = parser.namespaceEOL();
            Asserts.Not(el.HasError, "Syntax Error: " + input);
            return namespaceEol.@namespace();
        }

        public static TypeNamingParser.AssemblyContext ValidateAssemblyName(string input)
        {
            MyErrorListener el = new MyErrorListener();
            TypeNamingParser parser = SetupParser(input, el);
            var namespaceEol = parser.assemblyEOL();
            Asserts.Not(el.HasError, "Syntax Error: " + input);
            return namespaceEol.assembly();
        }

        public static TypeNamingParser.FormalParamContext ValidateParameterName(string input)
        {
            MyErrorListener el = new MyErrorListener();
            TypeNamingParser parser = SetupParser(input, el);
            var parameterEol = parser.parameterNameEOL();
            Asserts.Not(el.HasError, "Syntax Error: " + input);
            return parameterEol.formalParam();
        }

        public static TypeNamingParser.MemberNameContext ValidateMemberName(string input)
        {
            MyErrorListener el = new MyErrorListener();
            TypeNamingParser parser = SetupParser(input, el);
            var memberNameEol = parser.memberNameEOL();
            Asserts.Not(el.HasError, "Syntax Error: " + input);
            return memberNameEol.memberName();
        }

        public static TypeNamingParser.LambdaNameContext ValidateLambdaName(string input)
        {
            MyErrorListener el = new MyErrorListener();
            TypeNamingParser parser = SetupParser(input, el);
            var lambdaNameEol = parser.lambdaNameEOL();
            Asserts.Not(el.HasError, "Syntax Error: " + input);
            return lambdaNameEol.lambdaName();
        }
    }
}
