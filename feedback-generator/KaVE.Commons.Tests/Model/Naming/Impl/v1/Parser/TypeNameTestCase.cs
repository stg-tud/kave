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

using KaVE.Commons.Utils.Collections;

namespace KaVE.Commons.Tests.Model.Naming.Impl.v1.Parser
{
    public class TypeNameTestCase : ITestCase
    {
        public string Identifier { get; set; }
        public string Assembly { get; set; }
        public string Namespace { get; set; }
        public string DeclaringType { get; set; }
        public string FullName { get; set; }
        public string Name { get; set; }
        public string ArrayBaseType { get; set; }
        public string TypeParameterType { get; set; }
        public string TypeParameterShortName { get; set; }
        public IKaVEList<string> TypeParameters { get; set; }
        public bool IsReferenceType { get; set; }
        public bool IsClassType { get; set; }
        public bool IsInterfaceType { get; set; }
        public bool IsEnumType { get; set; }
        public bool IsStructType { get; set; }
        public bool IsNestedType { get; set; }
    }
}