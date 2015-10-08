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
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.Concurrency;
using KaVE.Commons.Utils.Exceptions;
using KaVE.Commons.Utils.IO;
using Newtonsoft.Json;

namespace KaVE.VS.Statistics.Utils
{
    public class FileHandler
    {
        private readonly ILogger _errorHandler;
        private readonly string _filePath;

        private readonly IIoUtils _ioUtil;
        private readonly BlockingCollection<string> _writeQueue;

        public FileHandler(string fileName, string directoryPath)
        {
            _filePath = Path.Combine(directoryPath, fileName);
            _ioUtil = Registry.GetComponent<IIoUtils>();
            _errorHandler = Registry.GetComponent<ILogger>();

            if (!_ioUtil.DirectoryExists(directoryPath))
            {
                _ioUtil.CreateDirectory(directoryPath);
            }

            _writeQueue = new BlockingCollection<string>();
            Task.StartNewLongRunning(ProcessWriteQueue);
        }

        public void WriteContentToFile(object content)
        {
            if (content == null)
            {
                return;
            }

            try
            {
                var json = JsonSerialization.JsonSerializeObject(content);

                _writeQueue.Add(json);
            }
            catch (Exception e)
            {
                _errorHandler.Error(
                    e,
                    "Interval between events is too short or too many events triggered");
            }
        }

        /// <exception cref="JsonSerializationException"></exception>
        /// <exception cref="JsonReaderException"></exception>
        public T ReadContentFromFile<T>()
        {
            var jsonString = _ioUtil.FileExists(_filePath) ? _ioUtil.ReadFile(_filePath) : "";
            return JsonSerialization.JsonDeserializeObject<T>(jsonString);
        }

        public void ProcessWriteQueue()
        {
            foreach (var content in _writeQueue.GetConsumingEnumerable())
            {
                if (!_ioUtil.FileExists(_filePath))
                {
                    _ioUtil.CreateFile(_filePath);
                }

                try
                {
                    using (var writer = new StreamWriter(_ioUtil.OpenFile(_filePath, FileMode.Create, FileAccess.Write))
                        )
                    {
                        writer.Write(content);
                    }
                }
                catch (Exception e)
                {
                    _errorHandler.Error(e, "Could not write file");
                }
            }
        }

        public void DeleteFile()
        {
            _ioUtil.DeleteFile(_filePath);
        }

        public void ResetFile()
        {
            _ioUtil.DeleteFile(_filePath);
            _ioUtil.CreateFile(_filePath);
        }
    }
}