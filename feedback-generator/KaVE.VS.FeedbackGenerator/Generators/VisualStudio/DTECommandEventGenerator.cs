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
using System.Collections.ObjectModel;
using System.Linq;
using EnvDTE;
using JetBrains.Application;
using JetBrains.Threading;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.IO;
using KaVE.JetBrains.Annotations;
using KaVE.VS.FeedbackGenerator.MessageBus;
using KaVE.VS.FeedbackGenerator.Utils.Naming;

namespace KaVE.VS.FeedbackGenerator.Generators.VisualStudio
{
    [ShellComponent]
    public class DTECommandEventGenerator : EventGeneratorBase
    {
        private static readonly ICollection<string> EventsDuplicatedByReSharper = new Collection<string>
        {
            "{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}:2:Edit.DeleteBackwards",
            "{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}:3:Edit.BreakLine",
            "{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}:4:Edit.InsertTab",
            "{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}:7:Edit.CharLeft",
            "{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}:8:Edit.CharLeftExtend",
            "{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}:9:Edit.CharRight",
            "{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}:10:Edit.CharRightExtend",
            "{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}:11:Edit.LineUp",
            "{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}:12:Edit.LineUpExtend",
            "{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}:13:Edit.LineDown",
            "{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}:14:Edit.LineDownExtend",
            "{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}:27:Edit.PageUp",
            "{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}:29:Edit.PageDown",
            "{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}:107:Edit.CompleteWord",
            "{5EFC7975-14BC-11CF-9B2B-00AA00573819}:627:Window.CloseAllDocuments"
        };

        private static readonly ICollection<string> EventsFiredAutomatically = new Collection<string>
        {
            "{5EFC7975-14BC-11CF-9B2B-00AA00573819}:1096:View.ObjectBrowsingScope",
            "{5EFC7975-14BC-11CF-9B2B-00AA00573819}:337:Edit.GoToFindCombo",
            "{5EFC7975-14BC-11CF-9B2B-00AA00573819}:684:Build.SolutionConfigurations",
            "{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}:1990:Build.SolutionPlatforms"
        };

        [UsedImplicitly]
        private readonly CommandEvents _commandEvents;

        private readonly Dictionary<string, CommandEvent> _eventQueue;

        public DTECommandEventGenerator(IRSEnv env, IMessageBus messageBus, IDateUtils dateUtils, IThreading threading)
            : base(env, messageBus, dateUtils, threading)
        {
            _eventQueue = new Dictionary<string, CommandEvent>();
            _commandEvents = DTE.Events.CommandEvents;
            _commandEvents.BeforeExecute += HandleCommandStarts;
            _commandEvents.AfterExecute += HandleCommandEnded;
        }

        private void HandleCommandStarts(string guid,
            int id,
            object customIn,
            object customOut,
            ref bool cancelDefault)
        {
            var commandEvent = CreateCommandEvent(guid, id);
            EnqueueEvent(commandEvent);
        }

        private CommandEvent CreateCommandEvent(string guid, int id)
        {
            var command = GetCommand(guid, id);
            var commandEvent = Create<CommandEvent>();
            if (command.HasPressedKeyBinding())
            {
                commandEvent.TriggeredBy = IDEEvent.Trigger.Shortcut;
            }
            commandEvent.CommandId = command.GetId();
            return commandEvent;
        }

        private Command GetCommand(string guid, int id)
        {
            try
            {
                return DTE.Commands.Item(guid, id);
            }
            catch (ArgumentException)
            {
                return new UnknownCommand {DTE = DTE, Guid = guid, ID = id};
            }
        }

        private void EnqueueEvent(CommandEvent evt)
        {
            Asserts.Not(_eventQueue.ContainsKey(evt.CommandId), "executing same event twice at a time: {0}", evt);
            _eventQueue.Add(evt.CommandId, evt);
        }

        private void HandleCommandEnded(string guid, int id, object customIn, object customOut)
        {
            var commandId = GetCommand(guid, id).GetId();
            // we need to take events from the queue here or enqueuing the next will fail!
            var commandEvent = TryTakeFromQueue(commandId);
            if (IsSuperfluousCommand(commandId))
            {
                return;
            }
            if (commandEvent == null && IsBasicCompletionCommand(commandId))
            {
                // for some reason code-completion command is not started...
                commandEvent = CreateCommandEvent(guid, id);
            }
            Asserts.NotNull(commandEvent, "command finished that didn't start: {0}", commandId);
            FireNow(commandEvent);
        }

        private static bool IsBasicCompletionCommand(string commandId)
        {
            return commandId.Equals("{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}:107:BasicCompletion");
        }

        private CommandEvent TryTakeFromQueue(string commandId)
        {
            var commandKey = commandId;
            CommandEvent evt;
            _eventQueue.TryGetValue(commandKey, out evt);
            _eventQueue.Remove(commandKey);
            return evt;
        }

        private static bool IsSuperfluousCommand(string commandId)
        {
            return IsDuplicatedByReSharper(commandId) || IsAutomaticEvent(commandId) || IsNamelessEvent(commandId);
        }

        /// <summary>
        ///     There exist ReSharper Actions for these commands, we don't need to track them twice.
        /// </summary>
        private static bool IsDuplicatedByReSharper(string commandId)
        {
            return EventsDuplicatedByReSharper.Contains(commandId) ||
                   IsReSharperActionEquivalent(commandId);
        }

        private static bool IsReSharperActionEquivalent(string commandId)
        {
            return commandId.Contains("ReSharper_");
        }

        /// <summary>
        ///     These commands are automatically triggered after every keyboard input. Furthermore, some of them are
        ///     triggered at regular intervals.
        /// </summary>
        private static bool IsAutomaticEvent(string commandId)
        {
            return EventsFiredAutomatically.Contains(commandId);
        }

        /// <summary>
        ///     Visual Studio often automatically triggers events without a name. Since we can't interpret them (and some are
        ///     triggered quite often and seemingly uncrelated to user interaction) we ignore them.
        /// </summary>
        private static bool IsNamelessEvent(string commandId)
        {
            return commandId.EndsWith(":");
        }
    }

    internal static class CommandHelper
    {
        internal static bool HasPressedKeyBinding(this Command command)
        {
            var bindings = ((object[]) command.Bindings).Cast<string>();
            return KeyUtils.ParseBindings(bindings).Any(b => b.IsPressed());
        }

        internal static string GetId([NotNull] this Command command)
        {
            return command.GetName().Identifier;
        }
    }

    /// <summary>
    ///     Internal dummy used to represent commands that are not known to the DTE.
    /// </summary>
    internal class UnknownCommand : Command
    {
        public object AddControl(object owner, int position = 1)
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public string Name
        {
            get { return ""; }
        }

        public Commands Collection
        {
            get { return null; }
        }

        public DTE DTE { get; internal set; }
        public string Guid { get; internal set; }
        public int ID { get; internal set; }

        public bool IsAvailable
        {
            get { return true; }
        }

        public object Bindings
        {
            get { return new string[0]; }
            set { }
        }

        public string LocalizedName
        {
            get { return ""; }
        }
    }
}