To build/run this project, install the command-line tools (CLT) for RS8:

 * Install Chocolate (https://chocolatey.org/) with
   $> @powershell -NoProfile -ExecutionPolicy unrestricted -Command "iex ((new-object net.webclient).DownloadString('https://chocolatey.org/install.ps1'))" && SET PATH=%PATH%;%ALLUSERSPROFILE%\chocolatey\bin
 * Install CLT with
   $> choco install resharper-clt.portable -version 8.2.0.2151

Copy the DLLs required to build/run this project from the install directory to './RSEnvLibs/':

 * CookComputing.XmlRpcV2.dll
 * DevExpress.Data.v7.1.dll
 * DevExpress.Utils.v7.1.dll
 * DevExpress.XtraEditors.v7.1.dll
 * DevExpress.XtraTreeList.v7.1.dll
 * JetBrains.CommandLine.Common.Console.dll
 * JetBrains.CommandLine.Common.dll
 * JetBrains.Platform.ReSharper.ActivityTracking.dll
 * JetBrains.Platform.ReSharper.ComponentModel.dll
 * JetBrains.Platform.ReSharper.DocumentModel.dll
 * JetBrains.Platform.ReSharper.IDE.dll
 * JetBrains.Platform.ReSharper.Interop.WinApi.dll
 * JetBrains.Platform.ReSharper.Metadata.dll
 * JetBrains.Platform.ReSharper.ProjectModel.dll
 * JetBrains.Platform.ReSharper.Shell.dll
 * JetBrains.Platform.ReSharper.Shell.Extensions.dll
 * JetBrains.Platform.ReSharper.UI.dll
 * JetBrains.Platform.ReSharper.Util.dll
 * JetBrains.ReSharper.Resources.dll
 * NuGet.Core.dll
 * System.Threading.dll