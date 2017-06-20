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

using System.Collections.Generic;
using System.Linq;
using KaVE.FeedbackProcessor.Model;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace KaVE.FeedbackProcessor.Database
{
    internal class MongoDbDeveloperCollection : MongoDbDatabaseCollection<Developer>, IDeveloperCollection
    {
        public MongoDbDeveloperCollection(MongoCollection<Developer> collection) : base(collection) {}

        public IList<Developer> FindBySessionId(string sessionId)
        {
            var query = Query<Developer>.EQ(dev => dev.SessionIds, sessionId);
            return Collection.Find(query).ToList();
        }
    }
}