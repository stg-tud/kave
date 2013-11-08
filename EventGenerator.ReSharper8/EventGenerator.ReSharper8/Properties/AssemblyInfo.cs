using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.ActionManagement;
using JetBrains.Application.PluginSupport;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("EventGenerator.ReSharper8")]
[assembly: AssemblyDescription("Generates IDEEvents from the interaction of developers with ReSharper 8 features.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("KAVE")]
[assembly: AssemblyProduct("EventGenerator.ReSharper8")]
[assembly: AssemblyCopyright("Copyright © KAVE, 2013")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

[assembly: ActionsXml("EventGenerator.ReSharper8.Actions.xml")]

// The following information is displayed by ReSharper in the Plugins dialog
[assembly: PluginTitle("EventGenerator.ReSharper8")]
[assembly: PluginDescription("Generates IDEEvents from the interaction of developers with ReSharper 8 features.")]
[assembly: PluginVendor("KAVE")]

[assembly: InternalsVisibleTo("EventGenerator.ReSharper8.Tests")]
[assembly: InternalsVisibleTo("EventGenerator.ReSharper8.IntegrationTests")]