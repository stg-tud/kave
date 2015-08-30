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

using System.Collections.Generic;
using System.Threading;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.Utils.Exceptions;
using KaVE.RS.Commons.Analysis.CompletionTarget;
using KaVE.RS.Commons.Utils.Names;
using KaVELogger = KaVE.Commons.Utils.Exceptions.ILogger;

namespace KaVE.RS.Commons.Analysis.Transformer
{
    public class DeclarationVisitor : TreeNodeVisitor<SST>
    {
        private readonly KaVELogger _logger;
        private readonly ISet<IMethodName> _entryPoints;
        private readonly CompletionTargetMarker _marker;
        private readonly CancellationToken _cancellationToken;

        public DeclarationVisitor(KaVELogger logger,
            ISet<IMethodName> entryPoints,
            CompletionTargetMarker marker,
            CancellationToken cancellationToken)
        {
            _logger = logger;
            _entryPoints = entryPoints;
            _marker = marker;
            _cancellationToken = cancellationToken;
        }

        public override void VisitNode(ITreeNode node, SST context)
        {
            _cancellationToken.ThrowIfCancellationRequested();
            node.Children<ICSharpTreeNode>().ForEach(child => child.Accept(this, context));
        }

        public override void VisitDelegateDeclaration(IDelegateDeclaration decl, SST context)
        {
            if (decl.DeclaredElement != null)
            {
                var sstDecl = new DelegateDeclaration {Name = decl.DeclaredElement.GetName<DelegateTypeName>()};
                context.Delegates.Add(sstDecl);
            }
        }

        public override void VisitEventDeclaration(IEventDeclaration decl,
            SST context)
        {
            if (decl.DeclaredElement != null)
            {
                var sstDecl = new EventDeclaration {Name = decl.DeclaredElement.GetName<IEventName>()};
                context.Events.Add(sstDecl);
            }
        }


        public override void VisitFieldDeclaration(IFieldDeclaration decl,
            SST context)
        {
            if (decl.DeclaredElement != null)
            {
                var sstDecl = new FieldDeclaration {Name = decl.DeclaredElement.GetName<IFieldName>()};
                context.Fields.Add(sstDecl);
            }
        }

        public override void VisitMethodDeclaration(IMethodDeclaration decl, SST context)
        {
            if (decl.DeclaredElement != null)
            {
                var methodName = decl.DeclaredElement.GetName<IMethodName>();
                _cancellationToken.ThrowIfCancellationRequested();

                var sstDecl = new MethodDeclaration
                {
                    Name = methodName,
                    IsEntryPoint = _entryPoints.Contains(methodName)
                };
                context.Methods.Add(sstDecl);

                if (decl == _marker.AffectedNode)
                {
                    sstDecl.Body.Add(new ExpressionStatement {Expression = new CompletionExpression()});
                }

                if (!decl.IsAbstract)
                {
                    var bodyVisitor = new BodyVisitor(_marker);

                    Execute.AndSupressExceptions(
                        delegate { decl.Accept(bodyVisitor, sstDecl.Body); });
                }
            }
        }

        public override void VisitPropertyDeclaration(IPropertyDeclaration decl, SST context)
        {
            if (decl.DeclaredElement != null)
            {
                var sstDecl = new PropertyDeclaration
                {
                    Name = decl.DeclaredElement.GetName<IPropertyName>()
                };
                context.Properties.Add(sstDecl);
                // TODO analyze getter/setter block
            }
        }
    }
}