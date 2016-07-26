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
using System.Collections.Generic;
using System.Linq;
using KaVE.Commons.Model.Naming.Types.Organization;

namespace KaVE.Commons.Utils.SSTPrinter
{
    public static class SSTPrintingUtils
    {
        public static void FormatAsUsingList(this IEnumerable<INamespaceName> namespaces, SSTPrintingContext context)
        {
            var filteredNamespaceStrings = namespaces.Where(n => !n.IsUnknown)
                                                     .Select(n => n.Identifier.Trim())
                                                     .Where(n => !string.IsNullOrWhiteSpace(n))
                                                     .OrderBy(n => n, StringComparer.OrdinalIgnoreCase);

            foreach (var n in filteredNamespaceStrings)
            {
                context.Keyword("using").Space().Text(n).Text(";");

                if (!ReferenceEquals(n, filteredNamespaceStrings.Last()))
                {
                    context.NewLine();
                }
            }
        }
    }
}