using System;
using EnvDTE;
using JetBrains.Application;
using KaVE.EventGenerator.ReSharper8.VsIntegration;

namespace KaVE.EventGenerator.ReSharper8.IntegrationTests.VsIntegrationMocks
{
    [ShellComponent]
    public class TestDTE : IVsDTE
    {
        public DTE DTE { get; private set; }
    }
}
