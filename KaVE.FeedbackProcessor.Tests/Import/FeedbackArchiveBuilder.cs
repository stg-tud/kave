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
using System.IO;
using Ionic.Zip;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils.Json;

namespace KaVE.FeedbackProcessor.Tests.Import
{
    public class FeedbackArchiveBuilder
    {
        private readonly string _archiveName;
        private readonly IDictionary<string, string> _contents = new Dictionary<string, string>();

        private FeedbackArchiveBuilder(string archiveName)
        {
            _archiveName = archiveName;
        }

        public static FeedbackArchiveBuilder AnArchive()
        {
            return new FeedbackArchiveBuilder("TestArchive.zip");
        }

        public static FeedbackArchiveBuilder AnArchive(string archiveName)
        {
            return new FeedbackArchiveBuilder(archiveName);
        }

        public FeedbackArchiveBuilder With(String fileName, String content)
        {
            _contents[fileName] = content;
            return this;
        }

        public FeedbackArchiveBuilder With(string fileName, IDEEvent @event)
        {
            _contents[fileName] = @event.ToCompactJson();
            return this;
        }

        public ZipFile Create()
        {
            var zipFile = new ZipFile();
            foreach (var e in _contents)
            {
                var fileName = e.Key;
                var json = e.Value;
                zipFile.AddEntry(fileName, json);
            }
            var ms = new MemoryStream();
            zipFile.Save(ms);
            ms.Position = 0;
            var archive = ZipFile.Read(ms);
            archive.Name = _archiveName;
            return archive;
        }
    }
}