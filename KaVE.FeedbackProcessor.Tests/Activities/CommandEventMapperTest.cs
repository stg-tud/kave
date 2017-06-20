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

using KaVE.Commons.Model.Events;
using KaVE.FeedbackProcessor.Activities;
using KaVE.FeedbackProcessor.Activities.Model;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Activities
{
    internal class CommandEventMapperTest : BaseToActivityMapperTest
    {
        // TODO make constructor params for mappers possible and use this mapping then
        private static readonly string[] TestMapping =
        {
            "Command ID,Mapping",
            "TextControl.Paste,D"
        };

        public override BaseToActivityMapper Sut
        {
            get { return new CommandEventToActivityMapper(); }
        }

        [Test]
        public void MapsCommandToActivityByProvidedMapping()
        {
            var @event = new CommandEvent { CommandId = "TextControl.Paste" };
            AssertMapsToActivity(@event, Activity.Development);
        }

        [Test]
        public void MapsUnknownCommandToOther()
        {
            var unknownCmd = new CommandEvent{CommandId = "SomeUnkownCommand"};
            AssertMapsToActivity(unknownCmd, Activity.Other);
        }

        [Test]
        public void MapsRecentFilesToNavigation()
        {
            var openRecentFileCmd = new CommandEvent {CommandId = "1 C:\\Some\\Path\\ToThe.File"};
            AssertMapsToActivity(openRecentFileCmd, Activity.Navigation);
        }

        [Test]
        public void MapsTfsCommandsToProjectManagement()
        {
            var tfsCommand = new CommandEvent{CommandId = "something.TfsFoo"};
            AssertMapsToActivity(tfsCommand, Activity.ProjectManagement);
        }
    }
}