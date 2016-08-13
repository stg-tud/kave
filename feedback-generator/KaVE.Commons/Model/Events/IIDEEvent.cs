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
using KaVE.Commons.Model.Naming.IDEComponents;

namespace KaVE.Commons.Model.Events
{
    public interface IIDEEvent
    {
        /// <summary>
        ///     Reference of this IDE session. Used to identify events that
        ///     originate from the same IDE session. A session starts when the IDE
        ///     is opened and ends when the IDE is shut down.
        /// </summary>
        string IDESessionUUID { get; set; }

        /// <summary>
        ///     The version of the KaVE extension that generated the event.
        /// </summary>
        string KaVEVersion { get; set; }

        /// <summary>
        ///     The moment the event is created. Defaults to the event's creation time.
        /// </summary>
        DateTime? TriggeredAt { get; set; }

        /// <summary>
        ///     The kind of action that triggered the event.
        /// </summary>
        EventTrigger TriggeredBy { get; set; }

        /// <summary>
        ///     The moment the event, i.e., the corresponding action,
        ///     terminates. This property may be <code>null</code>, if the
        ///     termination of the event is not known.
        /// </summary>
        DateTime? TerminatedAt { get; set; }

        /// <summary>
        ///     The event's duration. Is <code>null</code>, iff
        ///     <see cref="TerminatedAt" /> is <code>null</code>.
        /// </summary>
        TimeSpan? Duration { get; set; }

        /// <summary>
        ///     The window with the focus at the moment the event is fired. If no window has the focus, this property is
        ///     <code>null</code>.
        /// </summary>
        IWindowName ActiveWindow { get; set; }

        /// <summary>
        ///     The document with the focus, i.e., the document inside the active window, at the moment the event is fired. If the
        ///     focus was not in a document, this property is <code>null</code>.
        /// </summary>
        IDocumentName ActiveDocument { get; set; }
    }

    /// <summary>
    ///     Represents the kinds of triggers that can cause events.
    /// </summary>
    public enum EventTrigger
    {
        /// <summary>
        ///     The source of the event is unknown, i.e., no triggering
        ///     interaction has been observed. This is the case when events
        ///     are triggered implicitely by another (possible external) event.
        /// </summary>
        Unknown = 0, // default

        /// <summary>
        ///     The event has been initiated by a mouse click.
        /// </summary>
        Click,

        /// <summary>
        ///     The event has been initiated by some shortcut, i.e., a keyboard
        ///     input with some modifier key(s) involved.
        /// </summary>
        Shortcut,

        /// <summary>
        ///     The event has been triggered by some non-shortcut keyboard input.
        /// </summary>
        Typing,

        /// <summary>
        ///     The event has been implicitly triggered by some other event or an action.
        /// </summary>
        Automatic
    }
}