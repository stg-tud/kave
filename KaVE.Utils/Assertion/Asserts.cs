using System;
using KaVE.JetBrains.Annotations;

namespace KaVE.Utils.Assertion
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
    }
}