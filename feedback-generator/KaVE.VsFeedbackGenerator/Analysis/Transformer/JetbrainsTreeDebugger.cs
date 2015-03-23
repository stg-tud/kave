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
 *    - 
 */
using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.ReSharper.Psi.Tree;
using KaVE.VsFeedbackGenerator.Analysis.CompletionTarget;

namespace KaVE.VsFeedbackGenerator.Analysis.Transformer
{
    public static class JetbrainsTreeDebugger
    {
        private static readonly ISet<ITreeNode> Visited = new HashSet<ITreeNode>();

        public static void Dump(ITreeNode node, CompletionTargetMarker marker = null)
        {
            var sb = new StringBuilder();
            Traverse(node, sb, 0, marker);
            Console.WriteLine(sb.ToString());
            Visited.Clear();
        }

        private static void Traverse(ITreeNode node, StringBuilder sb, int depth, CompletionTargetMarker marker)
        {
            if (node.IsWhitespaceToken())
            {
                return;
            }

            sb.Indent(depth);
            sb.ObjectId(node, marker);
            sb.AppendLine();

            foreach (var child in node.Children())
            {
                if (Visited.Contains(child))
                {
                    sb.Indent(depth);
                    sb.Append("--> ");
                    sb.ObjectId(node, marker);
                    sb.AppendLine();
                }
                else
                {
                    Visited.Add(child);
                    Traverse(child, sb, depth + 1, marker);
                }
            }
        }

        private static void Indent(this StringBuilder sb, int depth)
        {
            sb.Append(new string(' ', 4*depth));
        }

        private static void ObjectId(this StringBuilder sb, object o, CompletionTargetMarker marker)
        {
            var isTarget = o == marker.AffectedNode;

            if (isTarget)
            {
                sb.Append("$$ ");
            }
            sb.Append(o.GetType().Name);
            sb.Append('@');
            sb.Append(o.GetHashCode());
            if (isTarget)
            {
                sb.Append(" $$");
            }
        }
    }
}