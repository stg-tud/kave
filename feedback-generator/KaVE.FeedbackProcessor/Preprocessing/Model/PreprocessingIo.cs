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

using System.IO;
using System.Linq;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;
using KaVE.JetBrains.Annotations;

namespace KaVE.FeedbackProcessor.Preprocessing.Model
{
    public interface IPreprocessingIo
    {
        IKaVESet<string> FindRelativeZipPaths();
        string GetFullPath_In(string relativeZipPath);
        string GetFullPath_Merged(string relativeZipPath);
        string GetFullPath_Out(string relativeZipPath);
        void EnsureParentExists(string fullPath);
        long GetSize_In(string zip);
    }

    public class PreprocessingIo : IPreprocessingIo
    {
        private readonly object _lock = new object();

        private readonly string _dirRaw;
        private readonly string _dirMerged;
        private readonly string _dirFinal;

        public PreprocessingIo([NotNull] string dirRaw, [NotNull] string dirMerged, [NotNull] string dirFinal)
        {
            Asserts.NotNull(dirRaw);
            Asserts.NotNull(dirMerged);
            Asserts.NotNull(dirFinal);
            Asserts.That(Directory.Exists(dirRaw));
            Asserts.That(Directory.Exists(dirMerged));
            Asserts.That(Directory.Exists(dirFinal));

            if (!dirRaw.EndsWith(@"\"))
            {
                dirRaw += @"\";
            }
            if (!dirMerged.EndsWith(@"\"))
            {
                dirMerged += @"\";
            }
            if (!dirFinal.EndsWith(@"\"))
            {
                dirFinal += @"\";
            }

            _dirRaw = dirRaw;
            _dirMerged = dirMerged;
            _dirFinal = dirFinal;
        }

        public IKaVESet<string> FindRelativeZipPaths()
        {
            lock (_lock)
            {
                var zips = Directory.EnumerateFiles(_dirRaw, "*.zip", SearchOption.AllDirectories)
                                    .Select(f => f.Replace(_dirRaw, ""));
                return Sets.NewHashSetFrom(zips);
            }
        }

        public string GetFullPath_In(string zip)
        {
            lock (_lock)
            {
                return Path.Combine(_dirRaw, zip);
            }
        }

        public string GetFullPath_Merged(string zip)
        {
            lock (_lock)
            {
                return Path.Combine(_dirMerged, zip);
            }
        }

        public string GetFullPath_Out(string zip)
        {
            lock (_lock)
            {
                return Path.Combine(_dirFinal, zip);
            }
        }

        public void EnsureParentExists(string fullName)
        {
            lock (_lock)
            {
                var parent = Path.GetDirectoryName(fullName);
                Asserts.NotNull(parent);
                if (!Directory.Exists(parent))
                {
                    Directory.CreateDirectory(parent);
                }
            }
        }

        public long GetSize_In(string zip)
        {
            lock (_lock)
            {
                var path = GetFullPath_In(zip);
                return new FileInfo(path).Length;
            }
        }
    }
}