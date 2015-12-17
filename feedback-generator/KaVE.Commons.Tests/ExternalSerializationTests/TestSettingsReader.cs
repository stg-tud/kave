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

namespace KaVE.Commons.Tests.ExternalSerializationTests
{
    public static class TestSettingsReader
    {
        public static Dictionary<ExternalTestSetting, string> ReadSection(string settingsFile, string section)
        {
            var settings = new Dictionary<ExternalTestSetting, string>();

            var lines = File.ReadAllLines(settingsFile);

            var inSection = false;
            foreach (var currentLine in lines.Where(currentLine => !string.IsNullOrWhiteSpace(currentLine)))
            {
                if (inSection)
                {
                    if (currentLine.StartsWith("[") && currentLine.EndsWith("]"))
                    {
                        break;
                    }

                    var setting = currentLine.Substring(0, currentLine.IndexOf('=')).ToExternalTestSetting();
                    var value = currentLine.Substring(currentLine.IndexOf('=') + 1);
                    settings.Add(setting, value);
                }
                else
                {
                    if (currentLine.Equals(string.Format("[{0}]", section)))
                    {
                        inSection = true;
                    }
                }
            }

            return settings;
        }

        public static ExternalTestSetting ToExternalTestSetting(this string value)
        {
            return (ExternalTestSetting) Enum.Parse(typeof (ExternalTestSetting), value);
        }
    }

    public enum ExternalTestSetting
    {
        SerializedType
    }
}