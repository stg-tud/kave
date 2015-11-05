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
using JetBrains.Application;
using KaVE.Commons.Utils.CodeCompletion.Impl;
using KaVE.Commons.Utils.CodeCompletion.Impl.Stores.UsageModelSources;
using KaVE.Commons.Utils.CodeCompletion.Stores;
using KaVE.Commons.Utils.IO;
using KaVE.JetBrains.Annotations;

namespace KaVE.RS.Commons.Utils.CodeCompletion
{
    [ShellComponent]
    public class UsageModelsSourceFactory
    {
        [NotNull]
        private readonly IIoUtils _ioUtils;

        [NotNull]
        private readonly TypePathUtil _typePathUtil;

        public UsageModelsSourceFactory([NotNull] IIoUtils ioUtils, [NotNull] TypePathUtil typePathUtil)
        {
            _typePathUtil = typePathUtil;
            _ioUtils = ioUtils;
        }

        [Pure]
        public IUsageModelsSource GetSource(Uri source)
        {
            if (source.IsFile)
            {
                return new FilePathUsageModelsSource(_ioUtils, _typePathUtil, source);
            }

            if (source.AbsoluteUri.StartsWith("http://"))
            {
                return new HttpUsageModelsSource(source);
            }

            return new EmptyUsageModelsSource();
        }

        [Pure]
        public IUsageModelsSource GetSource(string uriSource)
        {
            try
            {
                return GetSource(new Uri(uriSource));
            }
            catch (UriFormatException)
            {
                return new EmptyUsageModelsSource();
            }
        }
    }
}