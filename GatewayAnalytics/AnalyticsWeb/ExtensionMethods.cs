using System;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ionx.GatewayAnalytics
{
    /// <summary>
    /// Provides a centralized place for common functionality exposed via extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// This string is already correctly encoded html and can be sent to the client "as is" without additional encoding.
        /// </summary>
        public static IHtmlString AsHtml(this string html)
        {
            return MvcHtmlString.Create(html);
        }

        /// <summary>
        /// Answers true if this String is either null or empty.
        /// </summary>
        /// <remarks>I'm so tired of typing String.IsNullOrEmpty(s)</remarks>
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        /// <summary>
        /// Answers true if this String is neither null or empty.
        /// </summary>
        /// <remarks>I'm also tired of typing !String.IsNullOrEmpty(s)</remarks>
        public static bool HasValue(this string s)
        {
            return !string.IsNullOrEmpty(s);
        }

        /// <summary>
        /// Returns the first non-null/non-empty parameter when this String is null/empty.
        /// </summary>
        public static string IsNullOrEmptyReturn(this string s, params string[] otherPossibleResults)
        {
            if (s.HasValue())
                return s;

            if (otherPossibleResults == null)
                return "";

            foreach (var t in otherPossibleResults)
            {
                if (t.HasValue())
                    return t;
            }
            return "";
        }
    }

    //Credits to http://stackoverflow.com/questions/128618/c-file-size-format-provider/3968504#3968504
    public static class IntToBytesExtension
    {
        private const int _precision = 2;
        private static readonly IList<string> _units;

        static IntToBytesExtension()
        {
            _units = new List<string> { "", "K", "M", "G", "T" };
        }

        /// <summary>
        /// Formats the value as a filesize in bytes (KB, MB, etc.)
        /// </summary>
        /// <param name="bytes">This value.</param>
        /// <param name="unit">Unit to use in the fomat, defaults to B for bytes</param>
        /// <param name="precision">How much precision to show, defaults to 2</param>
        /// <param name="zero">String to show if the value is 0</param>
        /// <returns>Filesize and quantifier formatted as a string.</returns>
        public static string ToSize(this int bytes, string unit = "B", int precision = _precision, string zero = "n/a")
        {
            return ToSize((double)bytes, unit, precision, zero: zero);
        }

        /// <summary>
        /// Formats the value as a filesize in bytes (KB, MB, etc.)
        /// </summary>
        /// <param name="bytes">This value.</param>
        /// <param name="unit">Unit to use in the fomat, defaults to B for bytes</param>
        /// <param name="precision">How much precision to show, defaults to 2</param>
        /// <param name="zero">String to show if the value is 0</param>
        /// <returns>Filesize and quantifier formatted as a string.</returns>
        public static string ToSize(this long bytes, string unit = "B", int precision = _precision, string zero = "n/a")
        {
            return ToSize((double)bytes, unit, precision, zero: zero);
        }

        /// <summary>
        /// Formats the value as a filesize in bytes (KB, MB, etc.)
        /// </summary>
        /// <param name="bytes">This value.</param>
        /// <param name="unit">Unit to use in the fomat, defaults to B for bytes</param>
        /// <param name="precision">How much precision to show, defaults to 2</param>
        /// <param name="zero">String to show if the value is 0</param>
        /// <returns>Filesize and quantifier formatted as a string.</returns>
        public static string ToSize(this float bytes, string unit = "B", int precision = _precision, string zero = "n/a")
        {
            return ToSize((double)bytes, unit, precision, zero: zero);
        }

        /// <summary>
        /// Formats the value as a filesize in bytes (KB, MB, etc.)
        /// </summary>
        /// <param name="bytes">This value.</param>
        /// <param name="unit">Unit to use in the fomat, defaults to B for bytes</param>
        /// <param name="precision">How much precision to show, defaults to 2</param>
        /// <param name="kiloSize">1k size, usually 1024 or 1000 depending on context</param>
        /// <param name="zero">String to show if the value is 0</param>
        /// <returns>Filesize and quantifier formatted as a string.</returns>
        public static string ToSize(this double bytes, string unit = "B", int precision = _precision, int kiloSize = 1024, string zero = "n/a")
        {
            if (bytes < 1) return zero;
            var pow = Math.Floor((bytes > 0 ? Math.Log(bytes) : 0) / Math.Log(kiloSize));
            pow = Math.Min(pow, _units.Count - 1);
            var value = bytes / Math.Pow(kiloSize, pow);
            return value.ToString(pow == 0 ? "F0" : "F" + precision.ToString()) + " " + _units[(int)pow] + unit;
        }
    }

    public static class DateToStringExtensions
    {
        /// <summary>
        /// Convert a nullable datetime to a zulu string
        /// </summary>
        public static string ToUniversalTime(this DateTime? dt)
        {
            return !dt.HasValue ? string.Empty : ToUniversalTime(dt.Value);
        }

        /// <summary>
        /// Convert a datetime to a zulu string
        /// </summary>
        public static string ToUniversalTime(this DateTime dt)
        {
            return dt.ToString("u");
        }

        public static string ToDateOnlyString(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd");
        }

        public static MvcHtmlString ToDateOnlySpanPretty(this DateTime dt, string cssClass)
        {
            return MvcHtmlString.Create(String.Format(@"<span title=""{0:u}"" class=""{1}"">{2}</span>", dt, cssClass, ToDateOnlyStringPretty(dt, DateTime.UtcNow)));
        }

        public static string ToDateOnlyStringPretty(this DateTime dt, DateTime? utcNow = null)
        {
            var now = utcNow ?? DateTime.UtcNow;
            return dt.ToString(now.Year != dt.Year ? @"MMM %d \'yy" : "MMM %d");
        }

        /// <summary>
        /// Converts a timespan to a readable string adapted from http://stackoverflow.com/a/4423615
        /// </summary>
        public static string ToReadableString(this TimeSpan span)
        {
            var dur = span.Duration();
            var sb = new StringBuilder();
            if (dur.Days > 0) sb.AppendFormat("{0:0} day{1}, ", span.Days, span.Days == 1 ? "" : "s");
            if (dur.Hours > 0) sb.AppendFormat("{0:0} hour{1}, ", span.Hours, span.Hours == 1 ? "" : "s");
            if (dur.Minutes > 0) sb.AppendFormat("{0:0} minute{1}, ", span.Minutes, span.Minutes == 1 ? "" : "s");
            if (dur.Seconds > 0) sb.AppendFormat("{0:0} second{1}, ", span.Seconds, span.Seconds == 1 ? "" : "s");

            if (sb.Length >= 2) sb.Length -= 2;
            return sb.ToString().IsNullOrEmptyReturn("0 seconds");
        }

        /// <summary>
        /// returns a html span element with relative time elapsed since this event occurred, eg, "3 months ago" or "yesterday"; 
        /// assumes time is *already* stored in UTC format!
        /// </summary>
        public static IHtmlString ToRelativeTimeSpan(this DateTime dt)
        {
            return ToRelativeTimeSpan(dt, "relativetime");
        }
        public static IHtmlString ToRelativeTimeSpan(this DateTime dt, string cssclass, bool asPlusMinus = false, DateTime? compareTo = null)
        {
            // TODO: Make this a setting?
            // UTC Time is good for Stack Exchange but many people don't run their servers on UTC
            compareTo = compareTo ?? DateTime.UtcNow;
            if (string.IsNullOrEmpty(cssclass))
                return string.Format(@"<span title=""{0:u}"">{1}</span>", dt, dt.ToRelativeTime(asPlusMinus: asPlusMinus, compareTo: compareTo)).AsHtml();
            else
                return string.Format(@"<span title=""{0:u}"" class=""{2}"">{1}</span>", dt, dt.ToRelativeTime(asPlusMinus: asPlusMinus, compareTo: compareTo), cssclass).AsHtml();
        }
        public static IHtmlString ToRelativeTimeSpan(this DateTime? dt, string cssclass = "")
        {
            return dt == null
                       ? MvcHtmlString.Empty
                       : ToRelativeTimeSpan(dt.Value, "relativetime" + (cssclass.HasValue() ? " " + cssclass : ""));
        }


        /// <summary>
        /// returns a very *small* humanized string indicating how long ago something happened, eg "3d ago"
        /// </summary>
        public static string ToRelativeTimeMini(this DateTime dt, bool includeTimeForOldDates = true, bool includeAgo = true)
        {
            var ts = new TimeSpan(DateTime.UtcNow.Ticks - dt.Ticks);
            var delta = ts.TotalSeconds;

            if (delta < 60)
            {
                return ts.Seconds + "s" + (includeAgo ? " ago" : "");
            }
            if (delta < 3600) // 60 mins * 60 sec
            {
                return ts.Minutes + "m" + (includeAgo ? " ago" : "");
            }
            if (delta < 86400)  // 24 hrs * 60 mins * 60 sec
            {
                return ts.Hours + "h" + (includeAgo ? " ago" : "");
            }
            var days = ts.Days;
            if (days <= 2)
            {
                return days + "d" + (includeAgo ? " ago" : "");
            }
            else if (days <= 330)
            {
                return dt.ToString(includeTimeForOldDates ? "MMM %d 'at' %H:mmm" : "MMM %d").ToLowerInvariant();
            }
            return dt.ToString(includeTimeForOldDates ? @"MMM %d \'yy 'at' %H:mmm" : @"MMM %d \'yy").ToLowerInvariant();
        }


        /// <summary>
        /// returns a very *small* humanized string indicating how long ago something happened, e.g. "3d 10m" or "2m 10s"
        /// </summary>
        public static string ToRelativeTimeMiniAll(this DateTime dt)
        {
            var ts = new TimeSpan(DateTime.UtcNow.Ticks - dt.Ticks);
            var delta = ts.TotalSeconds;

            if (delta < 60)
            {
                return ts.Seconds + "s";
            }
            if (delta < 3600) // 60 mins * 60 sec
            {
                return ts.Minutes + "m" + ((ts.Seconds > 0) ? " " + ts.Seconds + "s" : "");
            }
            if (delta < 86400)  // 24 hrs * 60 mins * 60 sec
            {
                return ts.Hours + "h" + ((ts.Minutes > 0) ? " " + ts.Minutes + "m" : "");
            }
            return ts.Days + "d" + ((ts.Hours > 0) ? " " + ts.Hours + "h" : "");
        }

                /// <summary>
        /// Returns a humanized string indicating how long ago something happened, eg "3 days ago".
        /// For future dates, returns when this DateTime will occur from DateTime.UtcNow.
        /// </summary>
        public static string ToRelativeTime(this DateTime dt, bool includeTime = true, bool asPlusMinus = false, DateTime? compareTo = null, bool includeSign = true)
        {
            var comp = (compareTo ?? DateTime.UtcNow);
            if (asPlusMinus)
            {
                return dt <= comp ? ToRelativeTimePastSimple(dt, comp, includeSign) : ToRelativeTimeFutureSimple(dt, comp, includeSign);
            }
            return dt <= comp ? ToRelativeTimePast(dt, comp, includeTime) : ToRelativeTimeFuture(dt, comp, includeTime);
        }
        /// <summary>
        /// Returns a humanized string indicating how long ago something happened, eg "3 days ago".
        /// For future dates, returns when this DateTime will occur from DateTime.UtcNow.
        /// If this DateTime is null, returns empty string.
        /// </summary>
        public static string ToRelativeTime(this DateTime? dt, bool includeTime = true)
        {
            if (dt == null) return "";
            return ToRelativeTime(dt.Value, includeTime);
        }

        private static string ToRelativeTimePast(DateTime dt, DateTime utcNow, bool includeTime = true)
        {
            TimeSpan ts = utcNow - dt;
            double delta = ts.TotalSeconds;

            if (delta < 1)
            {
                return "just now";
            }
            if (delta < 60)
            {
                return ts.Seconds == 1 ? "1 sec ago" : ts.Seconds + " secs ago";
            }
            if (delta < 3600) // 60 mins * 60 sec
            {
                return ts.Minutes == 1 ? "1 min ago" : ts.Minutes + " mins ago";
            }
            if (delta < 86400)  // 24 hrs * 60 mins * 60 sec
            {
                return ts.Hours == 1 ? "1 hour ago" : ts.Hours + " hours ago";
            }

            var days = ts.Days;
            if (days == 1)
            {
                return "yesterday";
            }
            if (days <= 2)
            {
                return days + " days ago";
            }
            if (utcNow.Year == dt.Year)
            {
                return dt.ToString(includeTime ? "MMM %d 'at' %H:mmm" : "MMM %d");
            }
            return dt.ToString(includeTime ? @"MMM %d \'yy 'at' %H:mmm" : @"MMM %d \'yy");
        }

        private static string ToRelativeTimeFuture(DateTime dt, DateTime utcNow, bool includeTime = true)
        {
            TimeSpan ts = dt - utcNow;
            double delta = ts.TotalSeconds;

            if (delta < 1)
            {
                return "just now";
            }
            if (delta < 60)
            {
                return ts.Seconds == 1 ? "in 1 second" : "in " + ts.Seconds + " seconds";
            }
            if (delta < 3600) // 60 mins * 60 sec
            {
                return ts.Minutes == 1 ? "in 1 minute" : "in " + ts.Minutes + " minutes";
            }
            if (delta < 86400) // 24 hrs * 60 mins * 60 sec
            {
                return ts.Hours == 1 ? "in 1 hour" : "in " + ts.Hours + " hours";
            }

            // use our own rounding so we can round the correct direction for future
            var days = (int)Math.Round(ts.TotalDays, 0);
            if (days == 1)
            {
                return "tomorrow";
            }
            if (days <= 10)
            {
                return "in " + days + " day" + (days > 1 ? "s" : "");
            }
            // if the date is in the future enough to be in a different year, display the year
            if (utcNow.Year == dt.Year)
            {
                return "on " + dt.ToString(includeTime ? "MMM %d 'at' %H:mmm" : "MMM %d");
            }
            return "on " + dt.ToString(includeTime ? @"MMM %d \'yy 'at' %H:mmm" : @"MMM %d \'yy");
        }

        private static string ToRelativeTimePastSimple(DateTime dt, DateTime utcNow, bool includeSign)
        {
            TimeSpan ts = utcNow - dt;
            var sign = includeSign ? "-" : "";
            double delta = ts.TotalSeconds;
            if (delta < 1)
                return "< 1 sec";
            if (delta < 60)
                return sign + ts.Seconds + " sec" + (ts.Seconds == 1 ? "" : "s");
            if (delta < 3600) // 60 mins * 60 sec
                return sign + ts.Minutes + " min" + (ts.Minutes == 1 ? "" : "s");
            if (delta < 86400) // 24 hrs * 60 mins * 60 sec
                return sign + ts.Hours + " hour" + (ts.Hours == 1 ? "" : "s");
            return sign + ts.Days + " days";
        }

        private static string ToRelativeTimeFutureSimple(DateTime dt, DateTime utcNow, bool includeSign)
        {
            TimeSpan ts = dt - utcNow;
            double delta = ts.TotalSeconds;
            var sign = includeSign ? "+" : "";

            if (delta < 1)
                return "< 1 sec";
            if (delta < 60)
                return sign + ts.Seconds + " sec" + (ts.Seconds == 1 ? "" : "s");
            if (delta < 3600) // 60 mins * 60 sec
                return sign + ts.Minutes + " min" + (ts.Minutes == 1 ? "" : "s");
            if (delta < 86400) // 24 hrs * 60 mins * 60 sec
                return sign + ts.Hours + " hour" + (ts.Hours == 1 ? "" : "s");
            return sign + ts.Days + " days";
        }

        /// <summary>
        /// returns AN HTML SPAN ELEMENT with minified relative time elapsed since this event occurred, eg, "3mo ago" or "yday"; 
        /// assumes time is *already* stored in UTC format!
        /// </summary>
        public static IHtmlString ToRelativeTimeSpanMini(this DateTime dt, bool includeTimeForOldDates = true)
        {
            return string.Format(@"<span title=""{0:u}"" class=""relativetime"">{1}</span>", dt, ToRelativeTimeMini(dt, includeTimeForOldDates)).AsHtml();
        }
        /// <summary>
        /// returns AN HTML SPAN ELEMENT with minified relative time elapsed since this event occurred, eg, "3mo ago" or "yday"; 
        /// assumes time is *already* stored in UTC format!
        /// If this DateTime? is null, will return empty string.
        /// </summary>
        public static IHtmlString ToRelativeTimeSpanMini(this DateTime? dt, bool includeTimeForOldDates = true)
        {
            return dt == null
                       ? MvcHtmlString.Empty
                       : ToRelativeTimeSpanMini(dt.Value, includeTimeForOldDates);
        }

        public static string ToTimeStringMini(this TimeSpan span, int maxElements = 2)
        {
            var sb = new StringBuilder();
            var elems = 0;
            Action<string, int> add = (s, i) =>
            {
                if (elems < maxElements && i > 0)
                {
                    sb.AppendFormat("{0:0}{1} ", i, s);
                    elems++;
                }
            };
            add("d", span.Days);
            add("h", span.Hours);
            add("m", span.Minutes);
            add("s", span.Seconds);
            add("ms", span.Milliseconds);

            if (sb.Length == 0) sb.Append("0");

            return sb.ToString().Trim();
        }
    }
}
