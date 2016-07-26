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
using System.Collections.Generic;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Utils.Naming;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KaVE.Commons.Utils.Json
{
    internal class ProposalCollectionConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }

        public override object ReadJson(JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                var proposalCollection = JObject.Load(reader);
                var proposals = proposalCollection.GetValue("Proposals");

                var res = new ProposalCollection();
                foreach (var proposal in proposals)
                {
                    var p = new Proposal();

                    var propObj = (JObject) proposal;
                    var valName = propObj.GetValue("Name") as JValue;
                    if (valName != null)
                    {
                        var name = valName.Value as string;
                        if (name != null)
                        {
                            p.Name = name.Deserialize<IName>();
                        }
                    }
                    else
                    {
                        p.Name = Names.UnknownGeneral;
                    }
                    var valRelevance = propObj.GetValue("Relevance") as JValue;
                    if (valRelevance != null)
                    {
                        var relevance = (long) valRelevance.Value;
                        p.Relevance = unchecked((int) relevance);
                    }

                    res.Add(p);
                }
                return res;
            }
            if (reader.TokenType == JsonToken.StartArray)
            {
                return new ProposalCollection(serializer.Deserialize<IEnumerable<Proposal>>(reader));
            }
            throw new JsonSerializationException("expected either array or object to deserialize proposal collection");
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IProposalCollection).IsAssignableFrom(objectType);
        }
    }
}