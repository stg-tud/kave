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
using KaVE.Commons.Utils.Json;

namespace KaVE.Commons.Utils.CodeCompletion.Stores
{
    public abstract class UsageModelsSourceBase : IUsageModelsSource
    {
        public Uri Source { get; set; }

        public IEnumerable<UsageModelDescriptor> GetUsageModels()
        {
            return ReadIndexFile(GetIndexContent());
        }

        public abstract void Load(UsageModelDescriptor model, string baseTargetDirectory);

        protected abstract string GetIndexContent();

        private static IEnumerable<UsageModelDescriptor> ReadIndexFile(string indexFileContent)
        {
            IEnumerable<UsageModelDescriptor> result;

            try
            {
                result = indexFileContent.ParseJsonTo<IEnumerable<UsageModelDescriptor>>();
            }
            catch
            {
                result = null;
            }

            return result ?? new UsageModelDescriptor[0];
        }
    }
}