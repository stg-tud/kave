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
 *    - Sebastian Proksch
 */

using System.IO;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.IO;
using Smile;

namespace KaVE.Commons.Utils.CodeCompletion.Impl
{
    public class SmilePBNRecommenderStore : IPBNRecommenderStore
    {
        private readonly string _basePath;
        private readonly IIoUtils _io;

        public SmilePBNRecommenderStore(string basePath, IIoUtils io)
        {
            _basePath = basePath;
            _io = io;
        }

        public bool IsAvailable(CoReTypeName type)
        {
            var fileName = GetFileName(_basePath, type, "zip");
            return _io.FileExists(fileName);
        }

        public IPBNRecommender Load(CoReTypeName type)
        {
            var zipFileName = GetFileName(_basePath, type, "zip");
            Asserts.That(_io.FileExists(zipFileName));

            var tmpFolder = _io.UnzipToTempFolder(zipFileName);
            var xdslFileName = GetFileName(tmpFolder, type, "xdsl");
            Asserts.That(_io.FileExists(xdslFileName));

            var network = ReadNetwork(xdslFileName);
            return new SmilePBNRecommender(type, network);
        }

        private static Network ReadNetwork(string xdslFileName)
        {
            try
            {
                var network = new Network();
                network.ReadFile(xdslFileName);
                return network;
            }
            catch (SmileException e)
            {
                throw new AssertException("error reading the network", e);
            }
        }

        private static string GetFileName(string basePath, CoReTypeName typeName, string extension)
        {
            var typePart = typeName.ToString().Replace('/', '_');
            var fileName = Path.Combine(basePath, typePart + '.' + extension);
            return fileName;
        }
    }
}