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
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;
using KaVE.JetBrains.Annotations;

namespace KaVE.FeedbackProcessor.Preprocessing.Model
{
    public class PreprocessingData
    {
        private readonly object _lock = new object();

        private readonly IKaVESet<string> _allZips;
        private readonly IKaVESet<string> _unindexedZips;
        private readonly IDictionary<string, IKaVESet<string>> _idsByZip;
        private IKaVESet<IKaVESet<string>> _zipGroups;
        private readonly IKaVESet<string> _uncleansedZips;

        public PreprocessingData(IKaVESet<string> zips)
        {
            _allZips = zips;
            _unindexedZips = Sets.NewHashSetFrom(_allZips);
            _idsByZip = new Dictionary<string, IKaVESet<string>>();
            _uncleansedZips = Sets.NewHashSet<string>();
        }

        public int NumUnindexedZips
        {
            get
            {
                lock (_lock)
                {
                    return _unindexedZips.Count;
                }
            }
        }

        public int NumGroups
        {
            get
            {
                lock (_lock)
                {
                    return _zipGroups == null ? 0 : _zipGroups.Count;
                }
            }
        }

        public int NumUnclean
        {
            get
            {
                lock (_lock)
                {
                    return _uncleansedZips.Count;
                }
            }
        }

        public bool AcquireNextUnindexedZip(out string zip)
        {
            lock (_lock)
            {
                zip = _unindexedZips.FirstOrDefault();
                if (zip != null)
                {
                    _unindexedZips.Remove(zip);
                    return true;
                }
                return false;
            }
        }

        public void StoreIds([NotNull] string zip, [NotNull] IKaVESet<string> ids)
        {
            lock (_lock)
            {
                Asserts.NotNull(zip);
                Asserts.NotNull(ids);
                Asserts.That(_allZips.Contains(zip));
                Asserts.Not(_idsByZip.ContainsKey(zip));
                _idsByZip[zip] = ids;
            }
        }

        public IDictionary<string, IKaVESet<string>> GetIdsByZip()
        {
            lock (_lock)
            {
                return _idsByZip;
            }
        }

        public void StoreZipGroups([NotNull] IKaVESet<IKaVESet<string>> zipGroups)
        {
            lock (_lock)
            {
                Asserts.That(_zipGroups == null);
                Asserts.NotNull(zipGroups);
                Asserts.Not(zipGroups.Count == 0);
                foreach (var zipGroup in zipGroups)
                {
                    Asserts.Not(zipGroup.Count == 0);
                    foreach (var zip in zipGroup)
                    {
                        Asserts.That(_allZips.Contains(zip));
                    }
                }
                _zipGroups = Sets.NewHashSetFrom(zipGroups);
            }
        }

        public bool AcquireNextUnmergedZipGroup(out IKaVESet<string> zips)
        {
            lock (_lock)
            {
                zips = _zipGroups.FirstOrDefault();
                if (zips != null)
                {
                    _zipGroups.Remove(zips);
                    return true;
                }
                return false;
            }
        }

        public void StoreMergedZip([NotNull] string zip)
        {
            lock (_lock)
            {
                Asserts.NotNull(_zipGroups);
                Asserts.NotNull(zip);
                Asserts.That(_allZips.Contains(zip));
                Asserts.Not(_uncleansedZips.Contains(zip));
                foreach (var zipGroup in _zipGroups)
                {
                    Asserts.Not(zipGroup.Contains(zip));
                }
                _uncleansedZips.Add(zip);
            }
        }

        public bool AcquireNextUncleansedZip(out string zip)
        {
            lock (_lock)
            {
                zip = _uncleansedZips.FirstOrDefault();
                if (zip != null)
                {
                    _uncleansedZips.Remove(zip);
                    return true;
                }
                return false;
            }
        }
    }
}