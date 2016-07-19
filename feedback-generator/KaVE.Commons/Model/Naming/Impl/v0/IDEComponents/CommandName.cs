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

using KaVE.Commons.Model.Naming.IDEComponents;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.Naming.Impl.v0.IDEComponents
{
    public class CommandName : BaseName, ICommandName
    {
        public CommandName() : this(UnknownNameIdentifier) {}
        public CommandName([NotNull] string identifier) : base(identifier) {}

        private string[] _parts;

        private string[] Parts
        {
            get
            {
                if (_parts == null)
                {
                    _parts = new string[3];

                    var parts = Identifier.Split(':');
                    if (parts.Length >= 3)
                    {
                        _parts[0] = parts[0];
                        _parts[1] = parts[1];
                        _parts[2] = string.Join(":", parts, 2, parts.Length - 2);
                    }
                    else
                    {
                        _parts[0] = UnknownNameIdentifier;
                        _parts[1] = "-1";
                        _parts[2] = Identifier;
                    }
                }
                return _parts;
            }
        }

        public string Guid
        {
            get { return IsUnknown ? UnknownNameIdentifier : Parts[0]; }
        }

        public int Id
        {
            get { return IsUnknown ? -1 : int.Parse(Parts[1]); }
        }

        public string Name
        {
            get { return IsUnknown ? UnknownNameIdentifier : Parts[2]; }
        }

        public override bool IsUnknown
        {
            get { return UnknownNameIdentifier.Equals(Identifier); }
        }
    }
}