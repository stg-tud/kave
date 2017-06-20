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
using KaVE.Commons.Model.Naming.Types;
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

        int NumTypeDeclTotal { get; }
        int NumTypeDeclTopLevel { get; }
        int NumTypeDeclNested { get; }
        int NumTypeDeclExtendsOrImplements { get; }

        [NotNull]
        IKaVESet<ITypeName> UniqueTypeDecl { get; }

        int NumPartial { get; }

        int NumClasses { get; }
        int NumInterfaces { get; }
        int NumDelegates { get; }
        int NumStructs { get; }
        int NumEnums { get; }
        int NumUnusualType { get; }

        int NumMethodDeclsTotal { get; }
        int NumMethodDeclsOverrideOrImplement { get; }
        int NumMethodDeclsOverrideOrImplementAsm { get; }

        [NotNull]
        IKaVESet<IMethodName> UniqueMethodDeclsOverrideOrImplementAsm { get; set; }

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

        public int NumTypeDeclTotal
        {
            get { return NumTypeDeclTopLevel + NumTypeDeclNested; }
        }

        public int NumTypeDeclTopLevel { get; set; }
        public int NumTypeDeclNested { get; set; }

        public IKaVESet<ITypeName> UniqueTypeDecl { get; set; }

        public int NumPartial { get; set; }

        public int NumClasses { get; set; }
        public int NumInterfaces { get; set; }
        public int NumDelegates { get; set; }
        public int NumStructs { get; set; }
        public int NumEnums { get; set; }
        public int NumUnusualType { get; set; }
        public int NumTypeDeclExtendsOrImplements { get; set; }

        public int NumMethodDeclsTotal { get; set; }
        public int NumMethodDeclsOverrideOrImplement { get; set; }
        public int NumMethodDeclsOverrideOrImplementAsm { get; set; }
        public IKaVESet<IMethodName> UniqueMethodDeclsOverrideOrImplementAsm { get; set; }
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
            UniqueTypeDecl = Sets.NewHashSet<ITypeName>();
            UniqueMethodDeclsOverrideOrImplementAsm = Sets.NewHashSet<IMethodName>();
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
            NumTypeDeclTopLevel += stats.NumTypeDeclTopLevel;
            NumTypeDeclNested += stats.NumTypeDeclNested;
            NumPartial += stats.NumPartial;
            NumClasses += stats.NumClasses;
            NumInterfaces += stats.NumInterfaces;
            NumDelegates += stats.NumDelegates;
            NumStructs += stats.NumStructs;
            NumEnums += stats.NumEnums;
            NumUnusualType += stats.NumUnusualType;
            NumTypeDeclExtendsOrImplements += stats.NumTypeDeclExtendsOrImplements;
            NumMethodDeclsTotal += stats.NumMethodDeclsTotal;
            NumMethodDeclsOverrideOrImplement += stats.NumMethodDeclsOverrideOrImplement;
            NumMethodDeclsOverrideOrImplementAsm += stats.NumMethodDeclsOverrideOrImplementAsm;
            NumValidInvocations += stats.NumValidInvocations;
            NumUnknownInvocations += stats.NumUnknownInvocations;
            NumAsmCalls += stats.NumAsmCalls;
            NumAsmDelegateCalls += stats.NumAsmDelegateCalls;
            NumAsmFieldRead += stats.NumAsmFieldRead;
            NumAsmPropertyRead += stats.NumAsmPropertyRead;

            UniqueTypeDecl.AddAll(stats.UniqueTypeDecl);
            UniqueMethodDeclsOverrideOrImplementAsm.AddAll(stats.UniqueMethodDeclsOverrideOrImplementAsm);
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
                   NumTypeDeclTopLevel == other.NumTypeDeclTopLevel && NumTypeDeclNested == other.NumTypeDeclNested &&
                   NumPartial == other.NumPartial && NumClasses == other.NumClasses &&
                   NumInterfaces == other.NumInterfaces &&
                   NumDelegates == other.NumDelegates && NumStructs == other.NumStructs && NumEnums == other.NumEnums &&
                   NumUnusualType == other.NumUnusualType &&
                   NumTypeDeclExtendsOrImplements == other.NumTypeDeclExtendsOrImplements &&
                   NumMethodDeclsTotal == other.NumMethodDeclsTotal &&
                   NumMethodDeclsOverrideOrImplement == other.NumMethodDeclsOverrideOrImplement &&
                   NumMethodDeclsOverrideOrImplementAsm == other.NumMethodDeclsOverrideOrImplementAsm &&
                   NumValidInvocations == other.NumValidInvocations &&
                   NumUnknownInvocations == other.NumUnknownInvocations &&
                   UniqueAssemblies.Equals(other.UniqueAssemblies) && NumAsmCalls == other.NumAsmCalls &&
                   NumAsmDelegateCalls == other.NumAsmDelegateCalls && UniqueTypeDecl.Equals(other.UniqueTypeDecl) &&
                   UniqueAsmMethods.Equals(other.UniqueAsmMethods) &&
                   UniqueMethodDeclsOverrideOrImplementAsm.Equals(other.UniqueMethodDeclsOverrideOrImplementAsm) &&
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
                hashCode = (hashCode * 397) ^ NumTypeDeclTopLevel;
                hashCode = (hashCode * 397) ^ NumTypeDeclNested;
                hashCode = (hashCode * 397) ^ NumPartial;
                hashCode = (hashCode * 397) ^ NumClasses;
                hashCode = (hashCode * 397) ^ NumInterfaces;
                hashCode = (hashCode * 397) ^ NumDelegates;
                hashCode = (hashCode * 397) ^ NumStructs;
                hashCode = (hashCode * 397) ^ NumEnums;
                hashCode = (hashCode * 397) ^ NumUnusualType;
                hashCode = (hashCode * 397) ^ NumTypeDeclExtendsOrImplements;
                hashCode = (hashCode * 397) ^ NumMethodDeclsTotal;
                hashCode = (hashCode * 397) ^ NumMethodDeclsOverrideOrImplement;
                hashCode = (hashCode * 397) ^ NumMethodDeclsOverrideOrImplementAsm;
                hashCode = (hashCode * 397) ^ UniqueMethodDeclsOverrideOrImplementAsm.GetHashCode();
                hashCode = (hashCode * 397) ^ NumValidInvocations;
                hashCode = (hashCode * 397) ^ NumUnknownInvocations;
                hashCode = (hashCode * 397) ^ UniqueTypeDecl.GetHashCode();
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