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
using JetBrains.ReSharper.Feature.Services.CSharp.CodeCompletion.Infrastructure;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
using KaVE.Model.Names;
using KaVE.Model.SSTs;
using KaVE.Model.SSTs.Declarations;
using KaVE.VsFeedbackGenerator.Analysis.Transformer;
using KaVE.VsFeedbackGenerator.Analysis.Util;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.Utils.Names;
using ILogger = KaVE.VsFeedbackGenerator.Generators.ILogger;

namespace KaVE.VsFeedbackGenerator.Analysis
{
    public class SSTAnalysis
    {
        public static SST Analyze(CSharpCodeCompletionContext rsContext,
            ITypeDeclaration typeDeclaration,
            ISet<MethodRef> entryPoints,
            ISSTFactory factory)
        {
            var sst = new SST();


            var epNames = new HashSet<IMethodName>();

            foreach (var ep in entryPoints)
            {
                var decl = new MethodDeclaration {Name = ep.Name, IsEntryPoint = true};
                sst.Methods.Add(decl);
                epNames.Add(ep.Name);
            }

            var allDecls = typeDeclaration.MemberDeclarations.Select(m => m as IMethodDeclaration).Where(m => m != null);
            foreach (var d in allDecls)
            {
                if (d.DeclaredElement != null)
                {
                    var dElem = d.DeclaredElement;
                    var dName = dElem.GetName<IMethodName>();

                    MethodDeclaration decl;

                    if (!epNames.Contains(dName))
                    {
                        decl = new MethodDeclaration {Name = dName, IsEntryPoint = false};
                        sst.Methods.Add(decl);
                    }
                    else
                    {
                        decl = sst.Methods.First(ep => ep.Name == dName);
                    }

                    var context = new ScopeTransformerContext(factory, Registry.GetComponent<ILogger>());
                    decl.Body.AddRange(d.GetScope(context).Body);

                    //var dRef = MethodRef.CreateLocalReference(dName, dElem, dElem.GetDeclaration());
                }
            }

            return sst;
        }
    }
}