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
        private readonly NoTriggerPointOption _noTriggerPointOption;

        private int _numTotal;
        private int _numAdded;
        private readonly Dictionary<UseCase, int> _nums = new Dictionary<UseCase, int>();

        public CompletionEventFilter(string dirAll,
            string dirCompletion,
            NoTriggerPointOption noTriggerPointOption,
            IIoUtils io,
            CompletionEventFilterLogger logger)
        {
            _dirAll = dirAll;
            _dirCompletion = dirCompletion;
            _noTriggerPointOption = noTriggerPointOption;
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
            _logger.FoundZips(zips.Length, _noTriggerPointOption);
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
                        var isOk = useCase == UseCase.Ok;
                        var shouldKeepNoTrigger = useCase == UseCase.NoTrigger &&
                                                  _noTriggerPointOption == NoTriggerPointOption.Keep;
                        if (isOk || shouldKeepNoTrigger)
                        {
                            _numAdded++;
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
                _nums[UseCase.Ok],
                _numAdded,
                _noTriggerPointOption);
        }

        private string GetTargetName(string inZip)
        {
            var relativePath = inZip.Substring(_dirAll.Length);
            var outZip = Path.Combine(_dirCompletion, relativePath);
            var outParent = Directory.GetParent(outZip).FullName;
            _io.CreateDirectory(outParent);
            return outZip;
        }

        private static UseCase Categorize(IDEEvent e)
        {
            var ce = e as CompletionEvent;
            if (ce == null)
            {
                return UseCase.OtherEvent;
            }
            ce.ProposalCollection = new ProposalCollection();
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

        public enum NoTriggerPointOption
        {
            Keep,
            Remove
        }

        protected enum UseCase
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
        private DateTime _startedAt;

        public virtual void FoundZips(int num, CompletionEventFilter.NoTriggerPointOption noTriggerOption)
        {
            _startedAt = DateTime.Now;
            Log("processing {0} zips... (NoTrigger: {1})", num, noTriggerOption);
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
            Append("{0}", c);
            if (++_currentEvent%220 == 0)
            {
                Log("");
            }
        }

        public virtual void Finish(int numEvents,
            int numInvalid,
            int numEmpty,
            int numNoTrigger,
            int numOk,
            int numAdded,
            CompletionEventFilter.NoTriggerPointOption option)
        {
            var numCompletionEvents = numInvalid + numEmpty + numNoTrigger + numOk;
            Log("");
            Log("finished (started at: {0})", _startedAt);
            Log("");
            Log("we found {0} CompletionEvents in the {1} IDEEvents", numCompletionEvents, numEvents);
            Log("{0} are invalid (e.g., missing timestamp, not in .cs files, etc.)", numInvalid);
            Log("{0} contain empty SSTs (e.g., no method declarations)", numEmpty);
            Log("{0} contain no trigger information (NoTrigger: {1})", numNoTrigger, option);
            Log("{0} are 'ok'", numOk);
            Log("--> {0} are kept for further processing", numAdded);
            Log("");
        }

        private void Log(string msg, params object[] args)
        {
            Console.Write('\n');
            Console.Write(@"{0:yyMMddHHmmss} ", DateTime.Now);
            Console.Write(msg, args);
        }

        private void Append(string msg, params object[] args)
        {
            Console.Write(msg, args);
        }
    }
}