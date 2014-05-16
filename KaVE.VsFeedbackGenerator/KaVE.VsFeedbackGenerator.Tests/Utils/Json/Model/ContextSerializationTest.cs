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

using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names;
using KaVE.TestUtils.Model.Names;
using KaVE.VsFeedbackGenerator.Utils.Json;
using Newtonsoft.Json;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils.Json.Model
{
    [TestFixture]
    internal class ContextSerializationTest
    {
        [Test]
        public void ShouldSerializeEmptyContext()
        {
            var context = new Context();
            JsonAssert.SerializationPreservesData(context);
        }

        [Test]
        public void ShouldSerializeAllFieldsOfContext()
        {
            var context = new Context
            {
                TypeShape = new TypeShape
                {
                    TypeHierarchy = GetAnonymousTypeHierarchy(),
                    MethodHierarchies = new HashSet<MethodHierarchy>
                    {
                        new MethodHierarchy(TestNameFactory.GetAnonymousMethodName())
                        {
                            First = TestNameFactory.GetAnonymousMethodName(),
                            Super = TestNameFactory.GetAnonymousMethodName(),
                        }
                    }
                },
                EnclosingMethod = TestNameFactory.GetAnonymousMethodName(),
                EntryPointsToCalledMethods = new Dictionary<IMethodName, ISet<IMethodName>>
                {
                    {
                        TestNameFactory.GetAnonymousMethodName(), new HashSet<IMethodName>
                        {
                            TestNameFactory.GetAnonymousMethodName(),
                            TestNameFactory.GetAnonymousMethodName(),
                            TestNameFactory.GetAnonymousMethodName()
                        }
                    },
                    {
                        TestNameFactory.GetAnonymousMethodName(), new HashSet<IMethodName>
                        {
                            TestNameFactory.GetAnonymousMethodName()
                        }
                    }
                },
                TriggerTarget = TestNameFactory.GetAnonymousTypeName()
            };
            JsonAssert.SerializationPreservesData(context);
        }

        private static TypeHierarchy GetAnonymousTypeHierarchy()
        {
            return new TypeHierarchy(TestNameFactory.GetAnonymousTypeName().Identifier);
        }
    }
}