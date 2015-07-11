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
using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.SSTPrinter;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.SSTPrinter
{
    public class SSTPrintingUtilsTest
    {
        [Test]
        public void UsingListFormattedCorrectly()
        {
            var namespaces = Sets.NewHashSet<INamespaceName>();
            namespaces.Add(NamespaceName.Get("Z"));
            namespaces.Add(NamespaceName.Get("System"));
            namespaces.Add(NamespaceName.Get("System"));
            namespaces.Add(NamespaceName.Get("System.Collections.Generic"));
            namespaces.Add(NamespaceName.Get("A"));
            namespaces.Add(NamespaceName.GlobalNamespace);

            var context = new SSTPrintingContext();
            namespaces.FormatAsUsingList(context);
            var expected = String.Join(
                Environment.NewLine,
                "using A;",
                "using System;",
                "using System.Collections.Generic;",
                "using Z;");
            Assert.AreEqual(expected, context.ToString());
        }

        [Test]
        public void UnknownNameIsNotAddedToList()
        {
            var namespaces = Sets.NewHashSet<INamespaceName>();
            namespaces.Add(NamespaceName.UnknownName);
            namespaces.Add(NamespaceName.GlobalNamespace);

            var context = new SSTPrintingContext();
            namespaces.FormatAsUsingList(context);
            const string expected = "";
            Assert.AreEqual(expected, context.ToString());
        }
    }
}