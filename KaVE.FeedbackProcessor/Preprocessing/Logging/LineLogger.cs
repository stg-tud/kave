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
using System.Text;

namespace KaVE.FeedbackProcessor.Preprocessing.Logging
{
    public class LineLogger : IPrepocessingLogger
    {
        private readonly IList<string> lines = new List<string>();
        private StringBuilder cur = new StringBuilder();
        private bool _isFirstLine = true;

        public string[] LoggedLines
        {
            get
            {
                var arr = new string[lines.Count + 1];
                var i = 0;
                foreach (var line in lines)
                {
                    arr[i++] = line;
                }
                arr[i] = cur.ToString();
                return arr;
            }
        }

        public void Log()
        {
            Log("");
        }

        public void Log(string text, params object[] args)
        {
            if (!_isFirstLine)
            {
                lines.Add(cur.ToString());
                cur = new StringBuilder();
            }
            _isFirstLine = false;
            Append(text, args);
        }

        public void Append(string text, params object[] args)
        {
            _isFirstLine = false;
            cur.Append(args.Length == 0 ? text : string.Format(text, args));
        }
    }
}