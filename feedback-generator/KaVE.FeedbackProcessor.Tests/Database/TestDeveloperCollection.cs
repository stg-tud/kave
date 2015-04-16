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
using System;
using System.Collections.Generic;
using KaVE.FeedbackProcessor.Database;
using KaVE.FeedbackProcessor.Model;

namespace KaVE.FeedbackProcessor.Tests.Database
{
    internal class TestDeveloperCollection : IDeveloperCollection
    {
        private readonly ICollection<Developer> _developers = new List<Developer>();

        public IEnumerable<Developer> FindAll()
        {
            return _developers;
        }

        public void Insert(Developer instance)
        {
            _developers.Add(instance);
        }

        public void Save(Developer instance)
        {
            throw new NotImplementedException();
        }

        public IList<Developer> FindBySessionId(string sessionId)
        {
            throw new NotImplementedException();
        }
    }
}