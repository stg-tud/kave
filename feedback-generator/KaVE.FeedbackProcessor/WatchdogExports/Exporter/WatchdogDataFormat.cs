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
using System.Globalization;
using System.Linq;
using System.Text;
using KaVE.JetBrains.Annotations;
using Newtonsoft.Json;

namespace KaVE.FeedbackProcessor.WatchdogExports.Exporter
{
    public class WatchdogData
    {
        [NotNull]
        public IList<WatchdogObject> Intervals { get; set; }

        [NotNull]
        public IList<WatchdogObject> Projects { get; set; }

        [NotNull]
        public IList<WatchdogObject> Users { get; set; }

        public WatchdogData()
        {
            Intervals = new List<WatchdogObject>();
            Projects = new List<WatchdogObject>();
            Users = new List<WatchdogObject>();
        }
    }

    public abstract class WatchdogValue
    {
        public abstract override string ToString();
    }

    public class WatchdogStringValue : WatchdogValue
    {
        public string Value { get; set; }

        public override string ToString()
        {
            return JsonConvert.ToString(Value);
        }
    }

    public class WatchdogIntValue : WatchdogValue
    {
        public int Value { get; set; }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class WatchdogDoubleValue : WatchdogValue
    {
        public double Value { get; set; }

        public override string ToString()
        {
            return Value.ToString("F99", CultureInfo.InvariantCulture).TrimEnd("0".ToCharArray());
        }
    }

    public class WatchdogUnquotedLiteral : WatchdogValue
    {
        public string Value { get; set; }

        public override string ToString()
        {
            return Value;
        }
    }

    public class WatchdogWrappedValue : WatchdogValue
    {
        public string Wrapper { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return string.Format("{0}(\"{1}\")", Wrapper, Value);
        }
    }

    public class WatchdogObject : WatchdogValue
    {
        public IDictionary<string, WatchdogValue> Properties { get; private set; }

        public WatchdogObject()
        {
            Properties = new Dictionary<string, WatchdogValue>();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("{");
            foreach (var property in Properties)
            {
                sb.AppendFormat("\"{0}\":{1}", property.Key, property.Value);
                if (!Equals(property, Properties.Last()))
                {
                    sb.Append(",");
                }
            }

            sb.Append("}");
            return sb.ToString();
        }
    }

    public class WatchdogArray : WatchdogValue
    {
        public IList<WatchdogValue> Elements { get; private set; }

        public WatchdogArray()
        {
            Elements = new List<WatchdogValue>();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("[");
            foreach (var element in Elements)
            {
                sb.Append(element);
                if (!Equals(element, Elements.Last()))
                {
                    sb.Append(",");
                }
            }
            sb.Append("]");
            return sb.ToString();
        }
    }
}