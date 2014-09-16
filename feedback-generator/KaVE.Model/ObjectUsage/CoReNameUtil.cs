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
 *    - Dennis Albrecht
 */

using System.Linq;
using System.Text;
using KaVE.Model.Names;
using KaVE.Utils;

namespace KaVE.Model.ObjectUsage
{
    public static class CoReNameUtil
    {
        public static CoReName ToCoReName(this IName name)
        {
            var typeName = name as ITypeName;
            if (typeName != null)
            {
                return typeName.ToCoReName();
            }

            var methodName = name as IMethodName;
            if (methodName != null)
            {
                return methodName.ToCoReName();
            }

            var fieldName = name as IFieldName;
            if (fieldName != null)
            {
                return fieldName.ToCoReName();
            }

            return null;
        }

        private static CoReName ToCoReName(this ITypeName name)
        {
            return new CoReTypeName(name.ToName());
        }

        private static CoReName ToCoReName(this IMethodName name)
        {
            var builder = new StringBuilder();
            builder.Append(name.DeclaringType.ToName(), ".", name.Name, "(");
            StringBuilderUtils.Append(builder, name.Parameters.Select(n => n.ValueType.ToName() + ";").ToArray());
            builder.Append(")", name.ReturnType.ToName(), ";");
            return new CoReMethodName(builder.ToString());
        }

        private static CoReName ToCoReName(this IFieldName name)
        {
            var builder = new StringBuilder();
            builder.Append(name.DeclaringType.ToName(), ".", name.Name, ";", name.ValueType.ToName());
            return new CoReFieldName(builder.ToString());
        }

        private static string ToName(this INamespaceName name)
        {
            return name.IsGlobalNamespace ? "" : string.Format("{0}{1}/", name.ParentNamespace.ToName(), name.Name);
        }

        private static string ToName(this ITypeName name)
        {
            var builder = new StringBuilder();
            builder.Append("L", name.Namespace.ToName(), name.Name);
            return builder.ToString();
        }
    }
}