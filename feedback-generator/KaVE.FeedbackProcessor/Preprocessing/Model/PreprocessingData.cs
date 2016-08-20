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
using KaVE.Commons.Utils.Collections;

namespace KaVE.FeedbackProcessor.Preprocessing.Model
{
    public class PreprocessingData
    {
        private readonly object _lock = new object();

        public PreprocessingData(IKaVESet<string> zips)
        {
            throw new NotImplementedException();
        }

        public bool FindNextUnindexedZip(out string zip)
        {
            zip = null;
            return false;
        }

        public void StoreIds(string zip, IKaVESet<string> ids)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, IKaVESet<string>> GetIdsByZip()
        {
            throw new NotImplementedException();
        }

        public ISet<string> GetIds(string zip)
        {
            throw new NotImplementedException();
        }

        public void StoreZipGroups(IKaVESet<IKaVESet<string>> zipGroups)
        {
            foreach (var zipGroup in zipGroups)
            {
                StoreZipGroup(zipGroup);
            }
        }

        private void StoreZipGroup(IKaVESet<string> ids)
        {
            throw new NotImplementedException();
        }

        public bool AcquireNextUnmergedZipGroup(out IKaVESet<string> zips)
        {
            throw new NotImplementedException();
        }

        public bool AcquireNextUncleansedZip(out string zip)
        {
            throw new NotImplementedException();
        }
    }
}