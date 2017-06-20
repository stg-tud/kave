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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.TestRunEvents;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Utils;

namespace KaVE.FeedbackProcessor.WatchdogExports.Exporter
{
    public class EventStreamExport
    {
        private readonly string _dirRoot;
        private readonly IEventFixer _eventFixer;

        public EventStreamExport(string dirRoot, IEventFixer eventFixer)
        {
            _dirRoot = dirRoot;
            _eventFixer = eventFixer;
        }

        public void Write(IEnumerable<IDEEvent> events, string relFile)
        {
            var contents = CreateString(events);
            File.WriteAllText(Path.Combine(_dirRoot, relFile), contents);
        }

        private string CreateString(IEnumerable<IDEEvent> events)
        {
            events = _eventFixer.FixAndFilter(events);
            var sb = new StringBuilder();
            foreach (var e in events)
            {
                var hasArtificialEndDate = e.TerminatedAt == e.TriggeredAt;
                var start = e.TriggeredAt.HasValue ? e.TriggeredAt.Value.ToString("HH:mm:ss") : "--:--:--";
                var end = e.TerminatedAt.HasValue && !hasArtificialEndDate
                    ? e.TerminatedAt.Value.ToString("HH:mm:ss")
                    : "";
                var duration = e.Duration.HasValue && !hasArtificialEndDate
                    ? "{0:0.0s}".FormatEx(e.Duration.Value.TotalMilliseconds/1000)
                    : "";
                var file = e.ActiveDocument != null ? e.ActiveDocument.FileName : "";
                sb.AppendLine("{0}  {4,8} {1,6}  {2,-50} {3}".FormatEx(start, duration, Print(e), file, end));
            }
            return sb.ToString();
        }

        private static string Print(IIDEEvent e)
        {
            var ise = e as IDEStateEvent;
            if (ise != null)
            {
                return "IDEStateEvent({0})".FormatEx(ise.IDELifecyclePhase);
            }
            var de = e as DebuggerEvent;
            if (de != null)
            {
                return "DebuggerEvent({0}, {1})".FormatEx(de.Action, de.Mode);
            }
            var we = e as WindowEvent;
            if (we != null)
            {
                if (Names.Window("main Microsoft Visual Studio").Equals(we.Window))
                {
                    return we.Action == WindowAction.Activate
                        ? "FocusGain"
                        : "FocusLost";
                }
                return
                    "WindowEvent({0}, {1})".FormatEx(
                        we.Window.Caption.Replace("Unit Test Sessions - All tests from ", "Tests: ")
                          .Replace(" - Microsoft Visual Studio - Experimental Instance", ""),
                        we.Action);
            }
            var ce = e as CommandEvent;
            if (ce != null)
            {
                var cid = ce.CommandId;
                if (cid.Contains(':'))
                {
                    cid = cid.Substring(cid.LastIndexOf(':') + 1);
                }
                return "CommandEvent({0})".FormatEx(cid);
            }
            var tre = e as TestRunEvent;
            if (tre != null)
            {
                return "TestRunEvent({0} tests)".FormatEx(tre.Tests.Count);
            }
            var ne = e as NavigationEvent;
            if (ne != null)
            {
                switch (ne.TypeOfNavigation)
                {
                    case NavigationType.CtrlClick:
                        return "NavigationEvent({0} -> {1})".FormatEx(Simple(ne.Location), Simple(ne.Target));
                    case NavigationType.Unknown:
                        return "NavigationEvent(unknown)";
                    default:
                        return "NavigationEvent({0})".FormatEx(Simple(ne.Location));
                }
            }

            return e.GetType().Name;
        }

        private static string Simple(IName n)
        {
            var t = n as ITypeName;
            if (t != null)
            {
                return t.Name;
            }
            var m = n as IMemberName;
            if (m != null)
            {
                return "{0}.{1}".FormatEx(m.DeclaringType.Name, m.Name);
            }
            return n.Identifier;
        }
    }
}