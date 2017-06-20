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

using JetBrains.Application;
using JetBrains.ReSharper.Features.Navigation.Resources;
using JetBrains.UI.ToolWindowManagement;

namespace KaVE.VS.FeedbackGenerator.SessionManager.Presentation
{
    [ToolWindowDescriptor(
        ProductNeutralId = "SessionManagerFeedbackWindow",
        Text = "Event Manager",
        Type = ToolWindowType.SingleInstance,
        VisibilityPersistenceScope = ToolWindowVisibilityPersistenceScope.Global,
        Icon = typeof(FeaturesFindingThemedIcons.SearchOptionsPage), // TODO Replace with own icon
        InitialDocking = ToolWindowInitialDocking.Bottom, // TODO make it dock!
        InitialHeight = 400,
        InitialWidth = 1000
        )]
    public class SessionManagerWindowDescriptor : ToolWindowDescriptor
    {
        public SessionManagerWindowDescriptor(IApplicationHost applicationHost)
            : base(applicationHost) {}

        /// <summary>
        ///     If the type FeaturesFindingThemedIcons.SearchOptionsPage is only used within attributes, it will not be loaded
        ///     properly while installing the ReSharper plugin and cause errors. The errors don't actually cause any problems, but
        ///     using the type within actual code will make the errors disappear.
        /// </summary>
        private static void __nothing__()
        {
            var _ = FeaturesFindingThemedIcons.SearchOptionsPage.Id;
        }
    }
}