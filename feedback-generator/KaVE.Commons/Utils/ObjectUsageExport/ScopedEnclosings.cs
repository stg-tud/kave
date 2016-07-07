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

using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Types;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Utils.ObjectUsageExport
{
    internal class ScopedEnclosings
    {
        private ITypeName _type;
        private IMethodName _method;

        [NotNull]
        public ITypeName Type
        {
            get
            {
                if (_type != null)
                {
                    return _type;
                }
                return Parent != null ? Parent.Type : Names.UnknownType;
            }
            set { _type = value; }
        }

        [NotNull]
        public IMethodName Method
        {
            get
            {
                if (_method != null)
                {
                    return _method;
                }
                return Parent != null ? Parent.Method : Names.UnknownMethod;
            }
            set { _method = value; }
        }

        public ScopedEnclosings Parent { get; private set; }

        public ScopedEnclosings() {}

        public ScopedEnclosings(ScopedEnclosings parent)
        {
            Parent = parent;
        }
    }
}