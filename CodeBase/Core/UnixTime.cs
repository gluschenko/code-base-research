using System;

namespace CodeBase
{
    class UnixTime
    {
        public static readonly DateTime unixZero = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static long UtcNow()
        {
            return (long)(DateTime.UtcNow - unixZero).TotalSeconds;
        }

        public static long Now()
        {
            return (long)(DateTime.Now - unixZero).TotalSeconds;
        }

        public static long ToTimestamp(DateTime dateTime)
        {
            return (long)Math.Floor((dateTime - unixZero).TotalSeconds);
        }

        public static DateTime ToDateTime(long timestamp)
        {
            return unixZero.AddSeconds(timestamp);
        }
    }
}
