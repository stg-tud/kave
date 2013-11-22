using EnvDTE;
using JetBrains.Application;
using JetBrains.Application.Components;
using JetBrains.VsIntegration.Application;

namespace KaVE.VsFeedbackGenerator.VsIntegration
{
    [ShellComponent(ProgramConfigurations.VS_ADDIN)]
    public class VsDTE : IVsDTE
    {
        private readonly DTE _dte;

        public VsDTE(RawVsServiceProvider serviceProvider)
        {
            _dte = serviceProvider.Value.GetService<DTE, DTE>();
        }

        public void Quit()
        {
            _dte.Quit();
        }

        public object GetObject(string name)
        {
            return _dte.GetObject(name);
        }

        public Window OpenFile(string viewKind, string fileName)
        {
            return _dte.OpenFile(viewKind, fileName);
        }

        public void ExecuteCommand(string commandName, string commandArgs = "")
        {
            _dte.ExecuteCommand(commandName, commandArgs);
        }

        public wizardResult LaunchWizard(string vszFile, ref object[] contextParams)
        {
            return _dte.LaunchWizard(vszFile, contextParams);
        }

        public string SatelliteDllPath(string path, string name)
        {
            return _dte.SatelliteDllPath(path, name);
        }

        public string Name
        {
            get { return _dte.Name; }
        }

        public string FileName
        {
            get { return _dte.FileName; }
        }

        public string Version
        {
            get { return _dte.Version; }
        }

        public object CommandBars
        {
            get { return _dte.CommandBars; }
        }

        public Windows Windows
        {
            get { return _dte.Windows; }
        }

        public Events Events
        {
            get { return _dte.Events; }
        }

        public AddIns AddIns
        {
            get { return _dte.AddIns; }
        }

        public Window MainWindow
        {
            get { return _dte.MainWindow; }
        }

        public Window ActiveWindow
        {
            get { return _dte.ActiveWindow; }
        }

        public vsDisplay DisplayMode
        {
            get { return _dte.DisplayMode; }
            set { _dte.DisplayMode = value; }
        }

        public Solution Solution
        {
            get { return _dte.Solution; }
        }

        public Commands Commands
        {
            get { return _dte.Commands; }
        }

        public Properties get_Properties(string category, string page)
        {
            return _dte.Properties[category, page];
        }

        public SelectedItems SelectedItems
        {
            get { return _dte.SelectedItems; }
        }

        public string CommandLineArguments
        {
            get { return _dte.CommandLineArguments; }
        }

        public bool get_IsOpenFile(string viewKind, string fileName)
        {
            return _dte.IsOpenFile[viewKind, fileName];
        }

        public DTE DTE
        {
            get { return _dte.DTE; }
        }

        public int LocaleID
        {
            get { return _dte.LocaleID; }
        }

        public WindowConfigurations WindowConfigurations
        {
            get { return _dte.WindowConfigurations; }
        }

        public Documents Documents
        {
            get { return _dte.Documents; }
        }

        public Document ActiveDocument
        {
            get { return _dte.ActiveDocument; }
        }

        public Globals Globals
        {
            get { return _dte.Globals; }
        }

        public StatusBar StatusBar
        {
            get { return _dte.StatusBar; }
        }

        public string FullName
        {
            get { return _dte.FullName; }
        }

        public bool UserControl
        {
            get { return _dte.UserControl; }
            set { _dte.UserControl = value; }
        }

        public ObjectExtenders ObjectExtenders
        {
            get { return _dte.ObjectExtenders; }
        }

        public Find Find
        {
            get { return _dte.Find; }
        }

        public vsIDEMode Mode
        {
            get { return _dte.Mode; }
        }

        public ItemOperations ItemOperations
        {
            get { return _dte.ItemOperations; }
        }

        public UndoContext UndoContext
        {
            get { return _dte.UndoContext; }
        }

        public Macros Macros
        {
            get { return _dte.Macros; }
        }

        public object ActiveSolutionProjects
        {
            get { return _dte.ActiveSolutionProjects; }
        }

        public DTE MacrosIDE
        {
            get { return _dte.MacrosIDE; }
        }

        public string RegistryRoot
        {
            get { return _dte.RegistryRoot; }
        }

        public DTE Application
        {
            get { return _dte.Application; }
        }

        public ContextAttributes ContextAttributes
        {
            get { return _dte.ContextAttributes; }
        }

        public SourceControl SourceControl
        {
            get { return _dte.SourceControl; }
        }

        public bool SuppressUI
        {
            get { return _dte.SuppressUI; }
            set { _dte.SuppressUI = value; }
        }

        public Debugger Debugger
        {
            get { return _dte.Debugger; }
        }

        public string Edition
        {
            get { return _dte.Edition; }
        }
    }
}
