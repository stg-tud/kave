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
using System.Linq;
using System.Threading.Tasks;
using KaVE.Commons.Utils.Assertion;

namespace KaVE.FeedbackProcessor
{
    public class MultiRunner
    {
        private readonly object _lock = new object();

        private readonly IList<int> _tasksFresh = new List<int>();
        private readonly IList<int> _tasks = new List<int>();

        public void Run()
        {
            var rnd = new Random();
            for (var i = 0; i < 100; i++)
            {
                _tasksFresh.Add(rnd.Next()%25 + 17);
            }

            var t1 = Run(1);
            var t2 = Run(2);
            var t3 = Run(3);
            var t4 = Run(4);

            Console.WriteLine(@"1: {0}s", t1);
            Console.WriteLine(@"2: {0}s ({1:0.0}%)", t2, (100*t2/(double) t1));
            Console.WriteLine(@"3: {0}s ({1:0.0}%)", t3, (100*t3/(double) t1));
            Console.WriteLine(@"4: {0}s ({1:0.0}%)", t4, (100*t4/(double) t1));
        }

        public int Run(int num)
        {
            _tasks.Clear();
            foreach (var task in _tasksFresh)
            {
                _tasks.Add(task);
            }


            Console.WriteLine(@"Running with {0} parallel tasks", num);
            var start = DateTime.Now;
            var tasks = new Task[num];
            for (var i = 0; i < tasks.Length; i++)
            {
                //new Thread(() => Process(1)).Start();
                var i1 = i;
                tasks[i] = Task.Factory.StartNew(() => { Process(i1); });
            }

            Task.WaitAll(tasks);
            var end = DateTime.Now;

            Console.WriteLine(@"done");
            var duration = (end - start);
            return duration.Seconds;
        }

        private void Process(int i)
        {
            Console.WriteLine(@"Starting process {0}", i);
            int task;
            while (GetTask(out task))
            {
                var result = Fib(task);
                Console.WriteLine(@"({0}) Fib({1}) = {2}", i, task, result);
            }
            Console.WriteLine(@"Stopping process {0}", i);
        }

        private static int Fib(int task)
        {
            Asserts.That(task > 0);
            if (task == 1 || task == 2)
            {
                return 1;
            }
            return Fib(task - 1) + Fib(task - 2);
        }

        private bool GetTask(out int task)
        {
            lock (_lock)
            {
                if (_tasks.Count > 0)
                {
                    task = _tasks.First();
                    _tasks.Remove(task);
                    return true;
                }
                task = -1;
                return false;
            }
        }
    }
}