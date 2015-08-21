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
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Impl.Visitor;
using KaVE.Commons.Utils.IO;
using NuGet;

namespace KaVE.RS.SolutionAnalysis
{
    public class CompletionEventFilter
    {
        private readonly IIoUtils _io;
        private readonly CompletionEventFilterLogger _logger;

        private readonly string _dirAll;
        private readonly string _dirCompletion;

        private int _numTotal;
        private readonly Dictionary<UseCase, int> _nums = new Dictionary<UseCase, int>();

        public CompletionEventFilter(string dirAll,
            string dirCompletion,
            IIoUtils io,
            CompletionEventFilterLogger logger)
        {
            _dirAll = dirAll;
            _dirCompletion = dirCompletion;
            _io = io;
            _logger = logger;

            InitNums();
        }

        private void InitNums()
        {
            foreach (var uc in Enum.GetValues(typeof (UseCase)))
            {
                _nums[(UseCase) uc] = 0;
            }
        }

        public void Run()
        {
            var zips = _io.GetFilesRecursive(_dirAll, "*.zip");
            _logger.FoundZips(zips.Length);
            var current = 1;
            foreach (var inZip in zips)
            {
                var outZip = GetTargetName(inZip);

                _logger.ProgressZip(current++, zips.Length, inZip, outZip);

                var ra = _io.ReadArchive(inZip);
                using (var wa = _io.CreateArchive(outZip))
                {
                    _logger.FoundEvents(ra.Count);

                    while (ra.HasNext())
                    {
                        var e = ra.GetNext<IDEEvent>();

                        _numTotal++;
                        var useCase = Categorize(e);
                        _nums[useCase]++;

                        _logger.ProgressEvent((char) useCase);
                        if (useCase == UseCase.Ok)
                        {
                            wa.Add(e);
                        }
                    }
                }
            }

            _logger.Finish(
                _numTotal,
                _nums[UseCase.Invalid],
                _nums[UseCase.Empty],
                _nums[UseCase.NoTrigger],
                _nums[UseCase.Ok]);
        }

        private string GetTargetName(string inZip)
        {
            var relativePath = inZip.Substring(_dirAll.Length);
            var outZip = Path.Combine(_dirCompletion, relativePath);
            var outParent = Directory.GetParent(outZip).FullName;
            _io.CreateDirectory(outParent);
            return outZip;
        }

        private UseCase Categorize(IDEEvent e)
        {
            var ce = e as CompletionEvent;
            if (ce == null)
            {
                return UseCase.OtherEvent;
            }
            if (!e.ActiveDocument.FileName.EndsWith(".cs"))
            {
                return UseCase.Invalid;
            }
            if (e.IDESessionUUID == null || e.IDESessionUUID.IsEmpty())
            {
                return UseCase.Invalid;
            }
            if (!e.TriggeredAt.HasValue)
            {
                return UseCase.Invalid;
            }
            var sst = ce.Context2.SST;
            if (sst.Methods.Count == 0)
            {
                return UseCase.Empty;
            }
            if (!HasTriggerPoint(sst))
            {
                return UseCase.NoTrigger;
            }
            return UseCase.Ok;
        }

        private static bool HasTriggerPoint(ISST sst)
        {
            var counter = HasCompletionExpressionVisitor.On(sst);
            return counter.HasCompletionExpression;
        }

        private enum UseCase
        {
            OtherEvent = '.',
            Invalid = ':',
            Empty = '|',
            NoTrigger = 'o',
            Ok = 'x'
        }
    }

    public class CompletionEventFilterLogger
    {
        public virtual void FoundZips(int num)
        {
            Log("found {0} zips...", num);
        }

        public virtual void ProgressZip(int current, int total, string inZip, string outZip)
        {
            Log("");
            Log("########## {0}/{1} ##########", current, total);
            Log("in:  {0}", inZip);
            Log("out: {0}", outZip);
            Log("reading... ");
        }

        private int _currentEvent;

        public virtual void FoundEvents(int count)
        {
            Append("{0} events found", count);
            Log("");
            Log("");

            _currentEvent = 0;
        }

        public virtual void ProgressEvent(char c)
        {
            if (++_currentEvent%200 == 0)
            {
                Log("");
            }
            Append("{0}", c);
        }

        public virtual void Finish(int numEvents,
            int numInvalid,
            int numEmpty,
            int numNoTrigger,
            int numOk)
        {
            var numCompletionEvents = numInvalid + numEmpty + numNoTrigger + numOk;
            Log("");
            Log("");
            Log("we found {0} CompletionEvents in the {0} IDEEvents", numCompletionEvents, numEvents);
            Log("-{0} invalid (e.g., missing timestamp, in .xml files, etc.)", numInvalid);
            Log("-{0} empty SSTs (e.g., no method declarations)", numEmpty);
            Log("-{0} contain no trigger information", numNoTrigger);
            Log("= {0} are ok and can be used", numOk);
            Log("");
        }

        private void Log(string msg, params object[] args)
        {
            Console.Write('\n');
            Console.Write(@"{0} | ", DateTime.Now);
            Console.Write(msg, args);
        }

        private void Append(string msg, params object[] args)
        {
            Console.Write(msg, args);
        }
    }
}