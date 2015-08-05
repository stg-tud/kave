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
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.IO;
using Smile;

namespace KaVE.Commons.Utils.CodeCompletion.Impl
{
    public class SmilePBNRecommenderStore : IPBNRecommenderStore
    {
        public string BasePath { get; protected set; }
        private readonly IIoUtils _io;
        private readonly TypePathUtil _typePath;

        public SmilePBNRecommenderStore(string basePath, IIoUtils io, TypePathUtil typePath)
        {
            BasePath = basePath;
            _io = io;
            _typePath = typePath;
        }

        public bool IsAvailable(CoReTypeName type)
        {
            var fileName = GetNestedFileName(BasePath, type, "zip");
            return _io.FileExists(fileName);
        }

        public IPBNRecommender Load(CoReTypeName type)
        {
            var zipFileName = GetNestedFileName(BasePath, type, "zip");
            Asserts.That(_io.FileExists(zipFileName));

            var tmpFolder = _io.UnzipToTempFolder(zipFileName);
            var xdslFileName = GetFlatFileName(tmpFolder, type, "xdsl");
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

        private string GetNestedFileName(string basePath, CoReTypeName typeName, string extension)
        {
            var typePart = _typePath.ToNestedPath(typeName);
            var fileName = Path.Combine(basePath, typePart + '.' + extension);
            return fileName;
        }

        private string GetFlatFileName(string basePath, CoReTypeName typeName, string extension)
        {
            var typePart = _typePath.ToFlatPath(typeName);
            var fileName = Path.Combine(basePath, typePart + '.' + extension);
            return fileName;
        }
    }
}