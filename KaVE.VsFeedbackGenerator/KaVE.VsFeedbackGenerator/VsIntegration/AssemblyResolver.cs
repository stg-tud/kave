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
using System.Reflection;
using JetBrains.Application;

namespace KaVE.VsFeedbackGenerator.VsIntegration
{
    [ShellComponent]
    public class AssemblyResolver
    {
        static AssemblyResolver()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
        }

        /// <summary>
        ///     During deserialization of log files, loading the model assembly fails, when model types are used as type
        ///     parameters of type from other assemblies, e.g., in case of lists of model objects.
        /// </summary>
        private static Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assemblyName = args.Name;
            // This is required, because WpfAnimatedGif is loaded by the XamlParser from another assembly.
            // As a result, the resolving cannot consider the local references in resolving and fails.
            if (assemblyName.Equals("WpfAnimatedGif"))
            {
                return typeof (WpfAnimatedGif.ImageBehavior).Assembly;
            }
            return null;
        }
    }
}
