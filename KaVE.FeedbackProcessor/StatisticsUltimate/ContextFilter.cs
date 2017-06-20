/*
 * Copyright 2017 Sebastian Proksch
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

using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Collections;

namespace KaVE.FeedbackProcessor.StatisticsUltimate
{
    /// <summary>
    ///     thread safe way to manage visiting of types/method decls to avoid duplications
    /// </summary>
    public interface IContextFilter
    {
        bool ShouldProcessOrRegister(ISST td);
        bool ShouldProcessOrRegister(IMethodName m);
    }

    public enum GeneratedCode
    {
        Include,
        Exclude
    }

    public enum Duplication
    {
        Include,
        Exclude
    }

    public class ContextFilter : IContextFilter
    {
        private readonly GeneratedCode _genCodeSetting;
        private readonly Duplication _dupeSetting;

        private readonly IKaVESet<ITypeName> _seenTypes = Sets.NewHashSet<ITypeName>();
        private readonly IKaVESet<IMethodName> _seenMethods = Sets.NewHashSet<IMethodName>();

        public ContextFilter(GeneratedCode genCodeSetting, Duplication dupeSetting)
        {
            _genCodeSetting = genCodeSetting;
            _dupeSetting = dupeSetting;
        }

        public bool ShouldProcessOrRegister(ISST td)
        {
            if (IsGenerated(td) && _genCodeSetting == GeneratedCode.Exclude)
            {
                return false;
            }

            if (td.IsPartialClass)
            {
                return true;
            }

            lock (_seenTypes)
            {
                if (_seenTypes.Contains(td.EnclosingType))
                {
                    return _dupeSetting == Duplication.Include;
                }
                _seenTypes.Add(td.EnclosingType);
                return true;
            }
        }

        private static bool IsGenerated(ISST td)
        {
            return td.IsPartialClass && td.PartialClassIdentifier != null &&
                   (td.PartialClassIdentifier.Contains(".Designer") || td.PartialClassIdentifier.Contains(".designer"));
        }

        public bool ShouldProcessOrRegister(IMethodName m)
        {
            if (_dupeSetting == Duplication.Include)
            {
                return true;
            }

            lock (_seenMethods)
            {
                if (_seenMethods.Contains(m))
                {
                    return false;
                }
                _seenMethods.Add(m);
                return true;
            }
        }

        public override string ToString()
        {
            return "{0}(GeneratedCode.{1}, Duplication.{2})".FormatEx(
                typeof(ContextFilter).Name,
                _genCodeSetting,
                _dupeSetting);
        }
    }
}