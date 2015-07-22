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

using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.DataFlow;
using JetBrains.UI.CrossFramework;
using JetBrains.UI.ToolWindowManagement;
using KaVE.VS.Achievements.Achievements.Listing;

namespace KaVE.VS.Achievements.UI.AchievementGUI
{
    [ShellComponent]
    public class AchievementWindowRegistrar
    {
        [UsedImplicitly]
        private readonly Lifetime _lifetime;

        [UsedImplicitly]
        private readonly ToolWindowClass _toolWindowClass;

        public AchievementWindowRegistrar(Lifetime lifetime,
            ToolWindowManager toolWindowManager,
            AchievementWindowDescriptor descriptor,
            IAchievementListing achievementListing,
            IUiDelegator uiDelegator)
        {
            _lifetime = lifetime;

            _toolWindowClass = toolWindowManager.Classes[descriptor];
            _toolWindowClass.RegisterEmptyContent(
                lifetime,
                lt =>
                {
                    var control = new AchievementWindowControl(achievementListing, uiDelegator);
                    var wrapper = new EitherControl(control);
                    return wrapper.BindToLifetime(lt);
                });
        }
    }
}