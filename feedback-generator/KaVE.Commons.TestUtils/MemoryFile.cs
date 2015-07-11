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

namespace KaVE.Commons.TestUtils
{
    /// <summary>
    ///     An in-memory "file" containing a string value. Streams for reading and writing the content can be created from an
    ///     instance.
    /// </summary>
    public class MemoryFile
    {
        private byte[] _content = new byte[0];

        public string Content
        {
            get { return System.Text.Encoding.UTF8.GetString(_content); }
            set { _content = System.Text.Encoding.UTF8.GetBytes(value); }
        }

        public Stream OpenRead()
        {
            return new MemoryStream(_content, false);
        }

        public Stream OpenAppend()
        {
            var stream = new TestMemoryStream(this);
            stream.Write(_content, 0, _content.Length);
            return stream;
        }

        private class TestMemoryStream : MemoryStream
        {
            private readonly MemoryFile _file;

            public TestMemoryStream(MemoryFile file)
            {
                _file = file;
            }

            protected override void Dispose(bool disposing)
            {
                _file._content = base.ToArray();
                base.Dispose(disposing);
            }
        }
    }
}