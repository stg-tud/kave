using System;
using System.Reflection;
using JetBrains.Application;
using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.Utils.Json;

namespace KaVE.VsFeedbackGenerator.Utils
{
    [ShellComponent]
    public class TypeResolutionExceptionTest
    {
        /// <summary>
        /// This type resolution fails when the project is run as a R# plugin. The same resolution succeeds in the test
        /// project. This is an open issue, preventing our deserialization for the Session Manager. A respective
        /// question has been posted at Stackoverflow.com and in the R# forums. <see cref="MyTypeNameBinderForDebugging"/>
        /// </summary>
        public TypeResolutionExceptionTest()
        {
            const string assemblyName = "mscorlib";
            const string typeName = "System.Collections.Generic.List`1[[KaVE.Model.Events.CompletionEvent.ProposalSelection, KaVE.Model]]";
            Assembly assembly = Assembly.LoadWithPartialName(assemblyName);
            // TODO find a fix or workaround for this problem!
            //Type type = assembly.GetType(typeName, true);
            //Asserts.NotNull(type, "could not load {0} from assembly {1}.", typeName, assemblyName);
        }
    }
}