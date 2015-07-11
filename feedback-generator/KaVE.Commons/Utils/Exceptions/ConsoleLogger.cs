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

namespace KaVE.Commons.Utils.Exceptions
{
    public class ConsoleLogger : ILogger
    {
        public void Error(Exception exception, string content, params object[] args)
        {
            Console.Write("[ERROR] ");
            Console.Write(System.DateTime.Now);
            Console.Write(" - ");
            Console.WriteLine(content, args);
            Console.WriteLine(exception.ToString());
        }

        public void Error(Exception exception)
        {
            Console.Write("[ERROR] ");
            Console.Write(System.DateTime.Now);
            Console.Write(" - ");
            Console.WriteLine(exception.ToString());
        }

        public void Error(string content, params object[] args)
        {
            Console.Write("[ERROR] ");
            Console.Write(System.DateTime.Now);
            Console.Write(" - ");
            Console.WriteLine(content, args);
        }

        public void Info(string info, params object[] args)
        {
            Console.Write("[INFO] ");
            Console.Write(System.DateTime.Now);
            Console.Write(" - ");
            Console.WriteLine(info, args);
        }
    }
}