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
 *    - Roman Fojtik
 *    - Sebastian Proksch
 */

using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp;

namespace KaVE.Commons.Tests.Utils.ObjectUsageExport.UsageExtractorTestSuite
{
    internal class ObjectUsageExporterTestFixture
    {
        public static ITypeName Void = TypeName.Get("System.Void, mscorlib, 4.0.0.0");
        public static ITypeName Int = TypeName.Get("System.Int32, mscore, 4.0.0.0");

        public static IParameterName IntParam(string paramName)
        {
            return ParameterName.Get("[" + Int + "] " + paramName);
        }

        // ReSharper disable once InconsistentNaming
        public static IMethodName M_GetHashCode =
            MethodName.Get("[System.Int32, mscorlib, 4.0.0.0] [System.Object, mscorlib, 4.0.0.0].GetHashCode()");
    }
}