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

using System.Linq;
using JetBrains;
using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Expressions;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.Model.SSTs.References;
using KaVE.Commons.Utils.Collections;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite
{
    internal class SSTAnalysisFixture
    {
        internal static readonly ITypeName Int = TypeName.Get("System.Int32, mscorlib, 4.0.0.0");
        internal static readonly ITypeName Byte = TypeName.Get("System.Byte, mscorlib, 4.0.0.0");
        internal static readonly ITypeName IntArray = TypeName.Get("System.Int32[], mscorlib, 4.0.0.0");
        internal static readonly ITypeName Void = TypeName.Get("System.Void, mscorlib, 4.0.0.0");
        internal static readonly ITypeName Unknown = UnknownTypeName.Instance;
        internal static readonly ITypeName String = TypeName.Get("System.String, mscorlib, 4.0.0.0");
        internal static readonly ITypeName Bool = TypeName.Get("System.Boolean, mscorlib, 4.0.0.0");
        internal static readonly ITypeName Object = TypeName.Get("System.Object, mscorlib, 4.0.0.0");
        internal static readonly ITypeName Exception = TypeName.Get("System.Exception, mscorlib, 4.0.0.0");
        internal static readonly ITypeName IOException = TypeName.Get("System.IO.IOException, mscorlib, 4.0.0.0");

        internal static readonly ITypeName ListOfInt =
            TypeName.Get("System.Collections.Generic.List'1, mscorlib, 4.0.0.0");

        // ReSharper disable once InconsistentNaming
        internal static readonly IMethodName Object_GetHashCode =
            MethodName.Get("[System.Int32, mscorlib, 4.0.0.0] [System.Object, mscorlib, 4.0.0.0].GetHashCode()");

        // ReSharper disable once InconsistentNaming
        internal static readonly IMethodName Object_Init =
            MethodName.Get("[System.Void, mscorlib, 4.0.0.0] [System.Object, mscorlib, 4.0.0.0]..ctor()");

        // ReSharper disable once InconsistentNaming
        internal static IMethodName ListOfInt_Init =
            MethodName.Get("[System.Void, mscorlib, 4.0.0.0] [System.Object, mscorlib, 4.0.0.0]..ctor()");

        // ReSharper disable once InconsistentNaming
        internal static readonly IMethodName String_Format =
            MethodName.Get(
                "static [System.String, mscorlib, 4.0.0.0] [System.String, mscorlib, 4.0.0.0].Format([System.String, mscorlib, 4.0.0.0] format)");

        // ReSharper disable once InconsistentNaming
        internal static readonly IMethodName Object_Equals =
            MethodName.Get(string.Format("static [{0}] [{1}].Equals([{1}] objA, [{1}] objB)", Bool, Object));

        // ReSharper disable once InconsistentNaming
        public static readonly IMethodName Object_ctor =
            MethodName.Get(string.Format("[{0}] [{1}]..ctor()", Void, Object));

        // ReSharper disable once InconsistentNaming
        public static readonly IMethodName Exception_ctor =
            MethodName.Get(string.Format("[{0}] [{1}]..ctor([{2}] message)", Void, Exception, String));

        internal static IMethodName GetHashCode(ITypeName declaringType)
        {
            return MethodName.Get(string.Format("[{0}] [{1}].GetHashCode()", Int, declaringType));
        }

        internal static IMethodName ToString(ITypeName declaringType)
        {
            return MethodName.Get(string.Format("[{0}] [{1}].ToString()", String, declaringType));
        }

        internal static IMethodName Equals(ITypeName declaringType, ITypeName argType, string argName)
        {
            return MethodName.Get(string.Format("[{0}] [{1}].Equals([{2}] {3})", Bool, declaringType, argType, argName));
        }

        internal static IMethodName ConsoleWrite(ITypeName arg)
        {
            return
                MethodName.Get(
                    string.Format("static [{0}] [System.Console, mscorlib, 4.0.0.0].Write([{1}] value)", Void, arg));
        }

        public static IMethodName GetMethodName(string cGet)
        {
            return MethodName.Get(cGet);
        }

        public static ITypeName Type(string simpleName)
        {
            return TypeName.Get(simpleName + ", TestProject");
        }

        public static IMethodName Method(ITypeName returnType, ITypeName declType, string name)
        {
            // TODO extend for parameters
            return MethodName.Get("[{0}] [{1}].{2}()".FormatEx(returnType, declType, name));
        }

        public static IFieldName Field(ITypeName valueType, ITypeName declType, string name)
        {
            return FieldName.Get("[{0}] [{1}].{2}".FormatEx(valueType, declType, name));
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
    }
}