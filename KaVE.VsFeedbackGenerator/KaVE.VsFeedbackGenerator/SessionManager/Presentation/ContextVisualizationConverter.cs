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
            var enclosingMethod = context.EnclosingMethod;

            context.GetNamespaces().ForEach(
                ns =>
                {
                    if (ns != null && !ns.Equals(mainNameSpace))
                    {
                        builder.AppendUsingLine(ns);
                    }
                });
            builder.AppendLine(0, Util.Bold("namespace"), Space, mainNameSpace.Name);
            builder.AppendLine(0, CurlyBracketOpen);
            builder.AppendTypeDeclarationLine(1, context.TypeShape.TypeHierarchy);
            builder.AppendLine(1, CurlyBracketOpen);
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
            builder.AppendLine(1, CurlyBracketClose);
            builder.Append(CurlyBracketClose);
            return builder.ToString();
        }

        private static IEnumerable<INamespaceName> GetNamespaces(this Context context)
        {
            var usings =
                new SortedSet<INamespaceName>(
                    new global::JetBrains.Comparer<INamespaceName>(
                        (ns1, ns2) =>
                            (ns1 == null)
                                ? -1
                                : (ns2 == null) ? 1 : System.String.CompareOrdinal(ns1.Identifier, ns2.Identifier)));
            if (context.TypeShape.TypeHierarchy.HasSuperclass)
            {
                Add(usings, context.TypeShape.TypeHierarchy.Extends.Element.Namespace);
            }
            context.TypeShape.TypeHierarchy.Implements.ForEach(i => Add(usings, i.Element.Namespace));
            if (context.EnclosingMethod != null)
            {
                context.EnclosingMethod.Parameters.ForEach(p => Add(usings, p.ValueType.Namespace));
                Add(usings, context.EnclosingMethod.ReturnType.Namespace);
            }
            context.EntryPoints.ForEach(
                ep =>
                {
                    ep.Parameters.ForEach(p => Add(usings, p.ValueType.Namespace));
                    Add(usings, ep.ReturnType.Namespace);
                    context.EntryPointToCalledMethods[ep].ForEach(
                        c =>
                        {
                            Add(usings, c.DeclaringType.Namespace);
                            c.Parameters.ForEach(p => Add(usings, p.ValueType.Namespace));
                            Add(usings, c.ReturnType.Namespace);
                        });
                });
            return usings;
        }

        private static void Add(SortedSet<INamespaceName> usings, INamespaceName item)
        {
            usings.Add(item);
        }

        private static IEnumerable<IMethodName> GetCalledMethods(this Context context, IMethodName method)
        {
            return context.EntryPoints.Contains(method)
                ? context.EntryPointToCalledMethods[method]
                : new HashSet<IMethodName>();
        }

        private static void AppendLine([NotNull] this StringBuilder builder, int indentDepth, params string[] elems)
        {
            AppendLine(builder, indentDepth, elems as IEnumerable<string>);
        }

        private static void AppendLine([NotNull] this StringBuilder builder, int indentDepth, IEnumerable<string> elems)
        {
            for (var i = 0; i < indentDepth; i++)
            {
                builder.Append(Indent);
            }
            elems.ForEach(e => builder.Append(e));
            builder.Append(LineBreak);
        }

        private static void AppendUsingLine(this StringBuilder builder, INamespaceName usedNamespace)
        {
            builder.AppendLine(0, Util.Bold("using"), Space, usedNamespace.Identifier);
        }

        private static void AppendTypeDeclarationLine(this StringBuilder builder, int indent, ITypeHierarchy hierarchy)
        {
            var elems = new List<string> {Util.Bold(hierarchy.Element.ToTypeCategory()), Space, hierarchy.Element.Name};
            if (hierarchy.HasSupertypes)
            {
                elems.AddSupertypes(hierarchy);
            }
            builder.AppendLine(indent, elems);
        }

        private static void AddSupertypes(this ICollection<string> elems, ITypeHierarchy hierarchy)
        {
            elems.Add(Space);
            elems.Add(Util.Bold(":"));
            elems.Add(Space);
            var supertypes = new List<ITypeHierarchy>();
            if (hierarchy.HasSuperclass)
            {
                supertypes.Add(hierarchy.Extends);
            }
            supertypes.AddRange(hierarchy.Implements);
            elems.Add(string.Join(", ", supertypes.Select(t => t.Element.Name)));
        }

        private static void AppendMethod(this StringBuilder builder,
            int indent,
            IMethodName method,
            IEnumerable<IMethodName> calledMethods,
            bool isEnclosingMethod = false,
            IName triggerTarget = null)
        {
            builder.AppendMethodDeclarationLine(indent, method);
            builder.AppendLine(indent, CurlyBracketOpen);
            calledMethods.ForEach(m => builder.AppendCalledMethodLine(indent + 1, m));
            if (isEnclosingMethod)
            {
                builder.AppendCompletionMarkerLine(indent + 1, triggerTarget);
            }
            builder.AppendLine(indent, CurlyBracketClose);
        }

        private static void AppendMethodDeclarationLine(this StringBuilder builder, int indent, IMethodName method)
        {
            var elems = (method.IsConstructor)
                ? new List<string> {method.DeclaringType.Name}
                : new List<string> {method.ReturnType.Name, Space, method.Name};
            elems.AddParameterList(method);
            builder.AppendLine(indent, elems);
        }

        private static void AddParameterList(this ICollection<string> elems, IMethodName method)
        {
            elems.Add("(");
            elems.Add(string.Join(", ", method.Parameters.Select(p => p.ValueType.Name + Space + p.Name)));
            elems.Add(")");
        }

        private static void AppendCalledMethodLine(this StringBuilder builder, int indent, IMethodName method)
        {
            var elems = (method.IsConstructor)
                ? new List<string> {"new", Space, method.DeclaringType.Name}
                : new List<string> {method.DeclaringType.Name, ".", method.Name};
            elems.AddArgumentList(method);
            builder.AppendLine(indent, elems);
        }

        private static void AddArgumentList(this ICollection<string> elems, IMethodName method)
        {
            elems.Add("(");
            elems.Add(string.Join(", ", method.Parameters.Select(p => p.ValueType.Name)));
            elems.Add(");");
        }

        private static void AppendCompletionMarkerLine(this StringBuilder builder, int indent, IName triggerTarget)
        {
            if (triggerTarget == null)
            {
                builder.AppendLine(indent, CompletionMarker);
            }
            else
            {
                builder.AppendLine(indent, triggerTarget.Identifier, ".", CompletionMarker);
            }
        }
    }
}