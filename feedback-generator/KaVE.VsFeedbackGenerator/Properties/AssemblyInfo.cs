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

using System.Reflection;
using System.Runtime.CompilerServices;
using KaVE.VsFeedbackGenerator;
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

[assembly: AssemblyTitle(RSEnv.ExtensionId)]
[assembly:
    AssemblyDescription(
        "Generates IDE events from the interaction of developers with VisualStudio and ReSharper 8 features.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("TU Darmstadt")]
[assembly: AssemblyProduct(RSEnv.ExtensionId)]
[assembly: AssemblyCopyright("Copyright © TU Darmstadt, 2014")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

// TODO RS9
/*[assembly: ActionsXml("KaVE.VsFeedbackGenerator.Actions.xml")]

// The following information is displayed by ReSharper in the Plugins dialog
[assembly: PluginTitle(RSEnv.ExtensionId)]
[assembly: PluginDescription("Generates IDE events from the interaction of developers with VisualStudio and ReSharper 8 features.")]
[assembly: PluginVendor("TU Darmstadt")]
*/

// Allow internal access for test projects

[assembly: InternalsVisibleTo("KaVE.VsFeedbackGenerator.Tests")]
[assembly: InternalsVisibleTo("KaVE.VsFeedbackGenerator.RS8Tests")]