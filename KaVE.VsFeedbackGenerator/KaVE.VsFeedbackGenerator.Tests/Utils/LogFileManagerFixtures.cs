using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using KaVE.JetBrains.Annotations;

namespace KaVE.VsFeedbackGenerator.Tests.Utils
{
    internal static class LogFileManagerFixtures
    {
        public static Message RandomMessage(int count)
        {
            var r = new Random();
            var lines = new List<Line>();
            for (var i = 0; i < count; i++)
            {
                lines.Add(new Line {Content = r.Next().ToString(CultureInfo.InvariantCulture)});
            }
            return new Message
            {
                Header = r.Next().ToString(CultureInfo.InvariantCulture),
                Priority = r.Next(),
                HasAttachment = r.Next()%2 == 0,
                Lines = lines
            };
        }
    }
    internal class Message
    {
        [NotNull]
        public string Header { get; set; }

        public int Priority { get; set; }
        public bool HasAttachment { get; set; }

        [NotNull]
        public IEnumerable<Line> Lines { get; set; }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }
            var other = obj as Message;
            if (other == null)
            {
                return false;
            }
            return ((Header.Equals(other.Header)) && (Priority == other.Priority) &&
                    (HasAttachment == other.HasAttachment) && (Lines.SequenceEqual(other.Lines)));
        }

        public override int GetHashCode()
        {
            return ((Header.GetHashCode() * 29 + Priority) * 29 + (HasAttachment ? 1 : 0)) * 29 +
                   Lines.Aggregate(0, (h, l) => h * 29 + l.GetHashCode());
        }

        public override string ToString()
        {
            return string.Format(
                "{0} {1} {2} {3}",
                Header,
                Priority,
                HasAttachment,
                string.Join(", ", Lines.Select(l => l.ToString())));
        }
    }

    internal class Line
    {
        [NotNull]
        public string Content { get; set; }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }
            var other = obj as Line;
            if (other == null)
            {
                return false;
            }
            return Content.Equals(other.Content);
        }

        public override int GetHashCode()
        {
            return Content.GetHashCode();
        }

        public override string ToString()
        {
            return Content;
        }
    }
}