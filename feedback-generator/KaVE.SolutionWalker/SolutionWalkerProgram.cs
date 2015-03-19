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

using System.Threading;
using JetBrains.Application;
using JetBrains.CommandLine.Common;

namespace KaVE.SolutionWalker
{
    public static class SolutionWalkerProgram
    {
        public static int Main(string[] args)
        {
            var returnCode = 0;
            var thread = new Thread(() => returnCode = Startup(args));
            thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            thread.Start();
            thread.Join();
            return returnCode;
        }

        private static int Startup(string[] args)
        {
            return ConsoleApplicationHost.Main(
                () => (IApplicationDescriptor) new SolutionWalkerProductDescriptor(),
                args);
        }
    }
}