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
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Assertion;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.Naming.Impl.v0.IDEComponents
{
    public class DocumentName : BaseName, IDocumentName
    {
        public DocumentName() : this(UnknownNameIdentifier) {}

        public DocumentName([NotNull] string identifier) : base(identifier)
        {
            if (!UnknownNameIdentifier.Equals(identifier))
            {
                Asserts.That(identifier.Contains(" "), "document name is missing a space '{0}'".FormatEx(identifier));
            }
        }

        private string[] _parts;

        private string[] Parts
        {
            get
            {
                if (_parts == null)
                {
                    if (Identifier.StartsWith("Plain Text "))
                    {
                        _parts = new[] {"Plain Text", Identifier.Substring(11)};
                    }
                    else
                    {
                        _parts = Identifier.Split(new[] {' '}, 2);
                    }
                }
                return _parts;
            }
        }

        public string Language
        {
            get { return IsUnknown ? UnknownNameIdentifier : Parts[0]; }
        }

        public string FileName
        {
            get { return IsUnknown ? UnknownNameIdentifier : Parts[1]; }
        }

        public override bool IsUnknown
        {
            get { return UnknownNameIdentifier.Equals(Identifier); }
        }
    }
}