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
using System.IO.Compression;
using NUnit.Framework;

namespace KaVE.RS.Commons.Tests_Unit.Utils
{
    [TestFixture]
    internal class GZipStreamTest
    {
        [Test]
        public void ShouldReadStreamWrittenSingleWriter()
        {
            var memory = new MemoryStream();
            using (var stream = new GZipStream(memory, CompressionMode.Compress))
            {
                stream.WriteByte(5);
                stream.WriteByte(42);
            }
            memory = new MemoryStream(memory.GetBuffer());
            var actual = new Byte[2];
            using (var stream = new GZipStream(memory, CompressionMode.Decompress))
            {
                stream.Read(actual, 0, 2);
            }
            Assert.AreEqual(new byte[] {5, 42}, actual);
        }

        [Test, Ignore("this does not work, since GzipStreams do not support appending to existing data")]
        public void ShouldReadStreamWrittenByMultipleWriters()
        {
            var memory = new MemoryStream();
            using (var stream = new GZipStream(memory, CompressionMode.Compress))
            {
                stream.WriteByte(5);
            }
            var intermediate = memory.GetBuffer();
            memory = new MemoryStream();
            memory.Write(intermediate, 0, intermediate.Length);
            using (var stream = new GZipStream(memory, CompressionMode.Compress))
            {
                stream.WriteByte(1);
            }
            memory = new MemoryStream(memory.GetBuffer());
            var actual = new Byte[2];
            using (var stream = new GZipStream(memory, CompressionMode.Decompress))
            {
                stream.Read(actual, 0, 2);
            }
            Assert.AreEqual(new byte[] {5, 1}, actual);
        }
    }
}