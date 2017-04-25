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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using KaVE.Commons.Model.Events.TestRunEvents;
using KaVE.FeedbackProcessor.WatchdogExports.Model;

namespace KaVE.FeedbackProcessor.WatchdogExports.Exporter
{
    public static class WatchdogUtils
    {
        public static string GetSerializedIntervalTypeName(Interval t)
        {
            if (t is VisualStudioOpenedInterval)
            {
                return "eo";
            }
            if (t is VisualStudioActiveInterval)
            {
                return "ea";
            }
            if (t is UserActiveInterval)
            {
                return "ua";
            }
            if (t is PerspectiveInterval)
            {
                return "pe";
            }
            if (t is TestRunInterval)
            {
                return "ju";
            }

            var re = t as FileInteractionInterval;
            if (re != null)
            {
                switch (re.Type)
                {
                    case FileInteractionType.Reading:
                        return "re";
                    case FileInteractionType.Typing:
                        return "ty";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            throw new ArgumentException();
        }

        public static string GetSerializedDocumentTypeName(DocumentType type)
        {
            switch (type)
            {
                case DocumentType.Undefined:
                    return "un";
                case DocumentType.Production:
                    return "pr";
                case DocumentType.FilenameTest:
                    return "lt";
                case DocumentType.PathnameTest:
                    return "pf";
                case DocumentType.Test:
                    return "te";
                case DocumentType.TestFramework:
                    return "tf";
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }

        public static string ToSerializedName(this TestResult result)
        {
            switch (result)
            {
                case TestResult.Unknown:
                    return "U";
                case TestResult.Success:
                    return "O";
                case TestResult.Failed:
                    return "F";
                case TestResult.Error:
                    return "E";
                case TestResult.Ignored:
                    return "I";
                default:
                    throw new ArgumentOutOfRangeException("result", result, null);
            }
        }

        public static long AsUtcTimestamp(this DateTime date)
        {
            var start = new DateTime(1970, 1, 1);
            var unixTimestamp = date.Ticks - start.Ticks;
            unixTimestamp /= TimeSpan.TicksPerMillisecond;
            return unixTimestamp;
        }

        public static string ToIsoDate(this DateTime date)
        {
            return date.ToString("s", CultureInfo.InvariantCulture) + "Z";
        }

        public static void WriteToFiles(this WatchdogData data, string outputFolder)
        {
            Directory.CreateDirectory(outputFolder);
            File.WriteAllLines(Path.Combine(outputFolder, "intervals.json"), data.Intervals.Select(o => o.ToString()));
            File.WriteAllLines(Path.Combine(outputFolder, "projects.json"), data.Projects.Select(o => o.ToString()));
            File.WriteAllLines(Path.Combine(outputFolder, "users.json"), data.Users.Select(o => o.ToString()));
        }

        public static string Sha1Hash(string input)
        {
            using (var sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                return string.Join(string.Empty, hash.Select(b => b.ToString("x2")));
            }
        }
    }
}