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
using KaVE.Model.Collections;
using KaVE.Model.SSTs;
using KaVE.Model.SSTs.Declarations;
using KaVE.Model.SSTs.Impl;
using KaVE.Model.SSTs.Impl.Declarations;
using KaVE.Model.SSTs.Impl.Visitor;

namespace KaVE.VsFeedbackGenerator.SessionManager.Anonymize
{
    public class SSTAnonymization
    {
        private SSTStatementAnonymization _statementAnonymization;

        public SSTAnonymization(SSTStatementAnonymization stmtAnon)
        {
            _statementAnonymization = stmtAnon;
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
            return new DelegateDeclaration {Name = d.Name.ToAnonymousName()};
        }

        public IEventDeclaration Anonymize(IEventDeclaration d)
        {
            return new EventDeclaration {Name = d.Name.ToAnonymousName()};
        }

        public IFieldDeclaration Anonymize(IFieldDeclaration d)
        {
            return new FieldDeclaration {Name = d.Name.ToAnonymousName()};
        }

        public IMethodDeclaration Anonymize(IMethodDeclaration d)
        {
            var body = d.Body.Select(Anonymize);
            return new MethodDeclaration
            {
                Name = d.Name.ToAnonymousName(),
                IsEntryPoint = d.IsEntryPoint,
                Body = Lists.NewListFrom(body)
            };
        }

        public IPropertyDeclaration Anonymize(IPropertyDeclaration d)
        {
            return new PropertyDeclaration {Name = d.Name.ToAnonymousName()};
        }

        public IStatement Anonymize(IStatement stmt)
        {
            return null;//stmt.Accept(_statementAnonymization);
        }
    }


    public class SSTExpressionAnonymization : AbstractNodeVisitor<int, IExpression> {}

    public class SSTReferenceAnonymization : AbstractNodeVisitor<int, IReference> { }

    public class SSTStatementAnonymization : AbstractNodeVisitor<int, IStatement> { }
}