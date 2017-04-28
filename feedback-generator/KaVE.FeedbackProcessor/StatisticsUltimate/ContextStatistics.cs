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
        int EstimatedLinesOfCode { get; }

        int NumTopLevelType { get; }
        int NumNestedType { get; }

        int NumClasses { get; }
        int NumInterfaces { get; }
        int NumDelegates { get; }
        int NumStructs { get; }
        int NumEnums { get; }
        int NumUnusualType { get; }

        int NumTypeExtendsOrImplements { get; }
        int NumMethodDecls { get; }
        int NumMethodOverridesOrImplements { get; }
        int NumValidInvocations { get; }
        int NumUnknownInvocations { get; }

        [NotNull]
        IKaVESet<IAssemblyName> UniqueAssemblies { get; }

        int NumAsmDelegateCalls { get; }
        int NumAsmCalls { get; }

        [NotNull]
        IKaVESet<IMethodName> UniqueAsmMethods { get; }

        int NumAsmFieldRead { get; }

        [NotNull]
        IKaVESet<IFieldName> UniqueAsmFields { get; }

        int NumAsmPropertyRead { get; }

        [NotNull]
        IKaVESet<IPropertyName> UniqueAsmProperties { get; }


        void Add([NotNull] IContextStatistics stats);
    }

    public class ContextStatistics : IContextStatistics
    {
        public int NumRepositories { get; set; }
        public int NumUsers { get; set; }
        public int NumSolutions { get; set; }
        public int EstimatedLinesOfCode { get; set; }

        public int NumTopLevelType { get; set; }
        public int NumNestedType { get; set; }

        public int NumClasses { get; set; }
        public int NumInterfaces { get; set; }
        public int NumDelegates { get; set; }
        public int NumStructs { get; set; }
        public int NumEnums { get; set; }
        public int NumUnusualType { get; set; }
        public int NumTypeExtendsOrImplements { get; set; }

        public int NumMethodDecls { get; set; }
        public int NumMethodOverridesOrImplements { get; set; }
        public int NumValidInvocations { get; set; }
        public int NumUnknownInvocations { get; set; }

        public IKaVESet<IAssemblyName> UniqueAssemblies { get; set; }
        public int NumAsmCalls { get; set; }
        public int NumAsmDelegateCalls { get; set; }
        public IKaVESet<IMethodName> UniqueAsmMethods { get; set; }
        public int NumAsmFieldRead { get; set; }
        public IKaVESet<IFieldName> UniqueAsmFields { get; set; }
        public int NumAsmPropertyRead { get; set; }
        public IKaVESet<IPropertyName> UniqueAsmProperties { get; set; }

        public ContextStatistics()
        {
            UniqueAssemblies = Sets.NewHashSet<IAssemblyName>();
            UniqueAsmMethods = Sets.NewHashSet<IMethodName>();
            UniqueAsmFields = Sets.NewHashSet<IFieldName>();
            UniqueAsmProperties = Sets.NewHashSet<IPropertyName>();
        }

        public void Add(IContextStatistics stats)
        {
            NumRepositories += stats.NumRepositories;
            NumUsers += stats.NumUsers;
            NumSolutions += stats.NumSolutions;
            EstimatedLinesOfCode += stats.EstimatedLinesOfCode;
            NumTopLevelType += stats.NumTopLevelType;
            NumNestedType += stats.NumNestedType;
            NumClasses += stats.NumClasses;
            NumInterfaces += stats.NumInterfaces;
            NumDelegates += stats.NumDelegates;
            NumStructs += stats.NumStructs;
            NumEnums += stats.NumEnums;
            NumUnusualType += stats.NumUnusualType;
            NumTypeExtendsOrImplements += stats.NumTypeExtendsOrImplements;
            NumMethodDecls += stats.NumMethodDecls;
            NumMethodOverridesOrImplements += stats.NumMethodOverridesOrImplements;
            NumValidInvocations += stats.NumValidInvocations;
            NumUnknownInvocations += stats.NumUnknownInvocations;
            NumAsmCalls += stats.NumAsmCalls;
            NumAsmDelegateCalls += stats.NumAsmDelegateCalls;
            NumAsmFieldRead += stats.NumAsmFieldRead;
            NumAsmPropertyRead += stats.NumAsmPropertyRead;

            UniqueAssemblies.AddAll(stats.UniqueAssemblies);
            UniqueAsmMethods.AddAll(stats.UniqueAsmMethods);
            UniqueAsmFields.AddAll(stats.UniqueAsmFields);
            UniqueAsmProperties.AddAll(stats.UniqueAsmProperties);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        protected bool Equals(ContextStatistics other)
        {
            return NumRepositories == other.NumRepositories && NumUsers == other.NumUsers &&
                   NumSolutions == other.NumSolutions && EstimatedLinesOfCode == other.EstimatedLinesOfCode &&
                   NumTopLevelType == other.NumTopLevelType && NumNestedType == other.NumNestedType &&
                   NumClasses == other.NumClasses && NumInterfaces == other.NumInterfaces &&
                   NumDelegates == other.NumDelegates && NumStructs == other.NumStructs && NumEnums == other.NumEnums &&
                   NumUnusualType == other.NumUnusualType &&
                   NumTypeExtendsOrImplements == other.NumTypeExtendsOrImplements &&
                   NumMethodDecls == other.NumMethodDecls &&
                   NumMethodOverridesOrImplements == other.NumMethodOverridesOrImplements &&
                   NumValidInvocations == other.NumValidInvocations &&
                   NumUnknownInvocations == other.NumUnknownInvocations &&
                   UniqueAssemblies.Equals(other.UniqueAssemblies) && NumAsmCalls == other.NumAsmCalls &&
                   NumAsmDelegateCalls == other.NumAsmDelegateCalls && UniqueAsmMethods.Equals(other.UniqueAsmMethods) &&
                   NumAsmFieldRead == other.NumAsmFieldRead && UniqueAsmFields.Equals(other.UniqueAsmFields) &&
                   NumAsmPropertyRead == other.NumAsmPropertyRead &&
                   UniqueAsmProperties.Equals(other.UniqueAsmProperties);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 63446;
                hashCode = (hashCode * 397) ^ NumRepositories;
                hashCode = (hashCode * 397) ^ NumUsers;
                hashCode = (hashCode * 397) ^ NumSolutions;
                hashCode = (hashCode * 397) ^ EstimatedLinesOfCode;
                hashCode = (hashCode * 397) ^ NumTopLevelType;
                hashCode = (hashCode * 397) ^ NumNestedType;
                hashCode = (hashCode * 397) ^ NumClasses;
                hashCode = (hashCode * 397) ^ NumInterfaces;
                hashCode = (hashCode * 397) ^ NumDelegates;
                hashCode = (hashCode * 397) ^ NumStructs;
                hashCode = (hashCode * 397) ^ NumEnums;
                hashCode = (hashCode * 397) ^ NumUnusualType;
                hashCode = (hashCode * 397) ^ NumTypeExtendsOrImplements;
                hashCode = (hashCode * 397) ^ NumMethodDecls;
                hashCode = (hashCode * 397) ^ NumMethodOverridesOrImplements;
                hashCode = (hashCode * 397) ^ NumValidInvocations;
                hashCode = (hashCode * 397) ^ NumUnknownInvocations;
                hashCode = (hashCode * 397) ^ UniqueAssemblies.GetHashCode();
                hashCode = (hashCode * 397) ^ NumAsmCalls;
                hashCode = (hashCode * 397) ^ NumAsmDelegateCalls;
                hashCode = (hashCode * 397) ^ UniqueAsmMethods.GetHashCode();
                hashCode = (hashCode * 397) ^ NumAsmFieldRead;
                hashCode = (hashCode * 397) ^ UniqueAsmFields.GetHashCode();
                hashCode = (hashCode * 397) ^ NumAsmPropertyRead;
                hashCode = (hashCode * 397) ^ UniqueAsmProperties.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return this.ToStringReflection();
        }
    }
}