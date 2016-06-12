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
using System.Linq;
using JetBrains.ReSharper.Features.Inspections.Resources;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp.Parser;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.IO.Archives;

namespace KaVE.RS.SolutionAnalysis
{
    public class NameGrabber
    {
        private readonly string _dirIn;
        private readonly string _dirOut;
        private readonly int numMaxZips;
        private readonly int numMaxInvalidNames;
        private bool writeToFile_;

        public NameGrabber(string dirIn, string dirOut, int numMaxZips, int numMaxInvalidNames,bool writeToFile_)
        {
            _dirIn = dirIn;
            _dirOut = dirOut;
            this.numMaxZips = numMaxZips;
            this.numMaxInvalidNames = numMaxInvalidNames;
            this.writeToFile_ = writeToFile_;
        }
        public void Run()
        {
            Console.WriteLine("Grab Names from Contexts");
            var ctx = FindInputFiles();

            var numZips = ctx.Count();
            var currentZip = 1;

            var numTotalCtxs = 0;
            var numTotalUsages = 0;
            List<Tuple<string, List<string>>> ssts = new KaVEList<Tuple<string, List<string>>>();
            foreach (var fileName in ctx)
            {
                Log("### processing zip {0}/{1}: {2}", currentZip++, numZips, fileName);

                var fullFileIn = _dirIn + fileName;

                var ra = new ReadingArchive(fullFileIn);
                Log("reading contexts...");
                var numCtxs = 0;
                while (ra.HasNext())
                {
                    var context = ra.GetNext<Context>();
                    var list = new KaVEList<string>();
                    foreach (var n in CsNameUtil.Names)
                    {
                        list.Add(n);
                    }
                    ssts.Add(new Tuple<string, List<string>>(context.SST.EnclosingType.Identifier, list));
                    CsNameUtil.Names.Clear();
                    numCtxs++;
                }
                Log("found {0} contexts\n\n", numCtxs);
                if (currentZip == numMaxZips + 1)
                {
                    break;
                }
            }
            var typeNameNullCount = 0;
            var typeNameCount = 0;
            var methodNameNullCount = 0;
            var methodNameCount = 0;
            List<Tuple<string, string>> wrongSyntaxTypeName = new KaVEList<Tuple<string, string>>();
            List<Tuple<string, string>> wrongSyntaxMethodName = new KaVEList<Tuple<string, string>>();
            foreach (var t in ssts)
            {
                foreach (var s in t.Item2)
                {
                    var type = s.Split(':');
                    if (type[0].Equals("CSharp.TypeName"))
                    {
                        typeNameCount++;
                        var name = CsNameUtil.ParseJson(s);
                        if (name.Identifier == "?")
                        {
                            wrongSyntaxTypeName.Add(new Tuple<string, string>(s, t.Item1));
                            typeNameNullCount++;
                        }
                    }
                    else if (type[0].Equals("CSharp.MethodName"))
                    {
                        methodNameCount++;
                        var name = CsNameUtil.ParseJson(s);
                        if (name.Identifier == "?")
                        {
                            wrongSyntaxMethodName.Add(new Tuple<string, string>(s, t.Item1));
                            methodNameNullCount++;
                        }
                    }
                }
            }
            Log("{0} of {1} names are null", typeNameNullCount, typeNameCount);
            Log("{0} of {1} names are null", methodNameNullCount, methodNameCount);
            double percentageTypeNames = (double) ((double) typeNameNullCount/(double) typeNameCount);
            double percentageMethodNames = (double) ((double) methodNameNullCount/(double) methodNameCount);
            Log("TypeNames not parseable: {0}%\n", percentageTypeNames);
            Log("MethodNames not parseable: {0}%\n\n", percentageMethodNames);

            //showInvalidNames(wrongSyntaxTypeName);

            Log("\n\n");

            //showInvalidNames(wrongSyntaxMethodName);

            if (writeToFile_)
            {
                Log("File with invalid names written to {0}", _dirOut);
                writeToFile(wrongSyntaxMethodName, _dirOut + "/methodname.txt");   
                writeToFile(wrongSyntaxTypeName, _dirOut + "/typename.txt");

            }
            //Log(wrongSyntax[0].Item1 + "\n\n");
            //Log(wrongSyntax[0].Item2 + "\n\n");
        }

        private void showInvalidNames(List<Tuple<string, string>> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                if (i == numMaxInvalidNames)
                {
                    break;
                }
                Log("{0}", list[i].Item1);
                Log("{0} / {1}", list[i].Item1, list[i].Item2);
            }
        }

        private void writeToFile(List<Tuple<string, string>> list, string filePath)
        {
            List<string> s = new KaVEList<string>();
            for (var i = 0; i < list.Count; i++)
            {
                if (i == numMaxInvalidNames)
                {
                    break;
                }
                s.Add(list[i].Item1);
            }
            File.WriteAllLines(filePath, s);
        }

        private IList<string> FindInputFiles()
        {
            var files = Directory.EnumerateFiles(_dirIn, "*-contexts.zip", SearchOption.AllDirectories);
            return files.Select(f => f.Substring(_dirIn.Length)).ToList();
        }

        private static void Log(string msg, params object[] args)
        {
            Console.Write(Environment.NewLine + @"[{0}] ", DateTime.Now);
            Console.Write(msg, args);
        }
    }
}