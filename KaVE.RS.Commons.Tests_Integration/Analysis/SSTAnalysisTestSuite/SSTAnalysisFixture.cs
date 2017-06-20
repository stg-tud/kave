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

using System.Linq;
using JetBrains;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Expressions;
using KaVE.Commons.Model.SSTs.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.Model.SSTs.References;
using KaVE.Commons.Model.SSTs.Statements;
using KaVE.Commons.Utils.Collections;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite
{
    internal class SSTAnalysisFixture
    {
        internal static readonly ITypeName TestClass = Names.Type("N.C, TestProject");

        internal static readonly ITypeName Int = Names.Type("p:int");

        internal static readonly ITypeName Byte = Names.Type("p:byte");

        internal static readonly ITypeName Float = Names.Type("p:float");

        internal static readonly ITypeName Double = Names.Type("p:double");

        internal static readonly ITypeName IntArray = Names.Type("p:int[]");

        internal static readonly ITypeName Void = Names.Type("p:void");

        internal static readonly ITypeName Unknown = Names.UnknownType;

        internal static readonly ITypeName String = Names.Type("p:string");

        internal static readonly ITypeName Bool = Names.Type("p:bool");

        internal static readonly ITypeName Object = Names.Type("p:object");

        internal static readonly ITypeName Exception = Names.Type("System.Exception, mscorlib, 4.0.0.0");

        internal static readonly ITypeName IOException = Names.Type("System.IO.IOException, mscorlib, 4.0.0.0");

        internal static readonly ITypeName Type = Names.Type("System.Type, mscorlib, 4.0.0.0");

        internal static readonly ITypeName ListOfInt =
            Names.Type("System.Collections.Generic.List`1[[T -> {0}]], mscorlib, 4.0.0.0", Int);

        internal static readonly ITypeName ListOfObject =
            Names.Type("System.Collections.Generic.List`1[[T -> {0}]], mscorlib, 4.0.0.0", Object);

        internal static readonly ITypeName Action = Names.Type("d:[{0}] [System.Action, mscorlib, 4.0.0.0].()", Void);

        internal static readonly ITypeName ActionOfInt =
            Names.Type("d:[{0}] [System.Action`1[[T -> {1}]], mscorlib, 4.0.0.0].([T] obj)", Void, Int);

        internal static readonly ITypeName FuncOfIntAndInt =
            Names.Type("d:[TResult] [System.Func`2[[T -> {0}],[TResult -> {0}]], mscorlib, 4.0.0.0].([T] arg)", Int);

        // ReSharper disable once InconsistentNaming
        internal static readonly IMethodName Object_GetHashCode = Names.Method("[{0}] [{1}].GetHashCode()", Int, Object);

        // ReSharper disable once InconsistentNaming
        internal static readonly IMethodName Int_GetHashCode = Names.Method("[{0}] [{0}].GetHashCode()", Int);

        // ReSharper disable once InconsistentNaming
        internal static readonly IMethodName Object_Init = Names.Method("[{0}] [{1}]..ctor()", Void, Object);

        // ReSharper disable once InconsistentNaming
        internal static IMethodName ListOfInt_Init =
            Names.Method("[{0}] [System.Collections.Generic.List`1[[T -> {1}]], mscorlib, 4.0.0.0]..ctor()", Void, Int);

        // ReSharper disable once InconsistentNaming
        internal static IMethodName ListOfInt_Add =
            Names.Method(
                "[{0}] [System.Collections.Generic.List`1[[T -> {1}]], mscorlib, 4.0.0.0].Add([T] item)",
                Void,
                Int);

        // ReSharper disable once InconsistentNaming
        internal static IMethodName ListOfObject_Add =
            Names.Method(
                "[{0}] [System.Collections.Generic.List`1[[T -> {1}]], mscorlib, 4.0.0.0].Add([T] item)",
                Void,
                Object);

        // ReSharper disable once InconsistentNaming
        internal static readonly IMethodName String_Format = Names.Method(
            "static [{0}] [{0}].Format([{0}] format)",
            String);

        // ReSharper disable once InconsistentNaming
        internal static readonly IMethodName Object_static_Equals =
            Names.Method(string.Format("static [{0}] [{1}].Equals([{1}] objA, [{1}] objB)", Bool, Object));

        // ReSharper disable once InconsistentNaming
        internal static readonly IMethodName Object_Equals =
            Names.Method(string.Format("[{0}] [{1}].Equals([{1}] obj)", Bool, Object));

        // ReSharper disable once InconsistentNaming
        internal static readonly IMethodName Object_GetType =
            Names.Method(string.Format("[{0}] [{1}].GetType()", Type, Object));

        // ReSharper disable once InconsistentNaming
        public static readonly IMethodName Object_ctor =
            Names.Method(string.Format("[{0}] [{1}]..ctor()", Void, Object));

        // ReSharper disable once InconsistentNaming
        public static readonly IMethodName Exception_ctor =
            Names.Method(string.Format("[{0}] [{1}]..ctor([{2}] message)", Void, Exception, String));

        public static readonly ITypeName EventHandler =
            Names.Type("System.EventHandler, mscorlib, 4.0.0.0");

        public static readonly ITypeName EventArgs =
            Names.Type("System.EventArgs, mscorlib, 4.0.0.0");

        // ReSharper disable once InconsistentNaming
        public static readonly IMethodName TestClass_Init =
            Names.Method(string.Format("[{0}] [{1}]..ctor()", Void, TestClass));

        public static readonly IReturnStatement Return =
            new ReturnStatement {IsVoid = true};

        // ReSharper disable once InconsistentNaming
        public static readonly IMethodName Action_Invoke =
            Names.Method(
                string.Format(
                    "[{0}] [d:[{0}] [System.Action`1[[T -> {1}]], mscorlib, 4.0.0.0].([T] obj)].Invoke([T] obj)",
                    Void,
                    Int));

        internal static IMethodName GetHashCode(ITypeName declaringType)
        {
            return Names.Method(string.Format("[{0}] [{1}].GetHashCode()", Int, declaringType));
        }

        internal static IMethodName ToString(ITypeName declaringType)
        {
            return Names.Method(string.Format("[{0}] [{1}].ToString()", String, declaringType));
        }

        internal static IMethodName Equals(ITypeName declaringType, ITypeName argType, string argName)
        {
            return Names.Method(string.Format("[{0}] [{1}].Equals([{2}] {3})", Bool, declaringType, argType, argName));
        }

        internal static IMethodName ConsoleWrite(ITypeName arg)
        {
            return
                Names.Method(
                    string.Format("static [{0}] [System.Console, mscorlib, 4.0.0.0].Write([{1}] value)", Void, arg));
        }

        public static IMethodName GetMethodName(string cGet)
        {
            return Names.Method(cGet);
        }

        public static ITypeName TypeInCoreLib(string simpleName)
        {
            return Names.Type(simpleName + ", mscorlib, 4.0.0.0");
        }

        public static ITypeName TypeInProject(string simpleName)
        {
            return Names.Type(simpleName + ", TestProject");
        }

        public static IParameterName Parameter(ITypeName parameterType, string name)
        {
            return Names.Parameter("[{0}] {1}", parameterType, name);
        }

        public static IMethodName Method(ITypeName returnType, ITypeName declType, string name)
        {
            return Names.Method("[{0}] [{1}].{2}()".FormatEx(returnType, declType, name));
        }

        public static IMethodName Method(ITypeName returnType,
            ITypeName declType,
            string name,
            params IParameterName[] parameters)
        {
            return
                Names.Method(
                    "[{0}] [{1}].{2}({3})".FormatEx(
                        returnType,
                        declType,
                        name,
                        string.Join(" ,", parameters.ToString())));
        }

        public static IFieldName Field(ITypeName valueType, ITypeName declType, string name)
        {
            return Names.Field("[{0}] [{1}].{2}".FormatEx(valueType, declType, name));
        }

        public static IEventName Event(ITypeName declType, string name)
        {
            return
                Names.Event(
                    "[d:[{0}] [{1}].([{2}] sender, [{3}] e)] [{4}].{5}".FormatEx(
                        Void,
                        EventHandler,
                        Object,
                        EventArgs,
                        declType,
                        name));
        }

        public static IPropertyName Property(ITypeName valueType,
            ITypeName declType,
            string name,
            bool hasGetter = true,
            bool hasSetter = true)
        {
            return
                Names.Property(
                    "{3}{4}[{0}] [{1}].{2}",
                    valueType,
                    declType,
                    name,
                    hasGetter ? "get " : "",
                    hasSetter ? "set " : "");
        }

        public static ICompletionExpression CompletionOnVar(IVariableReference varRef, string token)
        {
            return new CompletionExpression
            {
                VariableReference = varRef,
                Token = token
            };
        }

        public static ICompletionExpression CompletionOnType(ITypeName typeName, string token)
        {
            return new CompletionExpression
            {
                TypeReference = typeName,
                Token = token
            };
        }

        public static IStatement EmptyCompletion
        {
            get
            {
                return new ExpressionStatement
                {
                    Expression = new CompletionExpression()
                };
            }
        }

        public static IAssignableExpression ComposedExpr(params string[] varNames)
        {
            var varRefs = varNames.Select<string, IVariableReference>(n => new VariableReference {Identifier = n});
            return new ComposedExpression
            {
                References = Lists.NewListFrom(varRefs)
            };
        }

        public static IMethodName Ctor(ITypeName type, params IParameterName[] parameterNames)
        {
            var parameters = string.Join<IParameterName>(",", parameterNames);
            return Names.Method(string.Format("[{0}] [{1}]..ctor({2})", Void, type, parameters));
        }

        public static IReturnStatement ReturnVar(string varName)
        {
            return new ReturnStatement
            {
                IsVoid = false,
                Expression = new ReferenceExpression {Reference = new VariableReference {Identifier = varName}}
            };
        }
    }
}