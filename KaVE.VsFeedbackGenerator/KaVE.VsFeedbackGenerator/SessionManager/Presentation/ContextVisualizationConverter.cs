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

using System;
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
        private const string CompletionMarker = "<Italic Foreground=\"Blue\">$</Italic>";

        public static string ToXaml(this Context context)
        {
            if (context == null || context.TypeShape == null || context.TypeShape.TypeHierarchy == null)
            {
                return null;
            }

            var builder = new StringBuilder();
            var mainNameSpace = context.TypeShape.TypeHierarchy.Element.Namespace;

            builder.AppendUsings(context, mainNameSpace);
            builder.AppendLine(0, Util.Bold("namespace"), Space, mainNameSpace.Name);
            builder.AppendLine(0, CurlyBracketOpen);
            builder.AppendTypeDeclarationLine(1, context.TypeShape.TypeHierarchy);
            builder.AppendLine(1, CurlyBracketOpen);
            builder.AppendTypeBody(context);
            builder.AppendLine(1, CurlyBracketClose);
            builder.Append(CurlyBracketClose);
            return builder.ToString();
        }

        private static void AppendLine([NotNull] this StringBuilder builder, int indent, params string[] elems)
        {
            builder.Append(indent, elems);
            builder.Append(LineBreak);
        }

        private static void Append([NotNull] this StringBuilder builder, int indent, params string[] elems)
        {
            for (var i = 0; i < indent; i++)
            {
                builder.Append(Indent);
            }
            elems.ForEach(e => builder.Append(e));
        }

        private static void AppendUsings(this StringBuilder builder, Context context, INamespaceName mainNameSpace)
        {
            var namespaces = context.GetNamespaces().Where(n => n != mainNameSpace).ToList();
            namespaces.ForEach(builder.AppendUsingLine);
            if (namespaces.Any())
            {
                builder.AppendLine();
            }
        }

        private static void AppendUsingLine(this StringBuilder builder, INamespaceName usedNamespace)
        {
            builder.AppendLine(0, Util.Bold("using"), Space, usedNamespace.Identifier, ";");
        }

        private static IEnumerable<INamespaceName> GetNamespaces(this Context context)
        {
            var usings =
                new SortedSet<INamespaceName>(
                    new global::JetBrains.Comparer<INamespaceName>(
                        (ns1, ns2) => String.CompareOrdinal(ns1.Identifier, ns2.Identifier)));
            usings.AddNamespacesFromSupertypes(context.TypeShape.TypeHierarchy);
            if (context.EnclosingMethod != null)
            {
                usings.AddNamespacesOfMethod(context.EnclosingMethod);
            }
            usings.AddNamespacesOfEntryPoints(context.EntryPointToCalledMethods);
            return usings;
        }

        private static void AddNamespacesFromSupertypes(this SortedSet<INamespaceName> usings,
            ITypeHierarchy typeHierarchy)
        {
            if (typeHierarchy.HasSuperclass)
            {
                usings.AddNamespaceOf(typeHierarchy.Extends.Element);
            }
            typeHierarchy.Implements.ForEach(i => usings.AddNamespaceOf(i.Element));
        }

        private static void AddNamespacesOfMethod(this SortedSet<INamespaceName> usings, IMethodName method)
        {
            method.Parameters.ForEach(p => usings.AddNamespaceOf(p.ValueType));
            usings.AddNamespaceOf(method.ReturnType);
        }

        private static void AddNamespacesOfEntryPoints(this SortedSet<INamespaceName> usings,
            IDictionary<IMethodName, ISet<IMethodName>> entryPointsToCalledMethods)
        {
            entryPointsToCalledMethods.Keys.ForEach(
                ep =>
                {
                    usings.AddNamespacesOfMethod(ep);
                    entryPointsToCalledMethods[ep].ForEach(
                        c =>
                        {
                            usings.AddNamespaceOf(c.DeclaringType);
                            c.Parameters.ForEach(p => usings.AddNamespaceOf(p.ValueType));
                        });
                });
        }

        private static void AddNamespaceOf(this ISet<INamespaceName> usings, ITypeName type)
        {
            if (!type.IsUnknownType && !type.Namespace.IsGlobalNamespace)
            {
                usings.Add(type.Namespace);
            }
        }

        private static void AppendTypeDeclarationLine(this StringBuilder builder, int indent, ITypeHierarchy hierarchy)
        {
            builder.Append(indent, Util.Bold(hierarchy.Element.ToTypeCategory()), Space, hierarchy.Element.Name);
            if (hierarchy.HasSupertypes)
            {
                builder.AppendSupertypes(hierarchy);
            }
            builder.AppendLine();
        }

        private static void AppendSupertypes(this StringBuilder builder, ITypeHierarchy hierarchy)
        {
            builder.Append(Space);
            builder.Append(Util.Bold(":"));
            builder.Append(Space);
            var supertypes = new List<ITypeHierarchy>();
            if (hierarchy.HasSuperclass)
            {
                supertypes.Add(hierarchy.Extends);
            }
            supertypes.AddRange(hierarchy.Implements);
            builder.Append(string.Join(", ", supertypes.Select(t => t.Element.Name)));
        }

        private static void AppendTypeBody(this StringBuilder builder, Context context)
        {
            var enclosingMethod = context.EnclosingMethod;
            if (enclosingMethod == null)
            {
                builder.AppendCompletionMarkerLine(2, context.TriggerTarget);
            }
            else
            {
                builder.AppendMethod(
                    2,
                    enclosingMethod,
                    context.GetCalledMethods(enclosingMethod),
                    true,
                    context.TriggerTarget);
            }
            context.EntryPoints.ForEach(
                entryPoint =>
                {
                    if (!entryPoint.Equals(enclosingMethod))
                    {
                        builder.AppendMethod(2, entryPoint, context.EntryPointToCalledMethods[entryPoint]);
                    }
                });
        }

        private static ICollection<IMethodName> GetCalledMethods(this Context context, IMethodName method)
        {
            return context.EntryPoints.Contains(method)
                ? context.EntryPointToCalledMethods[method]
                : new HashSet<IMethodName>();
        }

        private static void AppendMethod(this StringBuilder builder,
            int indent,
            IMethodName method,
            ICollection<IMethodName> calledMethods,
            bool isEnclosingMethod = false,
            IName triggerTarget = null)
        {
            builder.AppendMethodDeclarationLine(indent, method);
            builder.AppendLine(indent, CurlyBracketOpen);
            calledMethods.ForEach(m => builder.AppendCalledMethodLine(indent + 1, m));
            if (isEnclosingMethod)
            {
                if (calledMethods.Any())
                {
                    builder.AppendLine();
                }
                builder.AppendCompletionMarkerLine(indent + 1, triggerTarget);
            }
            builder.AppendLine(indent, CurlyBracketClose);
        }

        private static void AppendMethodDeclarationLine(this StringBuilder builder, int indent, IMethodName method)
        {
            if (method.IsConstructor)
            {
                builder.Append(indent, method.DeclaringType.Name);
            }
            else
            {
                builder.Append(indent, method.ReturnType.Name, Space, method.Name);
            }
            builder.AppendParameterList(method, p => p.ValueType.Name + Space + p.Name);
            builder.AppendLine();
        }

        private static void AppendCalledMethodLine(this StringBuilder builder, int indent, IMethodName method)
        {
            if (method.IsConstructor)
            {
                builder.Append(indent, "new", Space, method.DeclaringType.Name);
            }
            else
            {
                builder.Append(indent, method.DeclaringType.Name, ".", method.Name);
            }
            builder.AppendParameterList(method, p => p.ValueType.Name);
            builder.AppendLine(";");
        }

        private static void AppendParameterList(this StringBuilder builder,
            IMethodName method,
            Func<IParameterName, string> parameterToString)
        {
            builder.Append("(");
            builder.Append(string.Join(", ", method.Parameters.Select(parameterToString)));
            builder.Append(")");
        }

        private static void AppendCompletionMarkerLine(this StringBuilder builder, int indent, IName triggerTarget)
        {
            if (triggerTarget == null)
            {
                builder.AppendLine(indent, CompletionMarker);
            }
            else
            {
                builder.AppendLine(indent, "[", triggerTarget.Identifier, "].", CompletionMarker);
            }
        }
    }
}