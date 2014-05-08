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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using JetBrains.Util;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names;
using KaVE.VsFeedbackGenerator.Utils;
using Util = KaVE.VsFeedbackGenerator.Utils.XamlFormattingUtil;

namespace KaVE.VsFeedbackGenerator.SessionManager.Presentation
{
    public static class ContextVisualizationConverter
    {
        private const string LineBreak = "\r\n";
        private const string Indent = "  ";
        private const string Space = " ";
        private const string CurlyBracketOpen = "{";
        private const string CurlyBracketClose = "}";
        private const string CompletionMarker = ""; //"<Italic Foreground=\"Blue\">@Completion</Italic>";

        public static string ToXaml(this Context context)
        {
            if (context == null || context.TypeShape.TypeHierarchy == null)
            {
                return null;
            }

            var builder = new StringBuilder();

            builder.AppendHierarchyLine(context.TypeShape.TypeHierarchy);
            builder.AppendLine(0, CurlyBracketOpen);
            if (context.EnclosingMethod == null)
            {
                builder.AppendLine(1, CompletionMarker);
            }
            else
            {
                builder.AppendXamlMethodSignatureLine(context.EnclosingMethod);
                builder.AppendLine(1, CurlyBracketOpen);

                context.CalledMethods.ForEach(
                    m =>
                    {
                        var l = m.DeclaringType.Name + "." + m.Name + "(";
                        if (m.HasParameters)
                        {
                            l += string.Join(", ", m.Parameters.Select(p => p.ValueType.Name));
                        }
                        builder.AppendLine(2, l + ");");
                    });

                builder.AppendLine(2, CompletionMarker);
                builder.AppendLine(1, CurlyBracketClose);
            }
            builder.Append(CurlyBracketClose);

            return builder.ToString();
        }

        private static void AppendLine([NotNull] this StringBuilder builder, int indentDepth, string elem)
        {
            for (var i = 0; i < indentDepth; i++)
            {
                builder.Append(Indent);
            }
            builder.Append(elem);
            builder.Append(LineBreak);
        }

        private static void AppendHierarchyLine([NotNull] this StringBuilder builder, [NotNull] ITypeHierarchy hierarchy)
        {
            builder.AppendHierarchyElement(hierarchy.Element);
            if (hierarchy.HasSupertypes)
            {
                builder.AppendSupertypes(hierarchy);
            }
            builder.Append(LineBreak);
        }

        private static void AppendHierarchyElement([NotNull] this StringBuilder builder, [NotNull] ITypeName typeName)
        {
            builder.Append(Util.Bold(typeName.ToTypeCategory()))
                   .Append(Space)
                   .Append(typeName.FullName);
        }

        private static void AppendSupertypes([NotNull] this StringBuilder builder, [NotNull] ITypeHierarchy hierarchy)
        {
            builder.Append(Util.Bold(" : "));
            var supertypes = new List<ITypeHierarchy>();
            if (hierarchy.HasSuperclass)
            {
                supertypes.Add(hierarchy.Extends);
            }
            supertypes.AddRange(hierarchy.Implements);
            var supertypeNames = supertypes.Select(type => type.Element.FullName);
            builder.Append(string.Join(", ", supertypeNames));
        }

        private static void AppendXamlMethodSignatureLine([NotNull] this StringBuilder builder,
            [NotNull] IMethodName method)
        {
            builder.Append(Indent);
            builder.Append(method.ReturnType.FullName);
            if (!method.IsConstructor)
            {
                builder.Append(Space).Append(method.Name);
            }
            builder.Append("(");
            if (method.HasParameters)
            {
                var paramList = method.Parameters.Select(param => param.ValueType.FullName + Space + param.Name);
                builder.Append(string.Join(", ", paramList));
            }
            builder.Append(")");
            builder.Append(LineBreak);
        }
    }
}