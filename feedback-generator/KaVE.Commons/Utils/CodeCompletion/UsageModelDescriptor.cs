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

using KaVE.Commons.Model.ObjectUsage;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Utils.CodeCompletion
{
    public class UsageModelDescriptor
    {
        [NotNull]
        public CoReTypeName TypeName { get; set; }

        public int Version { get; set; }

        public UsageModelDescriptor([NotNull] CoReTypeName typeName, int version)
        {
            TypeName = typeName;
            Version = version;
        }

        public override bool Equals(object obj)
        {
            var other = obj as UsageModelDescriptor;
            return other != null && Equals(Version, other.Version) && Equals(TypeName, other.TypeName);
        }

        public override int GetHashCode()
        {
            var hashCode = Version.GetHashCode();
            hashCode = (hashCode * 397) ^ TypeName.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return this.ToStringReflection();
        }
    }
}