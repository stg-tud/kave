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
using KaVE.Model.Collections;
using KaVE.Model.Names;
using KaVE.Model.Names.CSharp;
using KaVE.Utils;

namespace KaVE.Model.SSTs
{
    public class SST
    {
        private ITypeName _td;

        private readonly ISet<FieldDeclaration> _fields = Sets.NewHashSet<FieldDeclaration>();
        private readonly ISet<PropertyDeclaration> _properties = Sets.NewHashSet<PropertyDeclaration>();
        private readonly ISet<MethodDeclaration> _methods = Sets.NewHashSet<MethodDeclaration>();
        private readonly ISet<MethodDeclaration> _eps = Sets.NewHashSet<MethodDeclaration>();
        private readonly ISet<EventDeclaration> _events = Sets.NewHashSet<EventDeclaration>();
        private readonly ISet<DelegateDeclaration> _delegates = Sets.NewHashSet<DelegateDeclaration>();

        public SST(ITypeName td)
        {
            _td = td;
        }

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

        public void Add(FieldDeclaration fd)
        {
            _fields.Add(fd);
        }

        public void Add(TypeTrigger md) {}

        public ISet<MethodDeclaration> GetEntrypoints()
        {
            return _eps;
        }

        public ISet<MethodDeclaration> GetNonEntrypoints()
        {
            return _methods;
        }

        public void Add(PropertyDeclaration pd)
        {
            _properties.Add(pd);
        }

        public void Add(DelegateDeclaration dd)
        {
            _delegates.Add(dd);
        }

        public void Add(EventDeclaration ed)
        {
            _events.Add(ed);
        }
    }


    public class Expression {}


    public class FieldDeclaration : Expression
    {
        public string Name;
        public ITypeName Type;

        public FieldDeclaration(string s, ITypeName typeName)
        {
            Name = s;
            Type = typeName;
        }

        public string Identifier
        {
            get { return Name; }
        }
    }

    public class TypeDeclaration : Expression
    {
        public ITypeName Type { get; set; }

        public TypeDeclaration(ITypeName type)
        {
            Type = type;
        }
    }

    public class DelegateDeclaration : Expression
    {
        public string Name { get; set; }
        public MethodName MethodName { get; set; }

        public DelegateDeclaration(string name, MethodName methodName)
        {
            Name = name;
            MethodName = methodName;
        }
    }

    public class EventDeclaration : Expression
    {
        public string Identifier { get; set; }
        public ITypeName Type { get; set; }

        public EventDeclaration(string identifier, ITypeName type)
        {
            Identifier = identifier;
            Type = type;
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
        public string Name { get; set; }
        public ITypeName Type { get; set; }

        public List<Expression> GetExpressions = new List<Expression>();
        public List<Expression> SetExpressions = new List<Expression>();

        public PropertyDeclaration(string s, ITypeName typeName)
        {
            Name = s;
            Type = typeName;
        }
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

        public Assignment(string identifier, Expression expr)
        {
            Identifier = identifier;
            Value = expr;
        }
    }

    public class Invocation : Expression
    {
        public string Identifier;
        public IMethodName Name;
        public string[] Parameters;

        public Invocation(string id, IMethodName name, params string[] parameters)
        {
            Identifier = id;
            Name = name;
            Parameters = parameters;
        }
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