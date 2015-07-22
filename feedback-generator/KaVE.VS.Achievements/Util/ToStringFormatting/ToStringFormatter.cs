using System;
using System.Numerics;
using KaVE.VS.Achievements.Properties;

namespace KaVE.VS.Achievements.Util.ToStringFormatting
{
    public static class ToStringFormatter
    {
        public static string Format(this int number)
        {
            return number.ToString(FormatStrings.Integer);
        }

        public static string Format(this double number)
        {
            return number.ToString(FormatStrings.Double);
        }

        public static string Format(this TimeSpan timeSpan)
        {
            if (timeSpan.Days > 0)
            {
                return Format(timeSpan.TotalDays) + " " + TimeUnits.Days;
            }
            if (timeSpan.Hours > 0)
            {
                return Format(timeSpan.TotalHours) + " " + TimeUnits.Hours;
            }
            if (timeSpan.Minutes > 0)
            {
                return Format(timeSpan.TotalMinutes) + " " + TimeUnits.Minutes;
            }
            if (timeSpan.Seconds > 0)
            {
                return Format(timeSpan.TotalSeconds) + " " + TimeUnits.Seconds;
            }
            return Format(timeSpan.TotalMilliseconds) + " " + TimeUnits.Milliseconds;
        }

        public static string Format(this BigInteger number)
        {
            return number.ToString(FormatStrings.Integer);
        }
    }
}