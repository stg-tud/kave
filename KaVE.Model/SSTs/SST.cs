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
using KaVE.Utils;

namespace KaVE.Model.SSTs
{
    public class SST
    {
        private readonly ISet<MethodDeclaration> _methods = new HashSet<MethodDeclaration>();
        private readonly ISet<MethodDeclaration> _eps = new HashSet<MethodDeclaration>();

        public void SetTriggerPoint(MethodDeclaration md)
        {
            //var i = 1 + 2;
            //var b = Console.Read();
            Console.Write("");
            Add((FieldDeclaration) null);
        }

        public void Add(MethodDeclaration md)
        {
            _methods.Add(md);
        }

        public void AddEntrypoint(MethodDeclaration mA)
        {
            _eps.Add(mA);
        }

        public void Add(FieldDeclaration fd) {}

        public void Add(TypeTrigger md) {}

        public ISet<MethodDeclaration> GetEntrypoints()
        {
            return _eps;
        }

        public ISet<MethodDeclaration> GetNonEntrypoints()
        {
            return _methods;
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
        public readonly string Identifier;
        public readonly ITypeName Type;

        public VariableDeclaration(string identifier, ITypeName type)
        {
            Identifier = identifier;
            Type = type;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        protected bool Equals(VariableDeclaration other)
        {
            return string.Equals(Identifier, other.Identifier) && Equals(Type, other.Type);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Identifier != null ? Identifier.GetHashCode() : 0)*397) ^
                       (Type != null ? Type.GetHashCode() : 0);
            }
        }
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