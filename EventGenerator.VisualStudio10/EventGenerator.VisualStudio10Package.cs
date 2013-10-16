using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using KAVE.EventGenerator_VisualStudio10.Generators;
using KAVE.KAVE_MessageBus.MessageBus;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Ninject;
using Ninject.Activation;
using Ninject.Activation.Providers;
using Ninject.Components;
using Ninject.Extensions.Conventions;
using Ninject.Infrastructure;
using Ninject.Modules;
using Ninject.Planning.Bindings;
using Ninject.Planning.Bindings.Resolvers;

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
    [PackageRegistration(UseManagedResourcesOnly = true),
     InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400),
     ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string), Guid(GuidList.guidEventGenerator_VisualStudio10PkgString)
    ]
    // This attribute is used to register the informations needed to show the this package
    // in the Help/About dialog of Visual Studio.
    // ReSharper disable once InconsistentNaming
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

        /////////////////////////////////////////////////////////////////////////////
        // Overriden Package Implementation

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initilaization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this));
            base.Initialize();
            InjectWithIndividualCompositionContainer();
        }

        private void InjectWithIndividualCompositionContainer()
        {
            using(var kernel = new StandardKernel(
                    new NinjectSettings
                    {
                        InjectAttribute = typeof (CodeCompletion.Utils.InjectAttribute),
                        AllowNullInjection = false
                    }))
            {
                kernel.Components.Add<IMissingBindingResolver, VsServiceResolver>();
                kernel.Load<VisualStudioEventGeneratorModule>();
            }
        }

        #endregion
    }

    internal class VisualStudioEventGeneratorModule : NinjectModule
    {
        [UsedImplicitly] private VisualStudioEventGenerator[] _visualStudioEventGenerators;

        public override void Load()
        {
            Kernel.Bind(
                x =>
                    x.FromThisAssembly()
                        .IncludingNonePublicTypes()
                        .SelectAllClasses()
                        .InheritedFrom<VisualStudioEventGenerator>()
                        .BindBase()
                        .Configure(
                            b =>
                                b.InSingletonScope()
                                    .OnActivation(
                                        (cxt, gen) => ((VisualStudioEventGenerator) gen).Initialize())));

            // eagerly load all generators
            _visualStudioEventGenerators = Kernel.GetAll<VisualStudioEventGenerator>().ToArray();
        }
    }

    [UsedImplicitly]
    internal class VsServiceResolver : NinjectComponent, IMissingBindingResolver
    {
        public IEnumerable<IBinding> Resolve(Multimap<Type, IBinding> bindings, IRequest request)
        {
            var requestedType = request.Service;
            if (IsVsServiceInterface(requestedType))
            {
                return new[]
                {
                    new Binding(requestedType)
                    {
                        ProviderCallback =
                            context =>
                            {
                                var service = Package.GetGlobalService(requestedType);
                                return new ConstantProvider<object>(service);
                            }
                    }
                };
            }
            return Enumerable.Empty<IBinding>();
        }

        private static bool IsVsServiceInterface(Type requestedType)
        {
            return requestedType.IsInterface &&
                   (requestedType.Name.StartsWith("S") || requestedType.Name.StartsWith("DTE"));
        }
    }
}