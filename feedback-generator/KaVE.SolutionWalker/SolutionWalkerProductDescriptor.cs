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

using System;
using System.Drawing;
using System.Reflection;
using JetBrains.Application.ExceptionReport;
using JetBrains.ReSharper;

namespace KaVE.SolutionWalker
{
    public class SolutionWalkerProductDescriptor : ReSharperApplicationDescriptor
    {
        public override Assembly AllAssembliesResourceAssembly
        {
            get { return GetType().Assembly; }
        }

        public override string AllAssembliesResourceName
        {
            get { return "KaVE.SolutionWalker.AllAssemblies.xml"; }
        }

        public override IIssueTracker IssueTracker
        {
            get { return null; }
        }

        public override Icon ProductIcon
        {
            get { return SystemIcons.Application; }
        }

        public override Uri ProductURL
        {
            get { return new Uri("http://kave.cc/"); }
        }

        public override string ProductName
        {
            get { return "KaVE-SolutionWalker"; }
        }

        //public override Version ProductVersion
        //{
        //    get { return new Version(8, 2, 0, 2151); }
        //}

        public override string ProductFullName
        {
            get { return ProductName + " " + ProductVersion; }
        }
    }
}