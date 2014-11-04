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

using System;
using System.Collections.Generic;
using KaVE.Model.Collections;
using KaVE.Model.Names;
using KaVE.Model.SSTs.Declarations;
using KaVE.Model.SSTs.Statements;

namespace KaVE.Model.SSTs
{
    public class SST
    {
        private readonly ITypeName _td;

        private readonly ISet<FieldDeclaration> _fields = Sets.NewHashSet<FieldDeclaration>();
        private readonly ISet<PropertyDeclaration> _properties = Sets.NewHashSet<PropertyDeclaration>();
        private readonly ISet<MethodDeclaration> _methods = Sets.NewHashSet<MethodDeclaration>();
        private readonly ISet<MethodDeclaration> _eps = Sets.NewHashSet<MethodDeclaration>();
        private readonly ISet<EventDeclaration> _events = Sets.NewHashSet<EventDeclaration>();
        private readonly ISet<DelegateDeclaration> _delegates = Sets.NewHashSet<DelegateDeclaration>();

        public SST(ITypeName td)
        {
            _td = td;
        }

        public ITypeName GetEnclosingType()
        {
            return _td;
        }

        public void Add(CompletionTrigger tp)
        {
            //var i = 1 + 2;
            //var b = Console.Read();
            Console.Write("");
            Add((FieldDeclaration) null);
        }

        public void Add(MethodDeclaration md)
        {
            _methods.Add(md);
        }

        public void AddEntrypoint(MethodDeclaration mA)
        {
            _eps.Add(mA);
        }

        public void Add(FieldDeclaration fd)
        {
            _fields.Add(fd);
        }

        public ISet<MethodDeclaration> GetEntrypoints()
        {
            return _eps;
        }

        public ISet<MethodDeclaration> GetNonEntrypoints()
        {
            return _methods;
        }

        public void Add(PropertyDeclaration pd)
        {
            _properties.Add(pd);
        }

        public void Add(DelegateDeclaration dd)
        {
            _delegates.Add(dd);
        }

        public void Add(EventDeclaration ed)
        {
            _events.Add(ed);
        }
    }
}