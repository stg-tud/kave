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
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.Util;
using NUnit.Framework;

namespace KaVE.ReSharper.Commons.Tests_Unit.Utils
{
    [Ignore]
    internal class HierarchyTest
    {
        [Test]
        public void Test()
        {
            SubTypeGraphOf(
                typeof (ICSharpTreeNode),
                "c:/typegraph.dot",
                type => type.IsInterface && !typeof (ICSharpExpression).IsAssignableFrom(type),
                false);
        }

        public void SubTypeGraphOf(Type t, string path)
        {
            SubTypeGraphOf(t, path, type => true, true);
        }

        public void SubTypeGraphOf(Type t, string path, Predicate<Type> filter, bool showTransitives)
        {
            var types = t.Assembly.GetTypes().Where(type => filter(type) && t.IsAssignableFrom(type)).ToList();
            var edges =
                types.SelectMany(
                    type =>
                        types.Where(super => super.IsAssignableFrom(type) && super != type)
                             .Select(super => new Edge(type, super))).ToList();
            edges.Where(
                edge =>
                    edges.Where(start => start.Sub == edge.Sub && start.Super != edge.Super)
                         .Any(start => edges.Exists(end => end.Sub == start.Super && end.Super == edge.Super)))
                 .ForEach(edge => edge.IsTransitive = true);
            var builder = new StringBuilder("digraph typegraph {\n");
            types.ForEach(type => builder.AppendLine(type.Name));
            edges.Where(edge => showTransitives || !edge.IsTransitive)
                 .ForEach(edge => builder.AppendLine(edge.ToString()));
            builder.Append("}");
            using (var writer = new StreamWriter(new FileStream(path, FileMode.Create)))
            {
                writer.Write(builder.ToString());
            }
        }

        private class Edge
        {
            public readonly Type Sub;
            public readonly Type Super;
            public bool IsTransitive;

            public Edge(Type sub, Type super)
            {
                Sub = sub;
                Super = super;
            }

            public override string ToString()
            {
                return string.Format("{0} -> {1}{2}", Sub.Name, Super.Name, IsTransitive ? "[color=\"grey\"]" : "");
            }
        }
    }
}