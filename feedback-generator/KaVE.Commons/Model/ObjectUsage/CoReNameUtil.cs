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
using System.Text;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Model.Naming.Types.Organization;
using KaVE.Commons.Utils;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.ObjectUsage
{
    public static class CoReNameUtil
    {
        [CanBeNull]
        public static CoReName ToCoReName([NotNull] this Proposal proposal)
        {
            return proposal.Name == null ? Names.UnknownGeneral.ToCoReName() : proposal.Name.ToCoReName();
        }

        [CanBeNull]
        public static CoReName ToCoReName([NotNull] this IName name)
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

        [NotNull]
        public static CoReTypeName ToCoReName([NotNull] this ITypeName name)
        {
            return new CoReTypeName(name.ToName());
        }

        [NotNull]
        public static CoReMethodName ToCoReName([NotNull] this IMethodName name)
        {
            var builder = new StringBuilder();
            if (name.IsConstructor)
            {
                builder.Append(name.DeclaringType.ToName(), ".<init>(");
                StringBuilderUtils.Append(builder, name.Parameters.Select(n => n.ValueType.ToName() + ";").ToArray());
                builder.Append(")LSystem/Void;");
            }
            else
            {
                builder.Append(name.DeclaringType.ToName(), ".");
                builder.Append(name.IsUnknown ? "unknown" : name.Name);
                builder.Append("(");
                // TODO @seb: fix analysis and remove check;
                if (name.Parameters != null)
                {
                    StringBuilderUtils.Append(
                        builder,
                        name.Parameters.Select(n => n.ValueType.ToName() + ";").ToArray());
                }
                builder.Append(")", name.ReturnType.ToName(), ";");
            }
            return new CoReMethodName(builder.ToString());
        }

        [NotNull]
        public static CoReFieldName ToCoReName([NotNull] this IFieldName name)
        {
            var builder = new StringBuilder();
            builder.Append(name.DeclaringType.ToName(), ".");
            builder.Append(name.IsUnknown ? "unknown" : name.Name);
            builder.Append(";", name.ValueType.ToName());
            return new CoReFieldName(builder.ToString());
        }

        private static string ToName(this INamespaceName name)
        {
            if (name.IsGlobalNamespace)
            {
                return "";
            }
            // TODO @seb: fix analysis and remove this check then
            if (name.ParentNamespace == null)
            {
                return "";
            }
            return string.Format("{0}{1}/", name.ParentNamespace.ToName(), name.Name);
        }

        private static string ToName(this ITypeName name)
        {
            if (name.IsTypeParameter)
            {
                return "LSystem/Object";
            }
            if (name.IsArray)
            {
                return "[" + name.AsArrayTypeName.ArrayBaseType.ToName();
            }
            if (name.IsNestedType)
            {
                return name.DeclaringType.ToName() + "$" + name.Name;
            }
            if (name.IsUnknown)
            {
                return "LUnknown";
            }
            var builder = new StringBuilder();
            builder.Append("L", name.Namespace.ToName(), GetName(name));
            return builder.ToString();
        }

        private static string GetName(ITypeName name)
        {
            if (name.IsPredefined)
            {
                // TODO (seb): get rid of this ugly hack, once models have been rebuild.
                return char.ToUpper(name.Name[0]) + name.Name.Substring(1);
            }
            return name.Name;
        }
    }
}