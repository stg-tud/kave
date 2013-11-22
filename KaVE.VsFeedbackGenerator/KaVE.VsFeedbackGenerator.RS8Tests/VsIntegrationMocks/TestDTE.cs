using System;
using EnvDTE;
using JetBrains.Application;
using KaVE.EventGenerator.ReSharper8.VsIntegration;

namespace KaVE.EventGenerator.ReSharper8.IntegrationTests.VsIntegrationMocks
{
    [ShellComponent]
    public class TestDTE : IVsDTE
    {
        public void Quit()
        {
            throw new NotImplementedException();
        }

        public object GetObject(string name)
        {
            throw new NotImplementedException();
        }

        public Window OpenFile(string viewKind, string fileName)
        {
            throw new NotImplementedException();
        }

        public void ExecuteCommand(string commandName, string commandArgs = "")
        {
            throw new NotImplementedException();
        }

        public wizardResult LaunchWizard(string vszFile, ref object[] contextParams)
        {
            throw new NotImplementedException();
        }

        public string SatelliteDllPath(string path, string name)
        {
            throw new NotImplementedException();
        }

        public string Name { get; private set; }
        public string FileName { get; private set; }
        public string Version { get; private set; }
        public object CommandBars { get; private set; }
        public Windows Windows { get; private set; }
        public Events Events { get; private set; }
        public AddIns AddIns { get; private set; }
        public Window MainWindow { get; private set; }
        public Window ActiveWindow { get; private set; }
        public vsDisplay DisplayMode { get; set; }
        public Solution Solution { get; private set; }
        public Commands Commands { get; private set; }
        public Properties get_Properties(string category, string page)
        {
            throw new NotImplementedException();
        }

        public SelectedItems SelectedItems { get; private set; }
        public string CommandLineArguments { get; private set; }
        public bool get_IsOpenFile(string viewKind, string fileName)
        {
            throw new NotImplementedException();
        }

        public DTE DTE { get; private set; }
        public int LocaleID { get; private set; }
        public WindowConfigurations WindowConfigurations { get; private set; }
        public Documents Documents { get; private set; }
        public Document ActiveDocument { get; private set; }
        public Globals Globals { get; private set; }
        public StatusBar StatusBar { get; private set; }
        public string FullName { get; private set; }
        public bool UserControl { get; set; }
        public ObjectExtenders ObjectExtenders { get; private set; }
        public Find Find { get; private set; }
        public vsIDEMode Mode { get; private set; }
        public ItemOperations ItemOperations { get; private set; }
        public UndoContext UndoContext { get; private set; }
        public Macros Macros { get; private set; }
        public object ActiveSolutionProjects { get; private set; }
        public DTE MacrosIDE { get; private set; }
        public string RegistryRoot { get; private set; }
        public DTE Application { get; private set; }
        public ContextAttributes ContextAttributes { get; private set; }
        public SourceControl SourceControl { get; private set; }
        public bool SuppressUI { get; set; }
        public Debugger Debugger { get; private set; }
        public string Edition { get; private set; }
    }
}
