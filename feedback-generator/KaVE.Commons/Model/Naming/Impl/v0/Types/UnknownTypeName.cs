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

using KaVE.Commons.Model.Naming.Types;

namespace KaVE.Commons.Model.Naming.Impl.v0.Types
{
    public class UnknownTypeName : TypeName
    {
        public new const string Identifier = "?";

        private static ITypeName _instance;

        public static ITypeName Instance
        {
            get { return _instance ?? (_instance = new UnknownTypeName(Identifier)); }
        }

        internal static bool IsUnknownTypeIdentifier(string identifier)
        {
            return Identifier.Equals(identifier);
        }

        internal UnknownTypeName(string identifier) : base(identifier) {}

        public override bool IsUnknownType
        {
            get { return true; }
        }

        public override IAssemblyName Assembly
        {
            get { return Names.UnknownAssembly; }
        }

        public override INamespaceName Namespace
        {
            get { return Names.UnknownNamespace; }
        }
    }
}