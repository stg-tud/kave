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
 */

using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Types;

namespace KaVE.Commons.Tests.Utils.ObjectUsageExport.UsageExtractorTestSuite
{
    internal class ObjectUsageExporterTestFixture
    {
        public static ITypeName Void = Names.Type("p:void");
        public static ITypeName Int = Names.Type("p:int");

        internal static readonly ITypeName Bool = Names.Type("p:bool");

        internal static readonly ITypeName Object = Names.Type("p:object");

        internal static readonly ITypeName Action = Names.Type("d:[p:void] [System.Action, mscorlib, 4.0.0.0].()");

        internal static readonly ITypeName ActionOfInt =
            Names.Type(
                "d:[p:void] [System.Action`1[[T -> p:int]], mscorlib, 4.0.0.0].([T] obj)");

        public static IParameterName IntParam(string paramName)
        {
            return Names.Parameter("[" + Int + "] " + paramName);
        }

        // ReSharper disable once InconsistentNaming
        public static IMethodName Int_GetHashCode = Names.Method("[p:int] [p:object].GetHashCode()");

        // ReSharper disable once InconsistentNaming
        internal static readonly IMethodName Object_Equals =
            Names.Method(string.Format("[{0}] [{1}].Equals([{1}] obj)", Bool, Object));
    }
}