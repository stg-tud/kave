using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.Json;
using KaVE.FeedbackProcessor.Preprocessing.Model;

namespace KaVE.FeedbackProcessor.DebuggingHacks
{
    public class FailIdentifier
    {
        [SuppressMessage("ReSharper", "LocalizableElement")]
        public void Run()
        {
            var io = new PreprocessingIo(@"F:\Contexts\", @"F:\Tmp\", @"F:\Tmp\");
            var zips = io.FindRelativeZipPaths();
            var zipSlns = Sets.NewHashSetFrom(zips.Select(z => @"GH\" + z.Replace("-contexts.zip", "")));

            var jsonAll = File.ReadAllText(@"F:\R\index.json");
            var allSln = jsonAll.ParseJsonTo<HashSet<string>>();

            var json = File.ReadAllText(@"F:\R\ended-incl-fails.json");
            var endedInclFails = json.ParseJsonTo<HashSet<string>>();

            Console.WriteLine("== Found zips for the following {0} slns ==", zipSlns.Count);

            var sb = new StringBuilder();
            sb.Append("[");
            var first = true;
            foreach (var sln in allSln)
            {
                if (zipSlns.Contains(sln))
                {
                    if (!first)
                    {
                        sb.Append(',');
                    }
                    first = false;
                    sb.Append('"').Append(sln).Append('"');
                }
            }
            sb.Append("]");
            Console.WriteLine(sb.ToString());
        }
    }
}