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
using KaVE.Commons.Model.Naming.Types.Organization;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Collections;
using KaVE.JetBrains.Annotations;

namespace KaVE.FeedbackProcessor.StatisticsUltimate
{
    public interface IContextStatistics
    {
        int NumRepositories { get; }
        int NumUsers { get; }
        int NumSolutions { get; }

        int NumTopLevelType { get; }
        int NumNestedType { get; }

        int NumClasses { get; }
        int NumInterfaces { get; }
        int NumDelegates { get; }
        int NumStructs { get; }
        int NumEnums { get; }

        int NumClassExtendOrImplement { get; }
        int NumMethodDecls { get; }
        int NumMethodOverridesOrImplements { get; }

        [NotNull]
        IKaVESet<IAssemblyName> UniqueAssemblies { get; }

        int NumCalls { get; }

        [NotNull]
        IKaVESet<IMethodName> UniqueMethods { get; }

        int NumFieldRead { get; }

        [NotNull]
        IKaVESet<IFieldName> UniqueFields { get; }

        int NumPropertyRead { get; }

        [NotNull]
        IKaVESet<IPropertyName> UniqueProperties { get; }

        void Add([NotNull] IContextStatistics stats);
    }

    public class ContextStatistics : IContextStatistics
    {
        public int NumRepositories { get; set; }
        public int NumUsers { get; set; }
        public int NumSolutions { get; set; }

        public int NumTopLevelType { get; set; }
        public int NumNestedType { get; set; }

        public int NumClasses { get; set; }
        public int NumInterfaces { get; set; }
        public int NumDelegates { get; set; }
        public int NumStructs { get; set; }
        public int NumEnums { get; set; }
        public int NumClassExtendOrImplement { get; set; }

        public int NumMethodDecls { get; set; }
        public int NumMethodOverridesOrImplements { get; set; }

        public IKaVESet<IAssemblyName> UniqueAssemblies { get; set; }
        public int NumCalls { get; set; }
        public IKaVESet<IMethodName> UniqueMethods { get; set; }
        public int NumFieldRead { get; set; }
        public IKaVESet<IFieldName> UniqueFields { get; set; }
        public int NumPropertyRead { get; set; }
        public IKaVESet<IPropertyName> UniqueProperties { get; set; }

        public ContextStatistics()
        {
            UniqueAssemblies = Sets.NewHashSet<IAssemblyName>();
            UniqueMethods = Sets.NewHashSet<IMethodName>();
            UniqueFields = Sets.NewHashSet<IFieldName>();
            UniqueProperties = Sets.NewHashSet<IPropertyName>();
        }

        public void Add(IContextStatistics stats)
        {
            NumRepositories += stats.NumRepositories;
            NumUsers += stats.NumUsers;
            NumSolutions += stats.NumSolutions;
            NumTopLevelType += stats.NumTopLevelType;
            NumNestedType += stats.NumNestedType;
            NumClasses += stats.NumClasses;
            NumInterfaces += stats.NumInterfaces;
            NumDelegates += stats.NumDelegates;
            NumStructs += stats.NumStructs;
            NumEnums += stats.NumEnums;
            NumClassExtendOrImplement += stats.NumClassExtendOrImplement;
            NumMethodDecls += stats.NumMethodDecls;
            NumMethodOverridesOrImplements += stats.NumMethodOverridesOrImplements;
            NumCalls += stats.NumCalls;
            NumFieldRead += stats.NumFieldRead;
            NumPropertyRead += stats.NumPropertyRead;

            UniqueAssemblies.AddAll(stats.UniqueAssemblies);
            UniqueMethods.AddAll(stats.UniqueMethods);
            UniqueFields.AddAll(stats.UniqueFields);
            UniqueProperties.AddAll(stats.UniqueProperties);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        protected bool Equals(ContextStatistics other)
        {
            return NumRepositories == other.NumRepositories && NumUsers == other.NumUsers &&
                   NumSolutions == other.NumSolutions && NumTopLevelType == other.NumTopLevelType &&
                   NumNestedType == other.NumNestedType && NumClasses == other.NumClasses &&
                   NumInterfaces == other.NumInterfaces && NumDelegates == other.NumDelegates &&
                   NumStructs == other.NumStructs && NumEnums == other.NumEnums &&
                   NumClassExtendOrImplement == other.NumClassExtendOrImplement &&
                   NumMethodDecls == other.NumMethodDecls &&
                   NumMethodOverridesOrImplements == other.NumMethodOverridesOrImplements &&
                   UniqueAssemblies.Equals(other.UniqueAssemblies) && NumCalls == other.NumCalls &&
                   UniqueMethods.Equals(other.UniqueMethods) && NumFieldRead == other.NumFieldRead &&
                   UniqueFields.Equals(other.UniqueFields) && NumPropertyRead == other.NumPropertyRead &&
                   Equals(UniqueProperties, other.UniqueProperties);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 46283;
                hashCode = (hashCode * 397) ^ NumRepositories;
                hashCode = (hashCode * 397) ^ NumUsers;
                hashCode = (hashCode * 397) ^ NumSolutions;
                hashCode = (hashCode * 397) ^ NumTopLevelType;
                hashCode = (hashCode * 397) ^ NumNestedType;
                hashCode = (hashCode * 397) ^ NumClasses;
                hashCode = (hashCode * 397) ^ NumInterfaces;
                hashCode = (hashCode * 397) ^ NumDelegates;
                hashCode = (hashCode * 397) ^ NumStructs;
                hashCode = (hashCode * 397) ^ NumEnums;
                hashCode = (hashCode * 397) ^ NumClassExtendOrImplement;
                hashCode = (hashCode * 397) ^ NumMethodDecls;
                hashCode = (hashCode * 397) ^ NumMethodOverridesOrImplements;
                hashCode = (hashCode * 397) ^ UniqueAssemblies.GetHashCode();
                hashCode = (hashCode * 397) ^ NumCalls;
                hashCode = (hashCode * 397) ^ UniqueMethods.GetHashCode();
                hashCode = (hashCode * 397) ^ NumFieldRead;
                hashCode = (hashCode * 397) ^ UniqueFields.GetHashCode();
                hashCode = (hashCode * 397) ^ NumPropertyRead;
                hashCode = (hashCode * 397) ^ UniqueProperties.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return this.ToStringReflection();
        }
    }
}