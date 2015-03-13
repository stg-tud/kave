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

using System;
using KaVE.Model.SSTs.Impl;
using KaVE.VsFeedbackGenerator.Utils.Json;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils.Json.JsonSerializationSuite
{
    public class SSTSerializationTest
    {
        [Test]
        public void EntrypointAndNonEntryPointsAreNotSerialized()
        {
            // TODO write some tests for ST de-/serialization!
            var sst = new SST();
            // ReSharper disable once AssignNullToNotNullAttribute
            sst.EnclosingType = null;
            var json1 = sst.ToCompactJson();
            var json = "{}";
            var sst2 = json.ParseJsonTo<SST>();
            json = "{\"$type\":\"KaVE.Model.SSTs.Impl.SST, KaVE.Model\",\"EnclosingType\":null,\"Fields\":[],\"Properties\":[],\"Methods\":[],\"Events\":[],\"Delegates\":[]}";
            var sst3 = json.ParseJsonTo<SST>();
            Console.WriteLine();
        }
    }
}