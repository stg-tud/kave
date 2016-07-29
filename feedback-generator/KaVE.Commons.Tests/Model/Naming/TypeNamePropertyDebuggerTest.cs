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
using System.Collections;
using KaVE.Commons.Model.Naming;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Naming
{
    internal class TypeNamePropertyDebuggerTest
    {
        public void Process(string typeId)
        {
            var sut = Names.Type(typeId);
            var type = sut.GetType();
            Console.WriteLine("### '{0}' ###", typeId);
            Console.WriteLine("Type: {0}", type);
            foreach (var p in type.GetProperties())
            {
                var name = p.Name;
                var value = p.GetValue(sut, null);
                var values = value as IEnumerable;
                if (!(value is string) && values != null)
                {
                    Console.WriteLine("{0}:", name);
                    foreach (var v in values)
                    {
                        Console.WriteLine("\t- {0}", v);
                    }
                }
                else
                {
                    Console.WriteLine("{0}: {1}", name, value);
                }
            }
            Console.WriteLine();
        }

        [Test, Ignore]
        public void Process()
        {
            foreach (
                var s in
                    new[]
                    {
                        "",
                        "x",
                        "x->x",
                        "x -> T,P",
                        "System.Int32, mscorlib, 4.0.0.0",
                        "System.Object, mscorlib, 4.0.0.0",
                        "System.Void, mscorlib, 4.0.0.0",
                        "e:a.b.AnEnum, P",
                        "a.b.C`1[[T]], P",
                        "i:a.b.I[], P",
                        // analyse bug (`1 missing)...
                        "a.b.TC`1[[T1]]+NC[[T2]], P",
                        // corrected version
                        "a.b.TC`1[[T1]]+NC`1[[T2]], P",
                        "a.b.T1+N1, P",
                        "d:[System.Void, mscorlib, 4.0.0.0] [System.Action, mscorlib, 4.0.0.0].()"
                    })
            {
                Process(s);
            }
        }
    }

    // used to trigger code completion for easy generation of example types
    public class TC<T1>
    {
        int _i;
        AnEnum _e;
        object o;

        public void M() {}

        public void M(T1 t)
        {
            I[] ii;
        }

        public class NC<T2>
        {
            public void M()
            {
                N(M);
            }

            public void N(Action a) {}
        }
    }

    public class T1
    {
        public class N1
        {
            public void M() {}
        }
    }

    public interface I {}

    public enum AnEnum
    {
        A,
        B
    }
}