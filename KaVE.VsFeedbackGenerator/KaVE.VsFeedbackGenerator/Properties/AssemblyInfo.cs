using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.ActionManagement;
using JetBrains.Application.PluginSupport;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("KaVE Feedback Generator")]
[assembly: AssemblyDescription("Generates IDE events from the interaction of developers with VisualStudio and ReSharper 8 features.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("TU Darmstadt")]
[assembly: AssemblyProduct("KaVE.VsFeedbackGenerator")]
[assembly: AssemblyCopyright("Copyright © TU Darmstadt, 2013")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

[assembly: ActionsXml("KaVE.VsFeedbackGenerator.Actions.xml")]

// The following information is displayed by ReSharper in the Plugins dialog
[assembly: PluginTitle("KaVE Feedback Generator")]
[assembly: PluginDescription("Generates IDE events from the interaction of developers with VisualStudio and ReSharper 8 features.")]
[assembly: PluginVendor("TU Darmstadt")]

// Allow internal access for test projects
[assembly: InternalsVisibleTo("KaVE.VsFeedbackGenerator.Tests")]
[assembly: InternalsVisibleTo("KaVE.VsFeedbackGenerator.RS8Tests")]