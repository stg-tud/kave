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
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Utils.Assertion
{
    public static class Asserts
    {
        [ContractAnnotation("obj:null => halt"), StringFormatMethod("message")]
        public static void NotNull([CanBeNull] object obj, [NotNull] string message, params object[] messageArgs)
        {
            That(obj != null, message, messageArgs);
        }

        [ContractAnnotation("obj:null => halt")]
        public static void NotNull([CanBeNull] object obj)
        {
            That(obj != null);
        }

        [ContractAnnotation("obj:notnull => halt"), StringFormatMethod("message")]
        public static void Null([CanBeNull] object obj, [NotNull] string message, params object[] messageArgs)
        {
            That(obj == null, message, messageArgs);
        }

        [ContractAnnotation("condition:true => halt"), StringFormatMethod("message")]
        public static void Not(bool condition, [NotNull] string message, params object[] messageArgs)
        {
            That(!condition, message, messageArgs);
        }

        [ContractAnnotation("condition:true => halt")]
        public static void Not(bool condition)
        {
            That(!condition);
        }

        [ContractAnnotation("condition:false => halt"), StringFormatMethod("message")]
        public static void That(bool condition, [NotNull] string message, params object[] messageArgs)
        {
            if (!condition)
            {
                Fail(message, messageArgs);
            }
        }

        [ContractAnnotation("condition:false => halt")]
        public static void That(bool condition)
        {
            if (!condition)
            {
                Fail();
            }
        }

        [ContractAnnotation("=> halt"), StringFormatMethod("message")]
        public static void Fail(string message, params object[] messageArgs)
        {
            throw new AssertException(String.Format(message, messageArgs));
        }

        [ContractAnnotation("=> halt")]
        public static void Fail()
        {
            throw new AssertException("unexpected condition");
        }

        [ContractAnnotation("=> halt"), StringFormatMethod("message")]
        public static TR Fail<TR>(string message, params object[] messageArgs)
        {
            Fail(message, messageArgs);
            return default(TR);
        }

        public static void NotSame(object a, object b)
        {
            Not(a == b, "same objects");
        }
    }
}