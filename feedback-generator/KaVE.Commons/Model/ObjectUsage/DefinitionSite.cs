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

using KaVE.Commons.Utils;

namespace KaVE.Commons.Model.ObjectUsage
{
    // ReSharper disable InconsistentNaming

    public class DefinitionSite
    {
        public DefinitionSiteKind kind { get; set; }

        public CoReFieldName field { get; set; }

        public CoReMethodName method { get; set; }

        public int argIndex { get; set; }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        private bool Equals(DefinitionSite oth)
        {
            return kind.Equals(oth.kind) && argIndex == oth.argIndex && Equals(method, oth.method) &&
                   Equals(field, oth.field);
        }

        public override string ToString()
        {
            return this.ToStringReflection();
        }

        public override int GetHashCode()
        {
            var hashCode = 397;
            hashCode = (hashCode*397) ^ kind.GetHashCode();
            if(method != null) hashCode = (hashCode*397) ^ method.GetHashCode();
            if(field != null) hashCode = (hashCode*397) ^ field.GetHashCode();
            hashCode = (hashCode*397) ^ argIndex.GetHashCode();
            return hashCode;
        }
    }
}