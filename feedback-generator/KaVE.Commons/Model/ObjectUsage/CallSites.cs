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

using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Utils.Assertion;

namespace KaVE.Commons.Model.ObjectUsage
{
    public class CallSites
    {
        public static CallSite CreateReceiverCallSite(string methodName)
        {
            Asserts.NotNull(methodName);

            var callSite = new CallSite
            {
                kind = CallSiteKind.RECEIVER, 
                method = new CoReMethodName(methodName)
            };

            return callSite;
        }

        public static CallSite CreateReceiverCallSite(IMethodName methodName)
        {
            Asserts.NotNull(methodName);
            return CreateReceiverCallSite(methodName.ToCoReName().Name);
        }

        public static CallSite CreateParameterCallSite(string methodName, int argIndex)
        {
            Asserts.NotNull(methodName);

            var callSite = new CallSite
            {
                kind = CallSiteKind.PARAMETER,
                method = new CoReMethodName(methodName),
                argIndex = argIndex
            };

            return callSite;
        }
    }
}