﻿/*
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
using System.Globalization;
using System.IO;
using System.Linq;
using KaVE.FeedbackProcessor.Intervals.Model;

namespace KaVE.FeedbackProcessor.Intervals.Exporter
{
    public static class WatchdogExporter
    {
        public static WatchdogObject Convert(Interval interval)
        {
            if (interval is VisualStudioOpenedInterval ||
                interval is UserActiveInterval)
            {
                return ConvertBasicInterval(interval);
            }
            var perspectiveInterval = interval as PerspectiveInterval;
            if (perspectiveInterval != null)
            {
                return ConvertPerspectiveInterval(perspectiveInterval);
            }
            var testRunInterval = interval as TestRunInterval;
            if (testRunInterval != null)
            {
                return ConvertTestRunInterval(testRunInterval);
            }
            var fileInteractionInterval = interval as FileInteractionInterval;
            if (fileInteractionInterval != null)
            {
                if (fileInteractionInterval.Type == FileInteractionType.Reading)
                {
                    return ConvertReadingInterval(fileInteractionInterval);
                }
                if (fileInteractionInterval.Type == FileInteractionType.Typing)
                {
                    return ConvertTypingInterval(fileInteractionInterval);
                }
            }
            throw new NotImplementedException("Unsupported interval type.");
        }

        public static WatchdogData Convert(IList<Interval> intervals)
        {
            intervals = intervals.OrderBy(i => i.StartTime).ToList();

            var data = new WatchdogData();
            var createdProjectObjects = new HashSet<string>();
            var createdUserObjects = new HashSet<string>();

            foreach (var interval in intervals)
            {
                try
                {
                    data.Intervals.Add(Convert(interval));
                }
                catch
                {
                    continue;
                }

                if (!createdUserObjects.Contains(interval.UserId))
                {
                    data.Users.Add(CreateUserObject(interval));
                    createdUserObjects.Add(interval.UserId);
                }

                if (!createdProjectObjects.Contains(interval.Project))
                {
                    data.Projects.Add(CreateProjectObject(interval));
                    createdProjectObjects.Add(interval.Project);
                }
            }

            return data;
        }

        private static WatchdogWrappedValue Wrapped(string wrapper, object value)
        {
            return new WatchdogWrappedValue {Wrapper = wrapper, Value = value.ToString()};
        }

        private static WatchdogStringValue String(string value)
        {
            return new WatchdogStringValue {Value = value};
        }

        private static WatchdogIntValue Int(int value)
        {
            return new WatchdogIntValue {Value = value};
        }

        private static WatchdogUnquotedLiteral Literal(string value)
        {
            return new WatchdogUnquotedLiteral {Value = value};
        }

        private static WatchdogObject ConvertBasicInterval(Interval interval)
        {
            var obj = new WatchdogObject();
            obj.Properties.Add("_id", Wrapped("ObjectId", interval.GetHashCode().ToString()));
            obj.Properties.Add("it", String(WatchdogUtils.GetSerializedIntervalTypeName(interval)));
            obj.Properties.Add("ts", Wrapped("NumberLong", interval.StartTime.ToJavaTimestamp()));
            obj.Properties.Add("te", Wrapped("NumberLong", (interval.StartTime + interval.Duration).ToJavaTimestamp()));
            obj.Properties.Add("ss", String(WatchdogUtils.Sha1Hash(interval.IDESessionId)));
            obj.Properties.Add("wdv", String("KaVE"));
            obj.Properties.Add("ide", String("vs"));
            obj.Properties.Add("userId", String(WatchdogUtils.Sha1Hash(interval.UserId)));
            obj.Properties.Add("projectId", String(WatchdogUtils.Sha1Hash(interval.Project)));
            obj.Properties.Add("ip", String("0.0.0.0"));
            obj.Properties.Add("regDate", Wrapped("ISODate", interval.CreationTime.ToString("o")));
            return obj;
        }

        private static WatchdogObject ConvertPerspectiveInterval(PerspectiveInterval interval)
        {
            var obj = ConvertBasicInterval(interval);
            // note: there is a third perspective type (Other, "ot")
            string type;
            switch (interval.Perspective)
            {
                case PerspectiveType.Production:
                    type = "ja"; // "Java" perspective
                    break;
                case PerspectiveType.Debug:
                    type = "de";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            obj.Properties.Add("pet", String(type));
            return obj;
        }

        private static WatchdogObject CreateFileRepresentationObject(FileInteractionInterval fileInteractionInterval)
        {
            var doc = new WatchdogObject();
            doc.Properties.Add("pn", String(WatchdogUtils.Sha1Hash(fileInteractionInterval.Project)));
            doc.Properties.Add("fn", String(WatchdogUtils.Sha1Hash(Path.GetFileName(fileInteractionInterval.FileName))));
            doc.Properties.Add("sloc", Int(-1));
            doc.Properties.Add(
                "dt",
                String(WatchdogUtils.GetSerializedDocumentTypeName(fileInteractionInterval.FileType)));
            return doc;
        }

        private static WatchdogObject ConvertReadingInterval(FileInteractionInterval fileInteractionInterval)
        {
            var obj = ConvertBasicInterval(fileInteractionInterval);
            obj.Properties.Add("doc", CreateFileRepresentationObject(fileInteractionInterval));
            return obj;
        }

        private static WatchdogObject ConvertTypingInterval(FileInteractionInterval fileInteractionInterval)
        {
            var obj = ConvertBasicInterval(fileInteractionInterval);
            var fileRepresentationObject = CreateFileRepresentationObject(fileInteractionInterval);
            obj.Properties.Add("doc", fileRepresentationObject);
            obj.Properties.Add("endingDoc", fileRepresentationObject);
            obj.Properties.Add("diff", Int(-1));
            return obj;
        }

        private static WatchdogObject ConvertTestRunInterval(TestRunInterval testRunInterval)
        {
            var obj = ConvertBasicInterval(testRunInterval);

            var projectObject = new WatchdogObject();
            obj.Properties.Add("testExecution", projectObject);

            projectObject.Properties.Add("projectHash", String(WatchdogUtils.Sha1Hash(testRunInterval.ProjectName)));
            projectObject.Properties.Add("result", String(testRunInterval.Result.ToSerializedName()));

            var testClassArray = new WatchdogArray();
            projectObject.Properties.Add("childrenExecutions", testClassArray);

            foreach (var testClass in testRunInterval.TestClasses)
            {
                var testClassObject = new WatchdogObject();
                testClassArray.Elements.Add(testClassObject);

                testClassObject.Properties.Add("testClassHash", String(WatchdogUtils.Sha1Hash(testClass.TestClassName)));
                testClassObject.Properties.Add("result", String(testClass.Result.ToSerializedName()));

                var testMethodArray = new WatchdogArray();
                testClassObject.Properties.Add("childrenExecutions", testMethodArray);

                foreach (var testMethod in testClass.TestMethods)
                {
                    var testMethodObject = new WatchdogObject();
                    testMethodArray.Elements.Add(testMethodObject);

                    testMethodObject.Properties.Add(
                        "testMethodHash",
                        String(WatchdogUtils.Sha1Hash(testMethod.TestMethodName)));
                    testMethodObject.Properties.Add("result", String(testMethod.Result.ToSerializedName()));
                }
            }

            return obj;
        }

        private static WatchdogObject CreateProjectObject(Interval interval)
        {
            var obj = new WatchdogObject();
            obj.Properties.Add("_id", Wrapped("ObjectId", Guid.NewGuid().ToString("N")));
            obj.Properties.Add("name", String(interval.Project));
            obj.Properties.Add("belongToASingleSoftware", Literal("true"));
            obj.Properties.Add("usesContinuousIntegration", String("Unknown"));
            obj.Properties.Add("usesJunit", String("No"));
            obj.Properties.Add("usesOtherTestingFrameworks", String("Unknown"));
            obj.Properties.Add("usesOtherTestingForms", String("Unknown"));
            obj.Properties.Add("productionPercentage", Int(0));
            obj.Properties.Add("useJunitOnlyForUnitTesting", String("No"));
            obj.Properties.Add("followTestDrivenDesign", String("Unknown"));
            // "Nov 4, 2015 1:09:58 AM"
            obj.Properties.Add(
                "localRegistrationDate",
                String(interval.CreationTime.ToString("MMM d, yyyy h:mm:ss tt", CultureInfo.InvariantCulture)));
            obj.Properties.Add("userId", String(WatchdogUtils.Sha1Hash(interval.UserId)));
            obj.Properties.Add("website", String(""));
            obj.Properties.Add("wdv", String("KaVE"));
            obj.Properties.Add("ide", String("vs"));
            obj.Properties.Add("id", String(WatchdogUtils.Sha1Hash(interval.Project)));
            obj.Properties.Add("ip", String("0.0.0.0"));
            obj.Properties.Add("regDate", Wrapped("ISODate", DateTime.Now.ToString("o")));
            return obj;
        }

        private static WatchdogObject CreateUserObject(Interval interval)
        {
            var obj = new WatchdogObject();
            obj.Properties.Add("_id", Wrapped("ObjectId", Guid.NewGuid().ToString("N")));
            obj.Properties.Add("email", String("unknown@example.org"));
            obj.Properties.Add("organization", String("Unknown"));
            obj.Properties.Add("programmingExperience", String("Unknown"));
            obj.Properties.Add("mayContactUser", Literal("false"));
            obj.Properties.Add("localRegistrationDate", Wrapped("NumberLong", interval.StartTime.ToJavaTimestamp()));
            obj.Properties.Add("operatingSystem", String("Unknown"));
            obj.Properties.Add("wdv", String("KaVE"));
            obj.Properties.Add("ide", String("vs"));
            obj.Properties.Add("id", String(WatchdogUtils.Sha1Hash(interval.UserId)));
            obj.Properties.Add("country", String("Unknown"));
            obj.Properties.Add("city", String("Unknown"));
            obj.Properties.Add("postCode", Literal(null));
            obj.Properties.Add("ip", String("0.0.0.0"));
            obj.Properties.Add("regDate", Wrapped("ISODate", DateTime.Now.ToString("o")));
            return obj;
        }
    }
}