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
 *    - Sebastian Proksch
 */

using KaVE.Model.Names;
using KaVE.Model.Names.CSharp;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite
{
    internal class SSTAnalysisFixture
    {
        internal static readonly ITypeName Int = TypeName.Get("System.Int32, mscorlib, 4.0.0.0");
        internal static readonly ITypeName IntArray = TypeName.Get("..., mscorlib, 4.0.0.0");
        internal static readonly ITypeName Void = TypeName.Get("System.Void, mscorlib, 4.0.0.0");
        internal static readonly ITypeName Unknown = UnknownTypeName.Instance;
        internal static readonly ITypeName String = TypeName.Get("System.String, mscorlib, 4.0.0.0");
        internal static readonly ITypeName Bool = TypeName.Get("System.Boolean, mscorlib, 4.0.0.0");
        internal static readonly ITypeName Object = TypeName.Get("System.Object, mscorlib, 4.0.0.0");
        internal static readonly ITypeName Exception = TypeName.Get("System.Exception, mscorlib, 4.0.0.0");

        public static IMethodName GetMethodName(string cGet)
        {
            return null;
        }
    }
}