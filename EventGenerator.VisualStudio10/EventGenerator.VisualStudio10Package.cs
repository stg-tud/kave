using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using EventGenerator.Commons;
using KAVE.EventGenerator_VisualStudio10.Annotations;
using KAVE.EventGenerator_VisualStudio10.Generators;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;

namespace KAVE.EventGenerator_VisualStudio10
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the informations needed to show the this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string)]
    [Guid(GuidList.guidEventGenerator_VisualStudio10PkgString)]
    public sealed class EventGenerator_VisualStudio10Package : Package
    {
        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public EventGenerator_VisualStudio10Package()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this));
        }

        [ImportMany(typeof(VisualStudioEventGenerator)), UsedImplicitly]
        private VisualStudioEventGenerator[] _eventGenerators;

        /////////////////////////////////////////////////////////////////////////////
        // Overriden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initilaization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Trace.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this));
            base.Initialize();
            var componentModel = (IComponentModel) GetService(typeof(SComponentModel));
            componentModel.DefaultCompositionService.SatisfyImportsOnce(this);
        }
        #endregion

    }
}
