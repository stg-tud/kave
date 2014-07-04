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
using KaVE.Model.Names;

namespace KaVE.Model.SSTs
{
    // ReSharper disable once InconsistentNaming
    public class SST
    {
        public void SetTriggerPoint(MethodDeclaration md)
        {
            //var i = 1 + 2;
            //var b = Console.Read();
            Console.Write("");
            Add((FieldDeclaration) null);
            throw new NotImplementedException();
        }

        public void Add(MethodDeclaration md)
        {
            throw new NotImplementedException();
        }

        public void Add(FieldDeclaration fd)
        {
            throw new NotImplementedException();
        }

        public void AddEntrypoint(MethodDeclaration mA)
        {
            throw new NotImplementedException();
        }

        public void Add(TypeTrigger md)
        {
            throw new NotImplementedException();
        }
    }


    public class Expression {}


    public class FieldDeclaration : Expression
    {
        public IFieldName Name;

        public string Identifier
        {
            get { return Name.Identifier; }
        }
    }

    public class VariableDeclaration : Expression
    {
        // var a = 1 --> var a; a = 1
        public string Identifier;
        public ITypeName Type;
    }

    public class PropertyDeclaration : Expression
    {
        public IPropertyName Name;

        public string Identifier
        {
            get { return Name.Identifier; }
        }

        public List<Expression> GetExpressions = new List<Expression>();
        public List<Expression> SetExpressions = new List<Expression>();
    }

    public class MethodDeclaration : Expression
    {
        public IMethodName Name;
        public List<Expression> Body = new List<Expression>();
    }

    public class LambdaDeclaration : Expression
    {
        public List<Expression> Body = new List<Expression>();
    }


    public class Block : Expression
    {
        public List<Expression> Expressions;
    }

    public class Assignment : Expression
    {
        public string Identifier;
        public Expression Value;
    }

    public class Invocation : Expression
    {
        public string Identifier;
        public IMethodName Name;
        public string[] Parameters;
    }

    public class ConstantExpression : Expression {}

    public class ComposedExpression : Expression
    {
        public string[] Variables;
    }

    public class ThrowStatement : Expression
    {
        public ITypeName Exception;
    }


    public class TypeTrigger : Expression
    {
        public string Token = "";
    }

    public class MethodTrigger : Expression
    {
        public string Token = "";
    }


    public class IfElse : Expression
    {
        public Expression Condition;
        public List<Expression> IfExpressions = new List<Expression>();
        public List<Expression> ElseExpressions = new List<Expression>();
    }

    public class WhileLoop : Expression
    {
        public Expression Condition;
        public List<Expression> Expressions = new List<Expression>();
    }

    public class ForLoop : Expression {}

    public class ForEachLoop : Expression {}

    public class TryCatch : Expression
    {
        public List<Expression> TryBlock = new List<Expression>();
        public Dictionary<ITypeName, List<Expression>> CatchBlocks = new Dictionary<ITypeName, List<Expression>>();
    }

    // ... whatever is missing (lock, using, etc..)
}