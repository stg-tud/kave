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
using KaVE.Commons.Model.Events;
using KaVE.FeedbackProcessor.Model;
using MongoDB.Driver;

namespace KaVE.FeedbackProcessor.Database
{
    internal class NullDatabase : IFeedbackDatabase
    {
        public IDeveloperCollection GetDeveloperCollection()
        {
            return new NullDeveloperCollection();
        }

        public IIDEEventCollection GetEventsCollection()
        {
            return new NullIDEEventCollection();
        }
    }

    internal class NullIDEEventCollection : IIDEEventCollection
    {
        public IEnumerable<IDEEvent> FindAll()
        {
            throw new NotImplementedException();
        }

        public void Insert(IDEEvent instance) {}

        public void Save(IDEEvent instance) {}

        public void Clear() {}

        public IEnumerable<IDEEvent> GetEventStream(Developer developer)
        {
            throw new NotImplementedException();
        }

        public bool Contains(IDEEvent @event)
        {
            return false;
        }

        public IEnumerable<IDEEvent> Find(IMongoQuery query)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> FindAs<T>(IMongoQuery query) where T : IDEEvent
        {
            throw new NotImplementedException();
        }
    }

    internal class NullDeveloperCollection : IDeveloperCollection
    {
        public IEnumerable<Developer> FindAll()
        {
            return new List<Developer>();
        }

        public void Insert(Developer instance) {}

        public void Save(Developer instance) {}

        public void Clear() {}

        public IList<Developer> FindBySessionId(string sessionId)
        {
            return new List<Developer>();
        }
    }
}