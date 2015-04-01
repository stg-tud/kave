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

using System.Linq;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Declarations;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.Utils.Collections;

namespace KaVE.VsFeedbackGenerator.SessionManager.Anonymize.CompletionEvents
{
    public class SSTAnonymization
    {
        private readonly SSTStatementAnonymization _statementAnon;

        public SSTAnonymization(SSTStatementAnonymization stmtAnon)
        {
            _statementAnon = stmtAnon;
        }

        public ISST Anonymize(ISST sst)
        {
            var delegates = sst.Delegates.Select(Anonymize);
            var events = sst.Events.Select(Anonymize);
            var fields = sst.Fields.Select(Anonymize);
            var methods = sst.Methods.Select(Anonymize);
            var properties = sst.Properties.Select(Anonymize);
            return new SST
            {
                EnclosingType = sst.EnclosingType.ToAnonymousName(),
                Delegates = Sets.NewHashSetFrom(delegates),
                Events = Sets.NewHashSetFrom(events),
                Fields = Sets.NewHashSetFrom(fields),
                Methods = Sets.NewHashSetFrom(methods),
                Properties = Sets.NewHashSetFrom(properties)
            };
        }

        public IDelegateDeclaration Anonymize(IDelegateDeclaration d)
        {
            var defaultName = TypeName.UnknownName;
            var isDefaultName = defaultName.Equals(d.Name);
            return new DelegateDeclaration {Name = isDefaultName ? defaultName : d.Name.ToAnonymousName()};
        }

        public IEventDeclaration Anonymize(IEventDeclaration d)
        {
            var defaultName = EventName.UnknownName;
            var isDefaultName = defaultName.Equals(d.Name);
            return new EventDeclaration {Name = isDefaultName ? defaultName : d.Name.ToAnonymousName()};
        }

        public IFieldDeclaration Anonymize(IFieldDeclaration d)
        {
            var defaultName = FieldName.UnknownName;
            var isDefaultName = defaultName.Equals(d.Name);
            return new FieldDeclaration {Name = isDefaultName ? defaultName : d.Name.ToAnonymousName()};
        }

        public IMethodDeclaration Anonymize(IMethodDeclaration d)
        {
            var defaultName = MethodName.UnknownName;
            var isDefaultName = defaultName.Equals(d.Name);
            return new MethodDeclaration
            {
                Name = isDefaultName ? defaultName : d.Name.ToAnonymousName(),
                IsEntryPoint = d.IsEntryPoint,
                Body = _statementAnon.Anonymize(d.Body)
            };
        }

        public IPropertyDeclaration Anonymize(IPropertyDeclaration d)
        {
            var defaultName = PropertyName.UnknownName;
            var isDefaultName = defaultName.Equals(d.Name);
            return new PropertyDeclaration
            {
                Name = isDefaultName ? defaultName : d.Name.ToAnonymousName(),
                Get = _statementAnon.Anonymize(d.Get),
                Set = _statementAnon.Anonymize(d.Set)
            };
        }
    }
}