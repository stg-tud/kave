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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using KaVE.Commons.Utils.Assertion;

namespace KaVE.Commons.Utils.Reflection
{
    public static class TypeExtensions<T>
    {
        public static string GetPropertyName<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            if (expression.Body.NodeType == ExpressionType.MemberAccess)
            {
                return ((MemberExpression) expression.Body).Member.Name;
            }

            if ((expression.Body.NodeType == ExpressionType.Convert) && (expression.Body.Type == typeof (object)))
            {
                return ((MemberExpression) ((UnaryExpression) expression.Body).Operand).Member.Name;
            }

            return ThrowInvalidExpression(ExpressionType.MemberAccess, expression.Body.NodeType);
        }

        private static string ThrowInvalidExpression(ExpressionType expectedType, ExpressionType actualType)
        {
            throw new Exception(
                string.Format(
                    "Invalid expression type: Expected ExpressionType.{0}, found ExpressionType.{1}",
                    expectedType,
                    actualType));
        }

        public static string GetMethodName(Expression<Action<T>> expression)
        {
            if (expression.Body.NodeType == ExpressionType.Call)
            {
                return ((MethodCallExpression) expression.Body).Method.Name;
            }

            return ThrowInvalidExpression(ExpressionType.Call, expression.Body.NodeType);
        }
    }

    public static class TypeExtensions
    {
        public static IEnumerable<MemberInfo> GetMembersWithCustomAttributeNoInherit<TAttribute>(this Type self)
        {
            var members = self.GetMembers();
            return members.Where(member => member.GetCustomAttributes(typeof (TAttribute), false).Any());
        }

        public static TValue GetPublicPropertyValue<TValue>(this object self, String propertyName)
        {
            var type = self.GetType();
            var propertyInfo = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            Asserts.NotNull(
                propertyInfo,
                string.Format("Property '{0}' doesn't exist on '{1}'.", propertyName, type.FullName));
            return (TValue) propertyInfo.GetValue(self, new object[0]);
        }

        public static TValue GetPrivatePropertyValue<TValue>(this object self, String propertyName)
        {
            var type = self.GetType();
            var propertyInfo = type.GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance);
            Asserts.NotNull(
                propertyInfo,
                string.Format("Property '{0}' doesn't exist on '{1}'.", propertyName, type.FullName));
            return (TValue) propertyInfo.GetValue(self, new object[0]);
        }

        public static TValue GetPrivateFieldValue<TValue>(this object self, String fieldName)
        {
            var type = self.GetType();
            var propertyInfo = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            Asserts.NotNull(
                propertyInfo,
                string.Format("Field '{0}' doesn't exist on '{1}'.", fieldName, type.FullName));
            return (TValue)propertyInfo.GetValue(self);
        }

        public static void RegisterToEvent(this object self, String eventName, Delegate handler)
        {
            var type = self.GetType();
            var eventInfo = type.GetEvent(eventName);
            Asserts.NotNull(
                eventInfo,
                string.Format("Event '{0}' doesn't exist on '{1}'.", eventName, type.FullName));
            eventInfo.AddEventHandler(self, handler);
        }

        public static TResult InvokeNonPublic<TResult>(this object self, String methodName, params object[] args)
        {
            var type = self.GetType();
            var methodInfo = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            Asserts.NotNull(methodInfo,
                string.Format("Method '{0}' doesn't exist on '{1}'.", methodName, type.FullName));
            return (TResult) methodInfo.Invoke(self, args);
        }
    }
}