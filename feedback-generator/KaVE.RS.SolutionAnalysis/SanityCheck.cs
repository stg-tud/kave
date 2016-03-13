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
using System.IO;
using System.Linq;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.SSTs.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Visitor;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.IO.Archives;

// ReSharper disable AccessToModifiedClosure
// ReSharper disable LocalizableElement

namespace KaVE.RS.SolutionAnalysis
{
    internal class SanityCheck
    {
        private readonly string _dirIn;

        public SanityCheck(string dirIn)
        {
            _dirIn = dirIn;
        }

        public void Run()
        {
            var numZips = 0;
            var numEvents = 0;
            var numWasApplied = 0;
            var numWasMethodApplied = 0;
            var numHasCompletion = 0;
            var numHasToken = 0;
            var numHasReference = 0;
            var numHasType = 0;

            Action log = () =>
            {
                Console.WriteLine();
                Console.WriteLine("numZips: {0}", numZips);
                Console.WriteLine("numEvents: {0}", numEvents);
                Console.WriteLine("numWasApplied: {0}", numWasApplied);
                Console.WriteLine("numWasMethodApplied: {0}", numWasMethodApplied);
                Console.WriteLine("numHasCompletion: {0}", numHasCompletion);
                Console.WriteLine("numHasToken: {0}", numHasToken);
                Console.WriteLine("numHasReference: {0}", numHasReference);
                Console.WriteLine("numHasType: {0}", numHasType);
            };

            Console.WriteLine("working in {0}", _dirIn);

            foreach (var zip in FindZips())
            {
                Console.WriteLine();
                Console.WriteLine("### opening {0}", zip);
                numZips++;

                foreach (var cce in ReadCce(zip))
                {
                    numEvents++;

                    if (cce.TerminatedState == TerminationState.Applied)
                    {
                        numWasApplied++;
                    }

                    if (cce.TerminatedState == TerminationState.Applied)
                    {
                        numWasApplied++;

                        if (IsMethodSelection(cce))
                        {
                            numWasMethodApplied++;
                        }
                    }

                    var complExpr = FindCompletion(cce);
                    if (complExpr != null)
                    {
                        numHasCompletion++;

                        if (!string.IsNullOrEmpty(complExpr.Token))
                        {
                            numHasToken++;
                        }
                        if (complExpr.TypeReference != null)
                        {
                            numHasType++;
                        }
                        if (complExpr.VariableReference != null)
                        {
                            numHasReference++;
                        }
                    }
                }

                log();
            }

            log();
        }

        private static ICompletionExpression FindCompletion(ICompletionEvent cce)
        {
            var cf = new CompletionFinder();
            cce.Context2.SST.Accept(cf, 0);
            return cf.CompletionExpr;
        }


        public IKaVEList<string> FindZips()
        {
            var findCcZips = Directory.EnumerateFiles(_dirIn, "*.zip", SearchOption.AllDirectories);
            var shortened = findCcZips.Select(z => z.Substring(_dirIn.Length));
            return Lists.NewListFrom(shortened);
        }

        private bool IsMethodSelection(ICompletionEvent ce)
        {
            var sel = ce.LastSelectedProposal;
            return sel != null && sel.Name is IMethodName;
        }

        private IEnumerable<ICompletionEvent> ReadCce(string zipName)
        {
            var fullPath = Path.Combine(_dirIn, zipName);
            var ra = new ReadingArchive(fullPath);
            while (ra.HasNext())
            {
                var e = ra.GetNext<IDEEvent>();
                var ce = e as CompletionEvent;
                if (ce != null)
                {
                    Console.Write('x');
                    yield return ce;
                }
                else
                {
                    Console.Write('.');
                }
            }
        }
    }

    internal class CompletionFinder : AbstractNodeVisitor<int>
    {
        public ICompletionExpression CompletionExpr { get; set; }

        public override void Visit(ICompletionExpression entity, int context)
        {
            CompletionExpr = entity;
        }
    }
}