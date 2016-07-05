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

using System;

// ReSharper disable once CheckNamespace
namespace Test.Targets
{
    internal class SimpleType
    {
        public static string _staticField = "";

        public static int StaticProperty { get; set; }

        public static void StaticMethod() {}

        public readonly int _simpleTypedField = 0;

        public string SimpleProperty { get; set; }

        public int GetOnlyProperty
        {
            get { return 0; }
        }

        public int SetOnlyProperty
        {
            // ReSharper disable once ValueParameterNotUsed
            set { }
        }

        public int PrivateSetProperty { get; private set; }

        public int SimpleMethod()
        {
            return 0;
        }

        public void ParameterizedMethod(int firstParam, Object secondParam) {}

        public void ParamsMethod(params int[] varArgs) {}

        public void RefParamMethod(ref int primitiveInOut) {}

        public void OutParamMethod(out int primitiveOut)
        {
            primitiveOut = 0;
        }

        public void OptionalParamMethod(int optional = 0) {}

        public delegate void SomeEventHandler(int param);

        public event SomeEventHandler SomeEvent;

        public static event SomeEventHandler StaticEvent;
    }
}