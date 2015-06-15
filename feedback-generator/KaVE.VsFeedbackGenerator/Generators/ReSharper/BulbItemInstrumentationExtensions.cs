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
 * 
 * Contributors:
 *    - Sven Amann
 */

using System;
using System.Reflection;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.Intentions;
using JetBrains.UI.BulbMenu;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.Utils;

namespace KaVE.VsFeedbackGenerator.Generators.ReSharper
{
    internal static class BulbItemInstrumentationExtensions
    {
        private static readonly MethodInfo MyExecutableProxiBulbActionSetter =
            typeof (IntentionAction.MyExecutableProxi).GetProperty("BulbAction").GetSetMethod(true);

        private static readonly FieldInfo ExecutableItemActionField =
            typeof (ExecutableItem).GetField("myAction", BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        ///     Wraps the <see cref="IBulbAction" /> contained in the passed proxy in a
        ///     <see cref="EventGeneratingBulbActionProxy" />.
        /// </summary>
        public static void WrapBulbAction(this IntentionAction.MyExecutableProxi proxy,
            IRSEnv env,
            IMessageBus messageBus,
            IDateUtils dateUtils)
        {
            var originalBulbAction = proxy.BulbAction;
            var bulbActionWrapper = new EventGeneratingBulbActionProxy(
                originalBulbAction,
                env,
                messageBus,
                dateUtils);
            MyExecutableProxiBulbActionSetter.Invoke(proxy, new object[] {bulbActionWrapper});
        }

        public static void WrapBulbAction(this ExecutableItem executable,
            IRSEnv env,
            IMessageBus messageBus,
            IDateUtils dateUtils)
        {
            var originalAction = (Action) ExecutableItemActionField.GetValue(executable);
            var wrapper = new EventGeneratingActionWrapper(originalAction, env, messageBus, dateUtils);
            var newAction = (Action) (wrapper.Execute);
            ExecutableItemActionField.SetValue(executable, newAction);
        }
    }
}