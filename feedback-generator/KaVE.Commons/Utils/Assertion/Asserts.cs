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
        // do not chain the calls to improve readability of resulting stack traces!

        #region reference types

        [ContractAnnotation("obj:null => halt"), StringFormatMethod("text")]
        public static void NotNull([CanBeNull] object obj, [NotNull] string text, params object[] args)
        {
            if (obj == null)
            {
                throw GetException(text, args);
            }
        }

        [ContractAnnotation("obj:null => halt")]
        public static void NotNull([CanBeNull] object obj)
        {
            if (obj == null)
            {
                throw GetException("is null");
            }
        }

        [ContractAnnotation("obj:notnull => halt"), StringFormatMethod("text")]
        public static void Null([CanBeNull] object obj, [NotNull] string text, params object[] args)
        {
            if (obj != null)
            {
                throw GetException(text, args);
            }
        }

        [ContractAnnotation("obj:notnull => halt")]
        public static void Null([CanBeNull] object obj)
        {
            if (obj != null)
            {
                throw GetException("is not null");
            }
        }

        [StringFormatMethod("text")]
        public static void NotSame(object a, object b, [NotNull] string text, params object[] args)
        {
            if (a == b)
            {
                throw GetException(text, args);
            }
        }

        public static void NotSame(object a, object b)
        {
            if (a == b)
            {
                throw GetException("is the same");
            }
        }

        [StringFormatMethod("text")]
        public static void Same(object a, object b, [NotNull] string text, params object[] args)
        {
            if (a != b)
            {
                throw GetException(text, args);
            }
        }

        public static void Same(object a, object b)
        {
            if (a != b)
            {
                throw GetException("is not the same");
            }
        }

        #endregion

        #region fails

        [ContractAnnotation("condition:true => halt"), StringFormatMethod("text")]
        public static void Not(bool condition, [NotNull] string text, params object[] args)
        {
            if (condition)
            {
                throw GetException(text, args);
            }
        }

        [ContractAnnotation("condition:true => halt")]
        public static void Not(bool condition)
        {
            if (condition)
            {
                throw GetException("is true");
            }
        }

        [ContractAnnotation("condition:false => halt"), StringFormatMethod("text")]
        public static void That(bool condition, [NotNull] string text, params object[] args)
        {
            if (!condition)
            {
                throw GetException(text, args);
            }
        }

        [ContractAnnotation("condition:false => halt")]
        public static void That(bool condition)
        {
            if (!condition)
            {
                throw GetException("is false");
            }
        }

        #endregion

        #region comparisons

        public static void AreEqual(object o, object o2)
        {
            if (!o.Equals(o2))
            {
                throw GetException("are not equal: {0} and {1}", o, o2);
            }
        }

        public static void AreNotEqual(object o, object o2)
        {
            if (o.Equals(o2))
            {
                throw GetException("are equal: {0} and {1}", o, o2);
            }
        }

        public static void IsLess<T>(T a, T b) where T : IComparable<T>
        {
            if (a.CompareTo(b) >= 0)
            {
                throw GetException("{0} >= {1}", a, b);
            }
        }

        public static void IsLessOrEqual<T>(T a, T b) where T : IComparable<T>
        {
            if (a.CompareTo(b) > 0)
            {
                throw GetException("{0} > {1}", a, b);
            }
        }

        // TODO provide more assertions

        #endregion

        #region fails

        [ContractAnnotation("=> halt"), StringFormatMethod("text")]
        public static void Fail(string text, params object[] args)
        {
            throw GetException(text, args);
        }

        [ContractAnnotation("=> halt")]
        public static void Fail()
        {
            throw GetException("unexpected condition");
        }

        [ContractAnnotation("=> halt"), StringFormatMethod("text")]
        public static TR Fail<TR>(string text, params object[] args)
        {
            throw GetException(text, args);
        }

        #endregion

        private static Exception GetException(string text, params object[] args)
        {
            return new AssertException(args.Length == 0 ? text : text.FormatEx(args));
        }
    }
}