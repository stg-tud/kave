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
 * 
 * Contributors:
 *    - Sebastian Proksch
 */

using System.Collections.Generic;
using System.Linq;
using KaVE.Model.Collections;
using KaVE.Model.Names;
using KaVE.Model.SSTs.Declarations;
using KaVE.Model.SSTs.Statements;

namespace KaVE.Model.SSTs
{
    public class SST
    {
        public ITypeName EnclosingType { get; set; }
        public ISet<FieldDeclaration> Fields { get; set; }
        public ISet<PropertyDeclaration> Properties { get; set; }
        public ISet<MethodDeclaration> Methods { get; set; }
        public ISet<EventDeclaration> Events { get; set; }
        public ISet<DelegateDeclaration> Delegates { get; set; }

        public SST()
        {
            Fields = Sets.NewHashSet<FieldDeclaration>();
            Properties = Sets.NewHashSet<PropertyDeclaration>();
            Methods = Sets.NewHashSet<MethodDeclaration>();
            Events = Sets.NewHashSet<EventDeclaration>();
            Delegates = Sets.NewHashSet<DelegateDeclaration>();
        }

        // TODO convert to ISet
        public IList<MethodDeclaration> EntryPoints
        {
            get { return Methods.Where(m => m.IsEntryPoint).ToList(); }
        }

        // TODO convert to ISet
        public IList<MethodDeclaration> NonEntryPoints
        {
            get { return Methods.Where(m => !m.IsEntryPoint).ToList(); }
        }

        public void Add(CompletionTrigger tp) {}
    }
}