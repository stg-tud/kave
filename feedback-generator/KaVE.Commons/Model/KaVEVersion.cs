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
using KaVE.Commons.Utils;

namespace KaVE.Commons.Model
{
    // ReSharper disable once InconsistentNaming
    public interface IKaVEVersion
    {
        Version Version { get; }
        Variant Variant { get; }
        int KaVEVersionNumber { get; }
    }

    public enum Variant
    {
        Unknown,
        Development,
        Default,
        Datev
    }

    public class KaVEVersion : IKaVEVersion
    {
        public Version Version
        {
            get { return new Version(0, KaVEVersionNumber); }
        }

        public Variant Variant { get; set; }
        public int KaVEVersionNumber { get; set; }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        protected bool Equals(KaVEVersion other)
        {
            return Variant == other.Variant && KaVEVersionNumber == other.KaVEVersionNumber;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 397;
                hashCode = hashCode ^ ((int) Variant*397);
                return hashCode ^ KaVEVersionNumber;
            }
        }

        public override string ToString()
        {
            return "0.{0}-{1}".FormatEx(KaVEVersionNumber, Variant);
        }
    }
}