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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Utils
{
    public static class ToStringUtils
    {
        public static string ToStringReflection<T>(this T obj)
        {
            // ReSharper disable once CompareNonConstrainedGenericWithNull
            if (obj == null)
            {
                return "null";
            }
            try
            {
                return PrimitiveToString(obj) ??
                       StringToString(obj) ?? EnumerableToString(obj) ?? ReflectiveToString(obj);
            }
            catch (Exception e)
            {
                return string.Format(
                    "ToString reflection failed for '{0}@{1}': {2}\n{3}",
                    obj.GetType().Name,
                    obj.GetHashCode(),
                    e.Message,
                    e.StackTrace);
            }
        }

        private static string PrimitiveToString(object o)
        {
            return o.GetType().IsPrimitive ? o.ToString() : null;
        }

        private static string StringToString(object o)
        {
            var s = o as string;
            if (s == null)
            {
                return null;
            }
            if (s.Length <= 128)
            {
                return s;
            }
            return s.Substring(0, 128) + "... (cut)";
        }

        private static string EnumerableToString(object o)
        {
            var items = o as IEnumerable;
            if (items != null)
            {
                var sb = new StringBuilder();
                sb.AppendEnumerable(items, "", null);
                return sb.ToString();
            }
            return null;
        }

        private static string ReflectiveToString([NotNull] this object obj)
        {
            var type = obj.GetType();
            var sb = new StringBuilder();
            sb.AppendTypeAndHash(obj).Append("{\n");

            foreach (var member in GetMembers(type, obj))
            {
                var isLambdaExpr = member.Key.Name.Contains("CachedAnonymousMethodDelegate");
                if (isLambdaExpr)
                {
                    continue;
                }

                if (IsConstField(member.Key))
                {
                    continue;
                }

                sb.Append("   ").Append(member.Key.Name).Append(" = ");
                if (obj.Equals(member.Value))
                {
                    sb.AppendReference(member.Value);
                }
                else
                {
                    sb.AppendValue(obj, member.Value);
                }
                sb.Append(",\n");
            }

            return sb.Append('}').ToString();
        }

        private static bool IsConstField(MemberInfo memberInfo)
        {
            var fieldInfo = memberInfo as FieldInfo;
            return fieldInfo != null && fieldInfo.IsLiteral;
        }

        private static void AppendReference(this StringBuilder sb, object o)
        {
            sb.Append("{--> ").Append(o.GetType().Name).Append('@').Append(o.GetHashCode()).Append('}');
        }

        private static void AppendValue(this StringBuilder sb, object parent, object value)
        {
            if (value == null)
            {
                sb.Append("null");
            }
            else if (value is string)
            {
                // TODO shortening
                sb.Append('"');
                sb.Append(StringToString(value));
                sb.Append('"');
            }
            else
            {
                var enumerable2 = value as IEnumerable;
                if (enumerable2 != null)
                {
                    sb.AppendEnumerable(enumerable2, "   ", parent);
                }
                else
                {
                    var indentedToString = value.ToString().Replace("\n", "\n   ");
                    sb.Append(indentedToString);
                }
            }
        }

        private static StringBuilder AppendTypeAndHash(this StringBuilder sb, object o)
        {
            var enumType = o.GetType();
            if (!enumType.IsArray)
            {
                sb.Append(enumType.Name).Append('@').Append(o.GetHashCode()).Append(' ');
            }
            return sb;
        }

        private static void AppendEnumerable(this StringBuilder sb,
            [NotNull] IEnumerable items,
            string prefix,
            object parent)
        {
            sb.AppendTypeAndHash(items).Append("[\n");
            foreach (var item in items)
            {
                sb.Append(prefix).Append("   ");
                if (item == null)
                {
                    sb.Append("null");
                }
                else if (item is string)
                {
                    // TODO shortening
                    sb.Append('"');
                    sb.Append(StringToString(item));
                    sb.Append('"');
                }

                else if (item == parent)
                {
                    sb.AppendReference(item);
                }
                else
                {
                    var indentedNewLine = string.Format("\n{0}   ", prefix);
                    var indentedToString = item.ToString().Replace("\n", indentedNewLine);
                    sb.Append(indentedToString);
                }
                sb.Append(",\n");
            }
            sb.Append(prefix).Append("]");
        }

        private static Dictionary<MemberInfo, object> GetMembers(IReflect type, object obj)
        {
            var members = new Dictionary<MemberInfo, object>();
            const BindingFlags bindingFlagsAll = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                                                 | BindingFlags.Static;

            var fields = type.GetFields(bindingFlagsAll);
            foreach (var field in fields)
            {
                members.Add(field, field.GetValue(obj));
            }

            var properties = type.GetProperties(bindingFlagsAll);
            foreach (var property in properties)
            {
                var backingField = property.GetBackingField();
                if (backingField != null)
                {
                    members.Remove(backingField);
                    members.Add(property, property.GetValue(obj, null));
                }
            }

            return members;
        }

        private static FieldInfo GetBackingField(this PropertyInfo property)
        {
            var backingFieldName = string.Format("<{0}>k__BackingField", property.Name);
            var declType = property.DeclaringType;
            if (declType == null)
            {
                return null;
            }
            var backingField = declType.GetField(
                backingFieldName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                | BindingFlags.Static);
            return backingField;
        }
    }
}