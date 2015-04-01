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
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.VsFeedbackGenerator.Utils.Names;

namespace KaVE.VsFeedbackGenerator.Analysis.Transformer
{
    public class DeclarationVisitor : TreeNodeVisitor<SST>
    {
        private readonly ISet<IMethodName> _entryPoints;
        private readonly CompletionTargetAnalysis.TriggerPointMarker _marker;

        public DeclarationVisitor(ISet<IMethodName> entryPoints, CompletionTargetAnalysis.TriggerPointMarker marker)
        {
            _entryPoints = entryPoints;
            _marker = marker;
        }

        public override void VisitNode(ITreeNode node, SST context)
        {
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
                var sstDecl = new MethodDeclaration
                {
                    Name = methodName,
                    IsEntryPoint = _entryPoints.Contains(methodName)
                };
                context.Methods.Add(sstDecl);

                if (!decl.IsAbstract)
                {
                    //var bodyVisitor = new BodyVisitor(_marker);
                    //decl.Accept(bodyVisitor, sstDecl.Body);
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