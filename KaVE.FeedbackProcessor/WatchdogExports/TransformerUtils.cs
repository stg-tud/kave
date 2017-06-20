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
using System.Globalization;
using System.IO;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Model.Naming.IDEComponents;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Visitor;
using KaVE.Commons.Utils;
using KaVE.FeedbackProcessor.WatchdogExports.Model;

namespace KaVE.FeedbackProcessor.WatchdogExports
{
    internal static class TransformerUtils
    {
        public static bool EventHasNoTimeData(IDEEvent ideEvent)
        {
            return !ideEvent.TriggeredAt.HasValue || !ideEvent.TerminatedAt.HasValue;
        }

        private static readonly IList<string> TestNamespaces = new List<string>
        {
            "NUnit.Framework",
            "xunit",
            "Microsoft.VisualStudio.TestTools.UnitTesting",
            "csUnit",
            "MbUnit",
            "PetaTest"
        };

        private static readonly IList<string> TestFrameworkNamespaces = new List<string>
        {
            "moq",
            "Rhino.Mocks",
            "NSubstitute",
            "Simple.Mocking"
        };

        public static DocumentType GuessDocumentType(IDocumentName docName, ISST context)
        {
            if (docName.Language != "CSharp")
            {
                return DocumentType.Undefined;
            }

            var finder = new NamespaceFinderVisitor(TestNamespaces);
            var testWasFound = new ValueTypeWrapper<bool>(false);
            finder.Visit(context, testWasFound);

            if (testWasFound)
            {
                return DocumentType.Test;
            }

            finder = new NamespaceFinderVisitor(TestFrameworkNamespaces);
            var testFrameworkWasFound = new ValueTypeWrapper<bool>(false);
            finder.Visit(context, testFrameworkWasFound);

            if (testFrameworkWasFound)
            {
                return DocumentType.TestFramework;
            }

            if (Path.GetFileNameWithoutExtension(docName.FileName).Contains("test", CompareOptions.IgnoreCase))
            {
                return DocumentType.FilenameTest;
            }

            if (docName.FileName.Contains("test", CompareOptions.IgnoreCase))
            {
                return DocumentType.PathnameTest;
            }

            return DocumentType.Production;
        }

        public static void SetDocumentTypeIfNecessary(FileInterval interval, IDEEvent @event)
        {
            DocumentType newDocype;

            var ee = @event as EditEvent;
            if (ee != null && ee.Context2 != null)
            {
                newDocype = GuessDocumentType(ee.ActiveDocument, ee.Context2.SST);
            }
            else if (@event is CompletionEvent)
            {
                var ce = (CompletionEvent) @event;
                newDocype = GuessDocumentType(ce.ActiveDocument, ce.Context2.SST);
            }
            else
            {
                newDocype = GuessDocumentType(@event.ActiveDocument, new SST());
            }

            if (newDocype > interval.FileType)
            {
                interval.FileType = newDocype;
            }
        }
    }

    internal class NamespaceFinderVisitor : AbstractNodeVisitor<ValueTypeWrapper<bool>>
    {
        private readonly IList<string> _namespacePrefixes;

        public NamespaceFinderVisitor(IList<string> namespacePrefixes)
        {
            _namespacePrefixes = namespacePrefixes;
        }

        public override void Visit(IInvocationExpression entity, ValueTypeWrapper<bool> context)
        {
            if (entity.MethodName.DeclaringType.Namespace.Identifier.StartsWithAny(
                          _namespacePrefixes,
                          StringComparison.OrdinalIgnoreCase))
            {
                context.Value = true;
            }
        }
    }
}