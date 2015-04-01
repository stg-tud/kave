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
 *    - Sven Amann
 */

using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.JetBrains.Annotations;
using KaVE.Commons.Utils.Assertion;

namespace KaVE.Commons.Model.Names
{
    internal class NameToStringConverter : TypeConverter
    {
        private const string NameQualifierPrefix = "KaVE.Model.Names.";
        private const char Separator = ':';

        private const BindingFlags GetMethodBindingFlags =
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof (string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertTo(ITypeDescriptorContext context,
            CultureInfo culture,
            object value,
            Type destinationType)
        {
            var name = value as Name;
            if (name != null && destinationType == typeof (string))
            {
                return AliasFor(value.GetType()) + Separator + name.Identifier;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        private static string AliasFor(Type nameType)
        {
            return nameType.FullName.Substring(NameQualifierPrefix.Length);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var serialization = value as string;
            if (serialization != null)
            {
                var data = serialization.Split(new[] {Separator}, 2);
                var type = TypeFrom(data[0]);
                Asserts.NotNull(type, "Could not load required type for '" + serialization + "'");
                var factoryMethod = GetFactoryMethod(type);
                return factoryMethod.Invoke(null, new object[] {data[1]});
            }
            return base.ConvertFrom(context, culture, value);
        }

        [CanBeNull]
        private static Type TypeFrom(string alias)
        {
            var assemblyName = typeof (IName).Assembly.FullName;
            var assemblyQualifiedTypeName = NameQualifierPrefix + alias + ", " + assemblyName;
            return Type.GetType(assemblyQualifiedTypeName);
        }

        private static MethodInfo GetFactoryMethod(Type type)
        {
            return type.GetMethods(GetMethodBindingFlags).Where(IsGetMethod).First();
        }

        private static bool IsGetMethod(MethodInfo method)
        {
            if (!method.Name.Equals("Get"))
            {
                return false;
            }
            var parameterInfos = method.GetParameters();
            return parameterInfos.Count() == 1 && parameterInfos[0].ParameterType == typeof (string);
        }
    }
}