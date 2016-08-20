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
using System.IO;
using System.Linq;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.Preprocessing.Logging;

namespace KaVE.FeedbackProcessor.Preprocessing
{
    public interface IPreprocessingIo
    {
        FileLogger CreateLoggerWorker(int taskId);

        IKaVESet<string> FindRelativeZipPaths();
        string GetFullPath_Raw(string relativeZipPath);
        string GetFullPath_Merged(string relativeZipPath);
        string GetFullPath_Final(string relativeZipPath);
        void EnsureParentExists(string fullPath);
    }

    public class PreprocessingIo : IPreprocessingIo
    {
        private readonly object _lock = new object();

        private readonly string _dirRaw;
        private readonly string _dirMerged;
        private readonly string _dirLogs;
        private readonly string _dirFinal;

        public PreprocessingIo(string dirRaw, string dirTmp, string dirFinal)
        {
            if (!dirRaw.EndsWith(@"\"))
            {
                dirRaw += @"\";
            }
            if (!dirTmp.EndsWith(@"\"))
            {
                dirTmp += @"\";
            }
            if (!dirFinal.EndsWith(@"\"))
            {
                dirFinal += @"\";
            }

            _dirRaw = dirRaw;
            _dirMerged = dirTmp + @"merged\";
            _dirLogs = dirTmp + @"logs\";
            _dirFinal = dirFinal;

            Console.WriteLine(@"working directories:");
            Console.WriteLine(@"- raw:  {0}", dirRaw);
            Console.WriteLine(@"- merged: {0}", _dirMerged);
            Console.WriteLine(@"- logs: {0}", _dirLogs);
            Console.WriteLine(@"- final: {0}", dirFinal);
        }


        public void EnsureParentExists(string fullName)
        {
            var parent = Path.GetDirectoryName(fullName);
            Asserts.NotNull(parent);
            if (!Directory.Exists(parent))
            {
                Directory.CreateDirectory(parent);
            }
        }

        public IKaVESet<string> FindRelativeZipPaths()
        {
            var zips = Directory.EnumerateFiles(_dirRaw, "*.zip", SearchOption.AllDirectories)
                                .Select(f => f.Replace(_dirRaw, ""));
            return Sets.NewHashSetFrom(zips);
        }

        public FileLogger CreateLoggerWorker(int taskId)
        {
            throw new NotImplementedException();
        }

        public string GetFullPath_Raw(string zip)
        {
            throw new NotImplementedException();
        }

        public string GetFullPath_Merged(string zip)
        {
            throw new NotImplementedException();
        }

        public string GetFullPath_Final(string zip)
        {
            throw new NotImplementedException();
        }
    }
}