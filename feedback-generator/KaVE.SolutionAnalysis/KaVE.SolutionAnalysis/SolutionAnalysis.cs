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
 *    - Sven Amann
 */

using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Text;
using JetBrains.Util;

namespace KaVE.SolutionAnalysis
{
    [SolutionComponent]
    public class SolutionAnalysis
    {
        public static readonly IList<string> AnalyzedProjects = new List<string>();
        public static readonly IList<string> AnalyzedFiles = new List<string>();
        public static readonly IList<string> AnalyzedClasses = new List<string>();

        private readonly ISolution _solution;

        public SolutionAnalysis(ISolution solution)
        {
            _solution = solution;
            var projects = solution.GetAllProjects().Where(NotDefaultProject);
            projects.ForEach(AnalyzeProject);
        }


        private void AnalyzeProject(IProject project)
        {
            AnalyzedProjects.Add(project.Name);
            Directory.GetFiles(project.Location.FullPath, "*.cs", SearchOption.AllDirectories)
                     .ForEach(file => AnalyzeFile(file, project));
        }

        private void AnalyzeFile(string file, IProject project)
        {
            AnalyzedFiles.Add(file);
            var languageService = CSharpLanguage.Instance.LanguageService();
            ILexer lexer = languageService.CreateCachingLexer(new StringBuffer(File.ReadAllText(file)));
            var primaryPsiModule = _solution.PsiModules().GetPrimaryPsiModule(project);
            var csharpParser = (ICSharpParser) languageService.CreateParser(lexer, primaryPsiModule, null);
            csharpParser.ExpandChameleons = csharpParser.OpenChameleonStrategy;
            var psiFile = (ICSharpFile)csharpParser.ParseFile();
            psiFile.TypeDeclarations.ForEach(AnalyzeType);
        }

        private void AnalyzeType(ICSharpTypeDeclaration aType)
        {
            AnalyzedClasses.Add(aType.CLRName);
        }

        private bool NotDefaultProject(IProject project)
        {
            return !project.Name.Equals("Miscellaneous Files") && !project.Name.Equals("&");
        }
    }
}