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
    public class MethodNameTestCase : ITestCase
    {
        public string Identifier { get; set; }
        public string DeclaringType { get; set; }
        public string ReturnType { get; set; }
        public string SimpleName { get; set; }
        public bool IsStatic { get; set; }
        public bool HasTypeParameters { get; set; }
        public IKaVEList<string> Parameters { get; set; }
        public IKaVEList<string> TypeParameters { get; set; }
    }
}