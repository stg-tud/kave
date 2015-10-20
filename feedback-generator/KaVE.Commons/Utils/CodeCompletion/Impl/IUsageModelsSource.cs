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
using System.Windows;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.IO;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Utils.CodeCompletion.Impl
{
    public interface IUsageModelsSource
    {
        IEnumerable<UsageModelDescriptor> UsageModels { get; }
        void LoadZip(CoReTypeName typeName);
    }

    public class UsageModelsSource : IUsageModelsSource
    {
        public IEnumerable<UsageModelDescriptor> UsageModels
        {
            get
            {
                // TODO implement this
                return new KaVEList<UsageModelDescriptor>
                {
                    new UsageModelDescriptor(new CoReTypeName("LSystem\\Collections\\List"), 2),
                    new UsageModelDescriptor(new CoReTypeName("LSomeAssembly\\SomeType"), 9),
                    new UsageModelDescriptor(new CoReTypeName("LSomeAssembly\\SomeOtherType"), 10)
                };
            }
        }

        [CanBeNull]
        public Uri Source { get; set; }

        protected readonly IIoUtils IoUtils;

        public UsageModelsSource(IIoUtils ioUtils)
        {
            IoUtils = ioUtils;
        }

        public void LoadZip(CoReTypeName typeName)
        {
            // TODO implement this
            MessageBox.Show("Loading type " + typeName, "(Prototype)");
        }
    }
}