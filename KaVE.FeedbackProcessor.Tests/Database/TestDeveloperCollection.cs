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
using System.Linq;
using KaVE.Commons.Utils.Assertion;
using KaVE.FeedbackProcessor.Database;
using KaVE.FeedbackProcessor.Model;
using MongoDB.Bson;

namespace KaVE.FeedbackProcessor.Tests.Database
{
    internal class TestDeveloperCollection : IDeveloperCollection
    {
        private int _nextDeveloperId = 1;
        private readonly ICollection<Developer> _developers = new List<Developer>();

        public IEnumerable<Developer> FindAll()
        {
            return _developers;
        }

        public void Insert(Developer instance)
        {
            if (instance.Id == default(ObjectId))
            {
                instance.Id = new ObjectId(Convert.ToString(_nextDeveloperId).PadLeft(24, '0'));
            }
            Asserts.That(!IsInCollection(instance), "dublicated object id");
            _developers.Add(instance);
            _nextDeveloperId++;
        }

        public void Save(Developer instance)
        {
            Asserts.That(IsInCollection(instance), "saving instance that's not in the collection");
        }

        private bool IsInCollection(Developer instance)
        {
            return _developers.Any(dev => dev.Id.Equals(instance.Id));
        }

        public void Clear()
        {
            _developers.Clear();
        }

        public IList<Developer> FindBySessionId(string sessionId)
        {
            return _developers.Where(dev => dev.SessionIds.Contains(sessionId)).ToList();
        }
    }
}