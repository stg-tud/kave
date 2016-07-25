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
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Impl.v1.Parser;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Model.Naming.Types.Organization;
using KaVE.Commons.Utils.Assertion;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.Naming.Impl.v1
{
    public class Names
    {
        [NotNull]
        public static ITypeName Type([NotNull] string input)
        {
            var ctx = TypeNameParseUtil.ValidateTypeName(input);
            return new TypeName(ctx);
        }

        public static IName ArrayType([NotNull] string input)
        {
            var ctx = TypeNameParseUtil.ValidateTypeName(input);
            return new ArrayTypeName(ctx);
        }

        public static INamespaceName Namespace(string input)
        {
            try
            {
                var ctx = TypeNameParseUtil.ValidateNamespaceName(input);
                return new NamespaceName(ctx);
            }
            catch (Exception)
            {
                return UnknownName.Get(typeof(INamespaceName));
            }
        }

        public static IAssemblyName Assembly(string input)
        {
            try
            {
                var ctx = TypeNameParseUtil.ValidateAssemblyName(input);
                return new AssemblyName(ctx);
            }
            catch (Exception)
            {
                return UnknownName.Get(typeof(IAssemblyName));
            }
        }

        private static MemberName GetMemberName(string input)
        {
            try
            {
                var ctx = TypeNameParseUtil.ValidateMemberName(input);
                if (ctx.UNKNOWN() == null)
                {
                    return new MemberName(ctx);
                }
                return null;
            }
            catch (AssertException)
            {
                return null;
            }
        }

        public static IFieldName Field(string input)
        {
            var name = GetMemberName(input);
            if (name != null)
            {
                return name;
            }
            return UnknownName.Get(typeof(IFieldName));
        }

        public static IEventName Event(string inputB, params object[] args)
        {
            var input = string.Format(inputB, args);
            var name = GetMemberName(input);
            if (name != null)
            {
                return name;
            }
            return UnknownName.Get(typeof(IEventName));
        }

        public static IPropertyName Property(string input)
        {
            var name = GetMemberName(input);
            if (name != null)
            {
                return name;
            }
            return UnknownName.Get(typeof(IPropertyName));
        }

        [NotNull]
        public static IMethodName Method([NotNull] string input)
        {
            try
            {
                var ctx = TypeNameParseUtil.ValidateMethodName(input);
                if (ctx.UNKNOWN() != null)
                {
                    return UnknownName.Get(typeof(IMethodName));
                }
                return new MethodName(ctx);
            }
            catch (Exception)
            {
                try
                {
                    var ctx = TypeNameParseUtil.ValidateMethodName(CsNameFixer.HandleOldMethodNames(input));
                    if (ctx.UNKNOWN() != null)
                    {
                        return UnknownName.Get(typeof(IMethodName));
                    }
                    return new MethodName(ctx);
                }
                catch (Exception)
                {
                    return UnknownName.Get(typeof(IMethodName));
                }
            }
        }

        public static IParameterName Parameter(string input)
        {
            try
            {
                var ctx = TypeNameParseUtil.ValidateParameterName(input);
                return new ParameterName(ctx);
            }
            catch (AssertException)
            {
                return UnknownName.Get(typeof(IParameterName));
            }
        }

        public static ILambdaName GetLambdaName(string input)
        {
            try
            {
                var ctx = TypeNameParseUtil.ValidateLambdaName(input);
                if (ctx.UNKNOWN() != null)
                {
                    return UnknownName.Get(typeof(ILambdaName));
                }
                return new LambdaName(ctx);
            }
            catch (AssertException)
            {
                return UnknownName.Get(typeof(ILambdaName));
            }
        }
    }
}