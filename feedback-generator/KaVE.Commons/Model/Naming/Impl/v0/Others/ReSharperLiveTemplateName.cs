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
using KaVE.Commons.Model.Naming.Others;
using KaVE.Commons.Utils.Assertion;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.Naming.Impl.v0.Others
{
    public class ReSharperLiveTemplateName : BaseName, IReSharperLiveTemplateName
    {
        private const string Separator = ":";

        public ReSharperLiveTemplateName() : this(UnknownNameIdentifier) {}

        public ReSharperLiveTemplateName([NotNull] string identifier) : base(identifier)
        {
            if (!UnknownNameIdentifier.Equals(identifier))
            {
                Asserts.That(identifier.Contains(Separator));
            }
        }

        public string Name
        {
            get
            {
                var endOfName = Identifier.IndexOf(Separator, StringComparison.Ordinal);
                return IsUnknown ? UnknownNameIdentifier : Identifier.Substring(0, endOfName);
            }
        }

        public string Description
        {
            get
            {
                var startOfDescription = Identifier.IndexOf(Separator, StringComparison.Ordinal) + 1;
                return IsUnknown ? UnknownNameIdentifier : Identifier.Substring(startOfDescription);
            }
        }

        public override bool IsUnknown
        {
            get { return UnknownNameIdentifier.Equals(Identifier); }
        }
    }
}