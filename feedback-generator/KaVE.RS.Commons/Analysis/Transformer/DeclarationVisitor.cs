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
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Model.SSTs.Expressions;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.Exceptions;
using KaVE.RS.Commons.Analysis.CompletionTarget;
using KaVE.RS.Commons.Analysis.Util;
using KaVE.RS.Commons.Utils.Naming;
using KaVELogger = KaVE.Commons.Utils.Exceptions.ILogger;
using IKaVEStatement = KaVE.Commons.Model.SSTs.IStatement;

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

        #region avoid stepping into nested types

        private bool _isAlreadyInsideTypeDeclaration;

        public override void VisitClassDeclaration(IClassDeclaration decl, SST context)
        {
            if (!_isAlreadyInsideTypeDeclaration)
            {
                _isAlreadyInsideTypeDeclaration = true;
                base.VisitClassDeclaration(decl, context);
                _isAlreadyInsideTypeDeclaration = false;
            }
        }

        public override void VisitEnumDeclaration(IEnumDeclaration decl, SST context)
        {
            if (!_isAlreadyInsideTypeDeclaration)
            {
                _isAlreadyInsideTypeDeclaration = true;
                base.VisitEnumDeclaration(decl, context);
                _isAlreadyInsideTypeDeclaration = false;
            }
        }

        public override void VisitInterfaceDeclaration(IInterfaceDeclaration decl, SST context)
        {
            if (!_isAlreadyInsideTypeDeclaration)
            {
                _isAlreadyInsideTypeDeclaration = true;
                base.VisitInterfaceDeclaration(decl, context);
                _isAlreadyInsideTypeDeclaration = false;
            }
        }

        public override void VisitStructDeclaration(IStructDeclaration decl, SST context)
        {
            if (!_isAlreadyInsideTypeDeclaration)
            {
                _isAlreadyInsideTypeDeclaration = true;
                base.VisitStructDeclaration(decl, context);
                _isAlreadyInsideTypeDeclaration = false;
            }
        }

        #endregion

        public override void VisitDelegateDeclaration(IDelegateDeclaration decl, SST context)
        {
            if (decl.DeclaredElement != null)
            {
                var name = decl.DeclaredElement.GetName<IDelegateTypeName>();

                context.Delegates.Add(new DelegateDeclaration {Name = name});
            }
        }

        public override void VisitEnumMemberDeclaration(IEnumMemberDeclaration decl, SST context)
        {
            var shortName = decl.DeclaredName;
            context.Fields.Add(
                new FieldDeclaration {Name = Names.Field("[{0}] [{0}].{1}", context.EnclosingType, shortName)});
        }

        public override void VisitEventDeclaration(IEventDeclaration decl,
            SST context)
        {
            if (decl.DeclaredElement != null)
            {
                var name = decl.DeclaredElement.GetName<IEventName>();

                context.Events.Add(new EventDeclaration {Name = name});
            }
        }


        public override void VisitFieldDeclaration(IFieldDeclaration decl,
            SST context)
        {
            if (decl.DeclaredElement != null)
            {
                var name = decl.DeclaredElement.GetName<IFieldName>();

                context.Fields.Add(new FieldDeclaration {Name = name});
            }
        }

        public override void VisitConstantDeclaration(IConstantDeclaration decl, SST context)
        {
            if (decl.DeclaredElement != null)
            {
                var name = decl.DeclaredElement.GetName<IFieldName>();

                context.Fields.Add(new FieldDeclaration {Name = name});
            }
        }

        public override void VisitConstructorDeclaration(IConstructorDeclaration decl, SST context)
        {
            _cancellationToken.ThrowIfCancellationRequested();

            var nameGen = new UniqueVariableNameGenerator();
            var exprVisit = new ExpressionVisitor(nameGen, _marker);

            if (decl.DeclaredElement != null)
            {
                var methodName = decl.DeclaredElement.GetName<IMethodName>();

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

                if (decl.Initializer != null)
                {
                    var name = Names.UnknownMethod;

                    var substitution = decl.DeclaredElement.IdSubstitution;
                    var resolvedRef = decl.Initializer.Reference.Resolve();
                    if (resolvedRef.DeclaredElement != null)
                    {
                        name = resolvedRef.DeclaredElement.GetName<IMethodName>(substitution);
                    }

                    var args = Lists.NewList<ISimpleExpression>();
                    foreach (var p in decl.Initializer.Arguments)
                    {
                        var expr = exprVisit.ToSimpleExpression(p.Value, sstDecl.Body);
                        args.Add(expr);
                    }

                    var varId = new VariableReference().Identifier; // default value
                    if (decl.Initializer.Instance != null)
                    {
                        var tokenType = decl.Initializer.Instance.GetTokenType();
                        var isThis = CSharpTokenType.THIS_KEYWORD == tokenType;
                        var isBase = CSharpTokenType.BASE_KEYWORD == tokenType;

                        varId = isThis ? "this" : isBase ? "base" : varId;
                    }

                    sstDecl.Body.Add(
                        new ExpressionStatement
                        {
                            Expression = new InvocationExpression
                            {
                                Reference = new VariableReference {Identifier = varId},
                                MethodName = name,
                                Parameters = args
                            }
                        });
                }

                if (!decl.IsAbstract)
                {
                    var bodyVisitor = new BodyVisitor(nameGen, _marker);

                    Execute.AndSupressExceptions(
                        delegate { decl.Accept(bodyVisitor, sstDecl.Body); });
                }
            }
        }

        public override void VisitMethodDeclaration(IMethodDeclaration decl, SST context)
        {
            _cancellationToken.ThrowIfCancellationRequested();

            if (decl.DeclaredElement != null)
            {
                var methodName = decl.DeclaredElement.GetName<IMethodName>();

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
                    var bodyVisitor = new BodyVisitor(new UniqueVariableNameGenerator(), _marker);

                    Execute.AndSupressExceptions(
                        delegate { decl.Accept(bodyVisitor, sstDecl.Body); });
                }
            }
        }

        public override void VisitPropertyDeclaration(IPropertyDeclaration decl, SST context)
        {
            if (decl.DeclaredElement != null)
            {
                var name = decl.DeclaredElement.GetName<IPropertyName>();

                var propDecl = new PropertyDeclaration {Name = name};
                context.Properties.Add(propDecl);

                AddAccessorDecl(decl, propDecl);
            }
        }

        public override void VisitIndexerDeclaration(IIndexerDeclaration decl, SST sst)
        {
            var name = decl.DeclaredElement.GetName<IPropertyName>();

            var propDecl = new PropertyDeclaration {Name = name};
            sst.Properties.Add(propDecl);

            AddAccessorDecl(decl, propDecl);
        }

        private void AddAccessorDecl(IAccessorOwnerDeclaration decl,
            KaVE.Commons.Model.SSTs.Declarations.IPropertyDeclaration propDecl)
        {
            foreach (var accessor in decl.AccessorDeclarations)
            {
                var bodyVisitor = new BodyVisitor(new UniqueVariableNameGenerator(), _marker);
                var body = Lists.NewList<IKaVEStatement>();

                if (accessor.Kind == AccessorKind.GETTER)
                {
                    body = propDecl.Get;
                }
                if (accessor.Kind == AccessorKind.SETTER)
                {
                    body = propDecl.Set;
                }

                accessor.Accept(bodyVisitor, body);
            }
        }
    }
}